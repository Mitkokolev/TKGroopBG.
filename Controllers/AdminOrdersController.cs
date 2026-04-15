using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TKGroopBG.Data;
using TKGroopBG.Models;

namespace TKGroopBG.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Показва всички поръчки с опция за ТЪРСЕНЕ
        public async Task<IActionResult> Index(string searchString)
        {
            // 1. Стартираме заявката
            var ordersQuery = _context.Orders.AsQueryable();

            // 2. Проверяваме дали потребителят търси нещо
            if (!string.IsNullOrEmpty(searchString))
            {
                // Филтрираме по име на клиент или телефон
                ordersQuery = ordersQuery.Where(o => o.CustomerName.Contains(searchString)
                                                  || o.Phone.Contains(searchString));
            }

            // 3. Запазваме филтъра в ViewData, за да се вижда в текстовото поле на Index.cshtml
            ViewData["CurrentFilter"] = searchString;

            // 4. Подреждаме по дата (най-новите първи) и взимаме списъка
            var orders = await ordersQuery
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return View(orders);
        }

        // Методът за СМЯНА на статуса
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            // Връщаме се към Index, като запазваме текущото търсене, ако има такова
            return RedirectToAction(nameof(Index), new { searchString = ViewData["CurrentFilter"] });
        }

        // Изтриване на поръчка
        [HttpPost]
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