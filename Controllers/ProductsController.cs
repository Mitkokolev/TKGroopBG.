using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TKGroopBG.Data;
using TKGroopBG.Models;

namespace TKGroopBG.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        // ТУК си държим категориите и ги ползваме навсякъде
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

        // ================== помощен метод за снимката ==================

        private async Task<string?> SaveImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "product-images");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return fileName;
        }

        // ================== КАТЕГОРИИ ==================

        // /Products -> показва плочки с категории (като home)
        [AllowAnonymous]
        public IActionResult Index()
        {
            // подаваме листа от категории към view-то "Categories"
            return View("Categories", Categories);
        }

        // /Products/Category?id=PVC%20дограма
        // показва продуктите от дадена категория
        [AllowAnonymous]
        public async Task<IActionResult> Category(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return RedirectToAction(nameof(Index));
            }

            var items = await _context.Products
                .AsNoTracking()
                .Where(p => p.Category == id)
                .ToListAsync();

            ViewBag.Category = id;
            return View("List", items);
        }

        // ================== ДЕТАЙЛИ (публично) ==================

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
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

            // ако идваш от "Добави продукт в {category}" – попълваме Category
            var model = new Products
            {
                Category = category
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Products products, IFormFile? imageFile)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = Categories;
                return View(products);
            }

            // качваме снимката, ако има
            var fileName = await SaveImageAsync(imageFile);
            if (fileName != null)
            {
                products.ImageFileName = fileName;
            }

            _context.Add(products);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ================== EDIT (Admin) ==================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var products = await _context.Products.FindAsync(id);
            if (products == null) return NotFound();

            ViewBag.Categories = Categories;
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Products products, IFormFile? imageFile)
        {
            if (id != products.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = Categories;
                return View(products);
            }

            var existing = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);

            if (existing == null) return NotFound();

            // ако качим нова снимка – сменяме я
            var fileName = await SaveImageAsync(imageFile);
            if (fileName != null)
            {
                products.ImageFileName = fileName;
            }
            else
            {
                // запазваме старата
                products.ImageFileName = existing.ImageFileName;
            }

            try
            {
                _context.Update(products);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                bool exists = await _context.Products.AnyAsync(e => e.Id == products.Id);
                if (!exists) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // ================== DELETE (Admin) ==================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var products = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (products == null) return NotFound();

            return View(products);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var products = await _context.Products.FindAsync(id);
            if (products != null)
            {
                _context.Products.Remove(products);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
