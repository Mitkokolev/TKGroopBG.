using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKGroopBG.Data;
using TKGroopBG.Models;

namespace TKGroopBG.Controllers
{
    [Authorize] // Гарантира, че само вписани потребители имат достъп
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- ПОТРЕБИТЕЛСКА ЧАСТ ---

        // Показва историята на поръчките само за текущия потребител
        public async Task<IActionResult> MyOrders()
        {
            var userEmail = User.Identity?.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _context.Orders
                .Where(o => o.Email == userEmail)
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        // Показва детайли за конкретна поръчка (с проверка на собственост)
        public async Task<IActionResult> Details(int id)
        {
            var userEmail = User.Identity?.Name;
            var isAdmin = User.IsInRole("Admin");

            // Включваме OrderItems, за да заредим продуктите към поръчката
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            // Сигурност: Ако не е админ и имейлът не съвпада, отказваме достъп
            if (!isAdmin && order.Email != userEmail)
            {
                return Forbid();
            }

            return View(order);
        }

        // --- АДМИНИСТРАТОРСКА ЧАСТ ---

        // Списък с всички поръчки в системата (само за админи)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .AsNoTracking()
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return View(orders);
        }

        // Промяна на статуса на поръчка (само за админи)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;
            await _context.SaveChangesAsync();

            // Пренасочваме обратно към списъка или детайлите
            return RedirectToAction(nameof(Index));
        }

        // Изтриване на поръчка (само за админи)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
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