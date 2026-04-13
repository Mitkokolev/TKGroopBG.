using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using TKGroopBG.Data;
using TKGroopBG.Models;
using Microsoft.AspNetCore.Http;

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

        // GET: Galleries
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var items = await _context.Gallery.AsNoTracking().ToListAsync();
            return View(items);
        }

        // GET: Galleries/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var gallery = await _context.Gallery
                .Include(g => g.Images)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (gallery == null) return NotFound();

            return View(gallery);
        }

        // GET: Galleries/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Galleries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Gallery gallery, List<IFormFile> imageFiles)
        {
            if (!ModelState.IsValid)
                return View(gallery);

            var uploadsRoot = Path.Combine(_env.WebRootPath, "gallery-images");
            Directory.CreateDirectory(uploadsRoot);

            if (imageFiles != null && imageFiles.Count > 0)
            {
                for (int i = 0; i < imageFiles.Count; i++)
                {
                    var file = imageFiles[i];
                    if (file.Length > 0)
                    {
                        var ext = Path.GetExtension(file.FileName);
                        var fileName = $"{Guid.NewGuid()}{ext}";
                        var filePath = Path.Combine(uploadsRoot, fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await file.CopyToAsync(stream);
                        }

                        if (i == 0)
                        {
                            gallery.ImageFileName = fileName;
                        }

                        gallery.Images.Add(new GalleryImage { FileName = fileName });
                    }
                }
            }

            _context.Add(gallery);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Galleries/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var gallery = await _context.Gallery.Include(g => g.Images).FirstOrDefaultAsync(g => g.Id == id);
            if (gallery == null) return NotFound();

            return View(gallery);
        }

        // POST: Galleries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Gallery gallery, List<IFormFile>? imageFiles)
        {
            if (id != gallery.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(gallery);

            var uploadsRoot = Path.Combine(_env.WebRootPath, "gallery-images");
            Directory.CreateDirectory(uploadsRoot);

            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var file in imageFiles)
                {
                    if (file.Length > 0)
                    {
                        var ext = Path.GetExtension(file.FileName);
                        var fileName = $"{Guid.NewGuid()}{ext}";
                        var filePath = Path.Combine(uploadsRoot, fileName);

                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await file.CopyToAsync(stream);
                        }

                        _context.GalleryImages.Add(new GalleryImage
                        {
                            FileName = fileName,
                            GalleryId = gallery.Id
                        });
                    }
                }
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

        // GET: Galleries/Delete/5
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

        // POST: Galleries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gallery = await _context.Gallery.Include(g => g.Images).FirstOrDefaultAsync(g => g.Id == id);
            if (gallery != null)
            {
                foreach (var img in gallery.Images)
                {
                    var filePath = Path.Combine(_env.WebRootPath, "gallery-images", img.FileName);
                    if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                }

                _context.Gallery.Remove(gallery);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // AJAX POST: Galleries/DeleteImage/5
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.GalleryImages.FindAsync(id);
            if (image == null)
            {
                return Json(new { success = false, message = "Снимката не е намерена." });
            }

            try
            {
                var filePath = Path.Combine(_env.WebRootPath, "gallery-images", image.FileName);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                _context.GalleryImages.Remove(image);
                await _context.SaveChangesAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}