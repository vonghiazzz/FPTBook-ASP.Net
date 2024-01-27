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
using Microsoft.Extensions.FileProviders;
using System.Net;

namespace FPTBook.Controllers
{
    [Authorize(Roles = "Owner")]
    public class AuthorsController : Controller
    {
        private readonly IFileService _fileService;
        private readonly ApplicationDbContext _context;

        public AuthorsController(ApplicationDbContext context, IFileService fs)
        {
            _context = context;
            this._fileService = fs;
        }

        // GET: Authors
        public async Task<IActionResult> Index()
        {
            return _context.Authorities != null ?
                        View(await _context.Authorities.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Authorities'  is null.");
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Authorities == null)
            {
                return NotFound();
            }

            var author = await _context.Authorities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return RedirectToAction("Index", "Owner");
        }

        // POST: Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Author author, IFormFile imageFile = null)
        {
            if (ModelState.IsValid)
            {
                var formCollection = HttpContext.Request.Form;

                author.Name = formCollection["author-name"];
                author.Description = formCollection["author-description"];
                if (imageFile != null)
                {
                    // Save the image file and get the result
                    var fileResult = _fileService.SaveImage(imageFile);

                    // Check if image saving was successful
                    if (fileResult.Item1 == 1)
                    {
                        // Construct the correct image file path
                        author.Picture = fileResult.Item2;
                    }
                }

                _context.Add(author);
                var result = await _context.SaveChangesAsync();

                if (result == 1)
                {
                    ViewBag.SuccessMessage = "Author was successfully created.";
                    return RedirectToAction(null);
                }
                else
                {
                    ViewBag.FailMessage = "Failed.";
                }
            }
            return RedirectToAction(null);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Authorities == null)
            {
                return NotFound();
            }

            var author = await _context.Authorities.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Picture,ImageFile")] Author author)
        {
            if (id != author.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (author.ImageFile != null)
                    {
                        // Save the image file and get the result
                        var fileResult = _fileService.SaveImage(author.ImageFile);

                        // Check if image saving was successful
                        if (fileResult.Item1 == 1)
                        {
                            // Construct the correct image file path
                            author.Picture = fileResult.Item2;
                        }
                        else
                        {
                            // Handle the case where image saving failed
                            ViewBag.FailMessage = "Error saving the image.";
                            return View(author);
                        }
                    }

                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Authorities == null)
            {
                return NotFound();
            }

            var author = await _context.Authorities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Authorities == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Authorities'  is null.");
            }
            var author = await _context.Authorities.FindAsync(id);
            if (author != null)
            {
                _context.Authorities.Remove(author);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
            return (_context.Authorities?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
