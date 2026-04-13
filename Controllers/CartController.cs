using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TKGroopBG.Data;
using TKGroopBG.Models;

namespace TKGroopBG.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CartController(ApplicationDbContext context) => _context = context;

        // 1. ПОКАЗВАНЕ НА КОЛИЧКАТА
        public async Task<IActionResult> Index()
        {
            var userEmail = User.Identity?.Name;
            var cartItems = await _context.Cart
                .Include(c => c.Product)
                .Where(c => c.UserEmail == userEmail)
                .ToListAsync();
            return View(cartItems);
        }

        // 2. ДОБАВЯНЕ В КОЛИЧКАТА
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity, string details)
        {
            var userEmail = User.Identity?.Name;
            if (userEmail == null) return Unauthorized();

            var existingItem = await _context.Cart
                .FirstOrDefaultAsync(c => c.ProductId == productId
                                          && c.UserEmail == userEmail
                                          && c.Details == details);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                _context.Cart.Add(new Cart
                {
                    ProductId = productId,
                    UserEmail = userEmail,
                    Quantity = quantity,
                    Details = details,
                    DateCreated = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        // 3. ПРЕМАХВАНЕ ОТ КОЛИЧКАТА
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userEmail = User.Identity?.Name;
            var item = await _context.Cart.FirstOrDefaultAsync(c => c.Id == id && c.UserEmail == userEmail);

            if (item != null)
            {
                _context.Cart.Remove(item);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }

        // 4. ПОКАЗВАНЕ НА СТРАНИЦАТА ЗА ПОРЪЧКА
        [HttpGet]
        public async Task<IActionResult> Order()
        {
            var userEmail = User.Identity?.Name;
            var cartItems = await _context.Cart
                .Include(c => c.Product)
                .Where(c => c.UserEmail == userEmail)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction("Index");
            }

            return View(cartItems);
        }

        // 5. ФИНАЛИЗИРАНЕ НА ПОРЪЧКАТА (Коригиран за работа с базата данни)
        [HttpPost]
        public async Task<IActionResult> FinalizeOrder([FromBody] OrderRequest request)
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail)) return Unauthorized();

            // Вземаме продуктите от SQL количката, а не от JS заявката
            var cartItems = await _context.Cart
                .Include(c => c.Product)
                .Where(c => c.UserEmail == userEmail)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return BadRequest(new { success = false, message = "Количката е празна в базата данни!" });
            }

            var order = new Orders
            {
                Email = userEmail,
                FirstName = request.FirstName ?? "",
                LastName = request.LastName ?? "",
                CustomerName = $"{request.FirstName} {request.LastName}".Trim(),
                Phone = request.Phone ?? "",
                Address = request.Address ?? "",
                City = request.City ?? "",
                Comment = request.Comment ?? "",
                CartJson = "[]",
                TotalPrice = cartItems.Sum(i => (i.Product?.Price ?? 0) * i.Quantity),
                OrderDate = DateTime.Now,
                CreatedAt = DateTime.Now,
                Status = "Нова",
                OrderItems = cartItems.Select(i => new OrderItems
                {
                    ProductName = i.Product?.Name ?? "Продукт",
                    Price = i.Product?.Price ?? 0,
                    Quantity = i.Quantity,
                    Image = i.Product?.ImageFileName ?? "",
                    ConfigurationDetails = i.Details ?? "" // ТУК прехвърляме детайлите
                }).ToList()
            };

            _context.Orders.Add(order);

            // ИЗЧИСТВАМЕ КОЛИЧКАТА В БАЗАТА
            _context.Cart.RemoveRange(cartItems);

            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}