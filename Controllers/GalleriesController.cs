using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKGroopBG.Data;
using TKGroopBG.Models;

namespace TKGroopBG.Controllers
{
    public class GalleriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public GalleriesController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: Galleries (видима за всички)
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var items = await _context.Gallery.AsNoTracking().ToListAsync();
            return View(items);
        }

        // GET: Galleries/Details/5 (видима за всички)
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var gallery = await _context.Gallery
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gallery == null) return NotFound();

            return View(gallery);
        }

        // GET: Galleries/Create (само админ)
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Galleries/Create (само админ)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Gallery gallery, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
                return View(gallery);

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsRoot = Path.Combine(_env.WebRootPath, "gallery-images");
                Directory.CreateDirectory(uploadsRoot);

                var ext = Path.GetExtension(imageFile.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsRoot, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await imageFile.CopyToAsync(stream);
                }

                gallery.ImageFileName = fileName;
            }

            _context.Add(gallery);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Galleries/Edit/5 (само админ)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var gallery = await _context.Gallery.FindAsync(id);
            if (gallery == null) return NotFound();

            return View(gallery);
        }

        // POST: Galleries/Edit/5 (само админ)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Gallery gallery, IFormFile? imageFile)
        {
            if (id != gallery.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(gallery);

            // ако има нов файл – качваме и сменяме
            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsRoot = Path.Combine(_env.WebRootPath, "gallery-images");
                Directory.CreateDirectory(uploadsRoot);

                var ext = Path.GetExtension(imageFile.FileName);
                var fileName = $"{Guid.NewGuid()}{ext}";
                var filePath = Path.Combine(uploadsRoot, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await imageFile.CopyToAsync(stream);
                }

                gallery.ImageFileName = fileName;
            }

            try
            {
                _context.Update(gallery);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Gallery.Any(e => e.Id == gallery.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Galleries/Delete/5 (само админ)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var gallery = await _context.Gallery
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gallery == null) return NotFound();

            return View(gallery);
        }

        // POST: Galleries/Delete/5 (само админ)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gallery = await _context.Gallery.FindAsync(id);
            if (gallery != null)
            {
                _context.Gallery.Remove(gallery);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
