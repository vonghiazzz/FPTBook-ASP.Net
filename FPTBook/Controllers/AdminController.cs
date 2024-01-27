using FPTBook.Data;
using FPTBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Cryptography;
using System.Text;

namespace FPTBook.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var adminId = _userManager.GetUserId(User);
            var adminPicture = _context.Users.FirstOrDefault(a=>a.Id==adminId)?.ProfilePicture;
            var adminName = _context.Users.FirstOrDefault(a => a.Id == adminId)?.Name;

            var customerList = _userManager.GetUsersInRoleAsync("Customer").Result;
            var ownerList = _userManager.GetUsersInRoleAsync("Owner").Result;

            var newGenreList = (from newGenre in _context.NewGenres
                                where newGenre.Status == null
                                join owner in _context.Users
                                on newGenre.OwnerId equals owner.Id
                                select new
                                {
                                    NewGenres = newGenre,
                                    Owner = owner
                                }).ToList();



            var genreList = _context.Genres.ToList();

            var customerSearchTerm = Request.Query["search-term"];
            var ownerSearchTerm = Request.Query["search-term-owner"];

            if (customerSearchTerm.IsNullOrEmpty())
            {
                customerSearchTerm = "";
                ViewData["CustomerSearch"] = true;
            }
            else
            {
                ViewData["CustomerSearch"] = false;
            }
            var customerListSearch = (from u in _context.Users
                                      join ur in _context.UserRoles on u.Id equals ur.UserId
                                      join r in _context.Roles on ur.RoleId equals r.Id
                                      where r.Name == "Customer" && u.Name.Contains(customerSearchTerm)
                                      select new
                                      {
                                          Customer = u
                                      }).ToList();

            if (ownerSearchTerm.IsNullOrEmpty())
            {
                ownerSearchTerm = "";
                ViewData["OwnerSearch"] = true;
            }
            else
            {
                ViewData["OwnerSearch"] = false;
            }
            var ownerListSearch = (from u in _context.Users
                                      join ur in _context.UserRoles on u.Id equals ur.UserId
                                      join r in _context.Roles on ur.RoleId equals r.Id
                                      where r.Name == "Owner" && u.Name.Contains(ownerSearchTerm)
                                      select new
                                      {
                                          Owner = u
                                      }).ToList();

            string userPassword = Convert.ToString(TempData["UserPassword"]) ;


            ViewData["NewGenre"] = newGenreList;
            ViewData["AdminPicture"] = adminPicture;
            ViewData["AdminName"] = adminName;
            ViewData["CustomerList"] = customerList;
            ViewData["OwnerList"] = ownerList;
            ViewData["GenreList"] = genreList;
            ViewData["CustomerListSearch"] = customerListSearch;
            ViewData["OwnerListSearch"] = ownerListSearch;
            ViewData["UserPassword"] = userPassword;

            TempData["AdminPciture"] = ViewData["AdminPicture"];

            return View("Views/Admin/AdminScreen.cshtml");
        }
        public async Task<IActionResult> GenreAccept(int id)
        {
            var newGenre = await _context.NewGenres.FindAsync(id);
            newGenre.Status = true;
            _context.Update(newGenre);
            await _context.SaveChangesAsync();

            var newGenreName = _context.NewGenres.FirstOrDefault(n => n.Id == id)?.Name;
            Genre genreItem = new Genre
            {
                Name = newGenreName
            };
            _context.Add(genreItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult SearchCustomer()
        {
            var searchTerm = Request.Query["search-term"];
            var customerList = _userManager.GetUsersInRoleAsync("Customer").Result;
            var customerSearch = customerList
                                .Where(c => c.Name.Contains(searchTerm))
                                .ToList();

            ViewData["CustomerSearch"] = customerSearch;
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CreateGenre(string GenreName)
        {
            Genre genre = new Genre
            {
                Name = GenreName
            };

            _context.Add(genre);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> UpdateGenre(int id, string GenreName)
        {
            var genreItem = _context.Genres.FirstOrDefault(g=>g.Id == id);
            if (genreItem != null)
            {
                if(genreItem.Name != GenreName && !GenreName.IsNullOrEmpty())
                {
                    genreItem.Name = GenreName;
                    _context.Update(genreItem);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
            }

            var adminId = _userManager.GetUserId(User);
            var adminPicture = _context.Users.FirstOrDefault(a => a.Id == adminId)?.ProfilePicture;
            ViewData["AdminPicture"] = adminPicture;
            ViewData["GenreItem"] = genreItem;
            return View("Views/Admin/GenreUpdate.cshtml");
        }

        public async Task<IActionResult> DeleteGenre(int id)
        {
            var genreItem = _context.Genres.FirstOrDefault(g=>g.Id==id);
            if (genreItem != null)
            {
                _context.Remove(genreItem);
                await _context.SaveChangesAsync();
                
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ResetUserPassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            string passwordReset = "User@" + new Random().Next(100000, 999999);
            if (user != null)
            {
                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, passwordReset);
            }

            TempData["UserPassword"] = passwordReset;
            return RedirectToAction("Index");
        }
    }
}
