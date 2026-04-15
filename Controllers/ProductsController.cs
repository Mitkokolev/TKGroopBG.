using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TKGroopBG.Data;
using TKGroopBG.Models;
using Newtonsoft.Json;

namespace TKGroopBG.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] Categories = new[]
        {
            "Алуминиеви изделия",
            "PVC дограма",
            "Щори",
            "Врати",
            "Мрежи против насекоми"
        };

        public ProductsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        private async Task<string?> SaveImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "product-images");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return fileName;
        }

        // ================== ПУБЛИЧНИ СТРАНИЦИ ==================
        [AllowAnonymous]
        public IActionResult Index() => View("Categories", Categories);

        [AllowAnonymous]
        public async Task<IActionResult> Category(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return RedirectToAction(nameof(Index));
            var items = await _context.Products.AsNoTracking().Where(p => p.Category == id).ToListAsync();
            ViewBag.Category = id;
            return View("List", items);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products
                .Include(p => p.Images).Include(p => p.Variants).Include(p => p.Colors)
                .AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            return product == null ? NotFound() : View(product);
        }

        // ================== CREATE ==================
        [Authorize(Roles = "Admin")]
        public IActionResult Create(string? category)
        {
            ViewBag.Categories = Categories;
            return View(new Products { Category = category });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Products product, IFormFile? imageFile, IFormFileCollection colorFiles, string variantsJson, string colorsJson)
        {
            if (!ModelState.IsValid) { ViewBag.Categories = Categories; return View(product); }

            product.ImageFileName = await SaveImageAsync(imageFile);
            _context.Add(product);
            await _context.SaveChangesAsync();

            // Логика за варианти и цветове (аналогична на Edit, но опростена за нов продукт)
            await ProcessVariantsAndColors(product.Id, variantsJson, colorsJson, colorFiles);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Category), new { id = product.Category });
        }

        // ================== EDIT ==================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products
                .Include(p => p.Variants).Include(p => p.Colors).Include(p => p.Images)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) return NotFound();
            ViewBag.Categories = Categories;
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Products product, IFormFile? imageFile, IFormFileCollection colorFiles, string variantsJson, string colorsJson)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Основна снимка
                    if (imageFile != null) product.ImageFileName = await SaveImageAsync(imageFile);
                    else _context.Entry(product).Property(x => x.ImageFileName).IsModified = false;

                    _context.Update(product);

                    // 2. Обработка на Варианти и Цветове
                    await ProcessVariantsAndColors(id, variantsJson, colorsJson, colorFiles);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.Id == product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Category), new { id = product.Category });
            }
            ViewBag.Categories = Categories;
            return View(product);
        }

        // ПОМОЩЕН МЕТОД ЗА ОБРАБОТКА НА JSON И ФАЙЛОВЕ
        private async Task ProcessVariantsAndColors(int productId, string variantsJson, string colorsJson, IFormFileCollection colorFiles)
        {
            // --- Варианти ---
            var oldVariants = _context.ProductVariants.Where(v => v.ProductId == productId);
            _context.ProductVariants.RemoveRange(oldVariants);

            if (!string.IsNullOrEmpty(variantsJson))
            {
                var variants = JsonConvert.DeserializeObject<List<ProductVariant>>(variantsJson);
                if (variants != null)
                {
                    foreach (var v in variants) { v.ProductId = productId; v.Id = 0; _context.ProductVariants.Add(v); }
                }
            }

            // --- Цветове и Снимки ---
            // Вземаме съществуващите снимки, за да не ги загубим
            var existingImages = await _context.ProductImages.Where(i => i.ProductId == productId && i.ColorHex != null).ToListAsync();

            // Махаме старите цветове
            _context.ProductColors.RemoveRange(_context.ProductColors.Where(c => c.ProductId == productId));

            if (!string.IsNullOrEmpty(colorsJson))
            {
                // Използваме dynamic за лесен достъп до FileIndex
                var colorsData = JsonConvert.DeserializeObject<List<dynamic>>(colorsJson);
                int fileCounter = 0;

                if (colorsData != null)
                {
                    foreach (var cData in colorsData)
                    {
                        string cName = cData.ColorName;
                        string cHex = cData.ColorHex;
                        int fIndex = (int)cData.FileIndex;

                        _context.ProductColors.Add(new ProductColor { ProductId = productId, ColorName = cName, ColorHex = cHex });

                        // Проверка за нова снимка
                        if (fIndex != -1 && colorFiles != null && fileCounter < colorFiles.Count)
                        {
                            // Изтриваме старата снимка за този цвят (ако има)
                            var oldImg = existingImages.FirstOrDefault(i => i.ColorHex == cHex);
                            if (oldImg != null) _context.ProductImages.Remove(oldImg);

                            // Записваме новата
                            var path = await SaveImageAsync(colorFiles[fileCounter]);
                            if (path != null)
                                _context.ProductImages.Add(new ProductImage { ProductId = productId, ImagePath = path, ColorHex = cHex });

                            fileCounter++;
                        }
                    }
                }
            }
        }

        // ================== DELETE ==================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            return product == null ? NotFound() : View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null) { _context.Products.Remove(product); await _context.SaveChangesAsync(); }
            return RedirectToAction(nameof(Index));
        }
    }
}