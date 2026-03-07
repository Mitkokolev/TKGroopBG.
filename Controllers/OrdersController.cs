using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKGroopBG.Data;
using TKGroopBG.Models;

namespace TKGroopBG.Controllers
{
    [Authorize] // Всички методи изискват логнат потребител
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public OrdersController(ApplicationDbContext context) => _context = context;

        // --- ПОТРЕБИТЕЛСКА ЧАСТ ---

        // Метод за "Моите поръчки" - достъпен за всеки логнат клиент
        public async Task<IActionResult> MyOrders()
        {
            // Взимаме имейла на текущо логнатия потребител
            var userEmail = User.Identity?.Name;

            var orders = await _context.Orders
                .Where(o => o.Email == userEmail)
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        // Потребителят може да вижда детайли само на СВОЯ поръчка
        public async Task<IActionResult> Details(int id)
        {
            var userEmail = User.Identity?.Name;
            var isAdmin = User.IsInRole("Admin");

            var order = await _context.Orders.AsNoTracking().FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            // Проверка за сигурност: Ако не е админ и имейлът не съвпада, отказваме достъп
            if (!isAdmin && order.Email != userEmail)
            {
                return Forbid();
            }

            return View(order);
        }

        // --- АДМИНИСТРАТОРСКА ЧАСТ ---

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

