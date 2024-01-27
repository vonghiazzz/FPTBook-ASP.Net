using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FPTBook.Data;
using FPTBook.Models;
using FPTBook_Test.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace FPTBook.Controllers
{
    [Authorize(Roles = "Owner")]
    public class ProductsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context, IFileService fs, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this._fileService = fs;
            _userManager = userManager;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Author).Include(p => p.Genre).Include(p => p.Publisher);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Author)
                .Include(p => p.Genre)
                .Include(p => p.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            //ViewBag.Authors = new SelectList(_context.Authorities, "Id", "Name");
            //ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name");
            //ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name");
            return RedirectToAction("Index", "Owner");
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Quantity,Price,Picture,ImageFile,PublishDate,ImportDate,PageNumber,Language,Description,Status,AuthorId,GenreId,PublisherId,OwnerId")] Product product)
        {
            if (ModelState.IsValid)
            {


                if (product.ImageFile != null)
                {
                    // Save the image file and get the result
                    var fileResult = _fileService.SaveImage(product.ImageFile);

                    // Check if image saving was successful
                    if (fileResult.Item1 == 1)
                    {
                        // Construct the correct image file path
                        product.Picture = fileResult.Item2;
                    }
                }

                if (product.ImportDate == null || product.ImportDate > DateTime.Now)
                {
                    product.ImportDate = DateTime.Now;
                }

                var user = await _userManager.GetUserAsync(User);

                product.OwnerId = user.Id;

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(null);
            }
            //ViewData["AuthorId"] = new SelectList(_context.Authorities, "Id", "Name", product.AuthorId);
            //ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Name", product.GenreId);
            //ViewData["PublisherId"] = new SelectList(_context.Publishers, "Id", "Name", product.PublisherId);
            return RedirectToAction(null);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewBag.Authors = new SelectList(_context.Authorities, "Id", "Name", product.AuthorId);
            ViewBag.Genres = new SelectList(_context.Genres, "Id", "Name", product.GenreId);
            ViewBag.Publishers = new SelectList(_context.Publishers, "Id", "Name", product.PublisherId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Quantity,Price,Picture,ImageFile,PublishDate,ImportDate,PageNumber,Language,Description,Status,AuthorId,GenreId,PublisherId,OwnerId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (product.ImageFile != null)
                    {
                        // Save the image file and get the result
                        var fileResult = _fileService.SaveImage(product.ImageFile);

                        // Check if image saving was successful
                        if (fileResult.Item1 == 1)
                        {
                            // Construct the correct image file path
                            product.Picture = fileResult.Item2;
                        }
                        else
                        {
                            // Handle the case where image saving failed
                            ViewBag.FailMessage = "Error saving the image.";
                            return View(product);
                        }
                    }

                    var user = await _userManager.GetUserAsync(User);
                    product.OwnerId = user.Id;

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Owner");
            }
            ViewBag.Authors = new SelectList(_context.Authorities, "Id", "Name", product.AuthorId);
            ViewBag.Genres = new SelectList(_context.Genres, "Id", "Name", product.GenreId);
            ViewBag.Publishers = new SelectList(_context.Publishers, "Id", "Name", product.PublisherId);
            return RedirectToAction(null);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Author)
                .Include(p => p.Genre)
                .Include(p => p.Publisher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Owner");
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}