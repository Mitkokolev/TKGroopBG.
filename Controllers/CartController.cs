using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using TKGroopBG.Models;
using TKGroopBG.Services;
using TKGroopBG.Data;

namespace TKGroopBG.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public CartController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Order() => View(new OrderRequest());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitOrder(OrderRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.CartJson))
            {
                ModelState.AddModelError("", "Количката е празна.");
                return View("Order", model);
            }

            if (!ModelState.IsValid) return View("Order", model);

            var order = new Order
            {
                CustomerName = model.CustomerName,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.Address,
                Comment = model.Comment,
                CartJson = model.CartJson,
                CreatedAt = DateTime.Now,
                Status = "Нова"
            };

            // Пресмятане на общата сума от JSON-а
            try
            {
                var items = JsonSerializer.Deserialize<List<CartItemDto>>(model.CartJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (items != null)
                {
                    foreach (var item in items)
                    {
                        order.TotalPrice += item.Price * item.Qty;
                    }
                }
            }
            catch { /* Логика за грешка при десериализация */ }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await _emailService.SendOrderEmailAsync(model);

            TempData["OrderSuccess"] = "Поръчката беше изпратена успешно.";
            return RedirectToAction("Index", "Home");
        }
    }

    public class CartItemDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
    }
}
