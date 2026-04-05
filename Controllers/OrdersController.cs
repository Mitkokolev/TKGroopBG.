using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<IActionResult> MyOrders()
        {
            // 1. Взимаме имейла на логнатия човек автоматично
            var userEmail = User.Identity?.Name;

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            // 2. Търсим поръчките му. 
            // ВАЖНО: Използваме OrderDate, защото CreatedAt липсва в твоя модел (image_62e15e.png)
            var orders = await _context.Orders
                .Where(o => o.Email == userEmail)
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }   

        // Потребителят може да вижда детайли само на СВОЯ поръчка
        public async Task<IActionResult> Details(int id)
        {
            var userEmail = User.Identity?.Name;
            var isAdmin = User.IsInRole("Admin");

            // КРИТИЧНО: Добавяме .Include(o => o.OrderItems), за да се виждат продуктите!
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            // Проверка за сигурност
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
            // Админът вижда всичко с включени продукти
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .AsNoTracking()
                .OrderByDescending(o => o.Id)
                .ToListAsync();

            return View(orders);
        }

        // НОВ МЕТОД: Смяна на статус (за админ панела)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null) return NotFound();

            order.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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