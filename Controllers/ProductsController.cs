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

        // ================== ПОМОЩЕН МЕТОД ЗА ЗАПИС НА ФАЙЛ ==================
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

            var items = await _context.Products
                .AsNoTracking()
                .Where(p => p.Category == id)
                .ToListAsync();

            ViewBag.Category = id;
            return View("List", items);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Variants)
                .Include(p => p.Colors)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // ================== CREATE (Admin) ==================

        [Authorize(Roles = "Admin")]
        public IActionResult Create(string? category)
        {
            ViewBag.Categories = Categories;
            return View(new Products { Category = category });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Products product, IFormFile? imageFile, IFormFileCollection galleryFiles, IFormFileCollection colorFiles, string variantsJson, string colorsJson)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = Categories;
                return View(product);
            }

            // 1. Основна снимка
            var fileName = await SaveImageAsync(imageFile);
            if (fileName != null) product.ImageFileName = fileName;

            _context.Add(product);
            await _context.SaveChangesAsync();

            // 2. Запис на общата Галерия
            if (galleryFiles != null && galleryFiles.Count > 0)
            {
                foreach (var file in galleryFiles)
                {
                    var path = await SaveImageAsync(file);
                    if (path != null)
                    {
                        _context.ProductImages.Add(new ProductImage
                        {
                            ProductId = product.Id,
                            ImagePath = path
                        });
                    }
                }
            }

            // 3. Запис на Вариантите (стъклопакети)
            if (!string.IsNullOrEmpty(variantsJson))
            {
                var variants = JsonConvert.DeserializeObject<List<ProductVariant>>(variantsJson);
                if (variants != null)
                {
                    foreach (var v in variants)
                    {
                        v.ProductId = product.Id;
                        _context.ProductVariants.Add(v);
                    }
                }
            }

            // 4. Запис на Цветовете и техните снимки
            if (!string.IsNullOrEmpty(colorsJson))
            {
                var colors = JsonConvert.DeserializeObject<List<ProductColor>>(colorsJson);
                if (colors != null)
                {
                    for (int i = 0; i < colors.Count; i++)
                    {
                        var colorObj = colors[i];
                        colorObj.ProductId = product.Id;
                        _context.ProductColors.Add(colorObj);

                        // Проверяваме дали има качена снимка за този индекс на цвят
                        if (colorFiles != null && i < colorFiles.Count)
                        {
                            var colorImgPath = await SaveImageAsync(colorFiles[i]);
                            if (colorImgPath != null)
                            {
                                // Важно: ColorHex ни трябва за филтрация в Details.cshtml
                                _context.ProductImages.Add(new ProductImage
                                {
                                    ProductId = product.Id,
                                    ImagePath = colorImgPath,
                                    ColorHex = colorObj.ColorHex
                                });
                            }
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Category), new { id = product.Category });
        }

        // ================== DELETE (Admin) ==================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}