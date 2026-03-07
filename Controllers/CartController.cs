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

        // Поправих конструктора: само една дефиниция за _context
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
            // 1. Десериализираме количката от JSON
            var items = JsonSerializer.Deserialize<List<CartItemDto>>(model.CartJson);

            if (items == null || !items.Any())
            {
                ModelState.AddModelError(string.Empty, "Количката е празна.");
            }

            if (!ModelState.IsValid)
                return View("Order", model);

            // 2. Създаваме поръчката
            var order = new Order
            {
                CustomerName = model.CustomerName,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.Address,
                Comment = model.Comment,
                CreatedAt = DateTime.Now,
                Status = "Нова" // Важно: задай статус
            };

            // 3. Добавяме артикулите
            foreach (var item in items)
            {
                order.Items.Add(new OrderItem
                {
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = item.Qty,
                    Image = item.Image
                });

                order.TotalPrice += item.Price * item.Qty;
            }

            // 4. Запис в БД
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // 5. Email изпращане
            await _emailService.SendOrderEmailAsync(model);

            TempData["OrderSuccess"] = "Поръчката беше изпратена успешно.";
            return RedirectToAction("Index", "Home");
        }
    }
}


