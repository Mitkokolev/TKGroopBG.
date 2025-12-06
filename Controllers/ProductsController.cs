using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKGroopBG.Data;
using TKGroopBG.Models;

namespace TKGroopBG.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // ТУК си държим категориите и ги ползваме навсякъде
        private static readonly string[] Categories = new[]
        {
            "Алуминиеви изделия",
            "PVC дограма",
            "Щори",
            "Врати",
            "Мрежи против насекоми"
        };

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
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
        public IActionResult Create()
        {
            // ТУК вече подаваме реален масив, не null
            ViewBag.Categories = Categories;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Category")] Products products)
        {
            if (!ModelState.IsValid)
            {
                // при грешки пак подаваме категориите към ViewBag
                ViewBag.Categories = Categories;
                return View(products);
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

            // ако Edit view-то ти има dropdown за Category – подаваме отново
            ViewBag.Categories = Categories;
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Category")] Products products)
        {
            if (id != products.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = Categories;
                return View(products);
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
