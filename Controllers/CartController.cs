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

        // GET: /Cart
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Cart/Order
        [HttpGet]
        public IActionResult Order()
        {
            return View(new OrderRequest());
        }

        // POST: /Cart/SubmitOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitOrder(OrderRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.CartJson))
            {
                ModelState.AddModelError("", "Количката е празна.");
            }

            if (!ModelState.IsValid)
            {
                return View("Order", model);
            }

            // deserialize cart
            var items = JsonSerializer.Deserialize<List<CartItemDto>>(model.CartJson);

            if (items == null || !items.Any())
            {
                ModelState.AddModelError("", "Няма продукти.");
                return View("Order", model);
            }

            // create order
            var order = new Order
            {
                CustomerName = model.CustomerName,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.Address,
                Comment = model.Comment,
                CreatedAt = DateTime.Now
            };

            foreach (var item in items)
            {
                order.Items.Add(new OrderItem
                {
                    Name = item.name,
                    Price = item.price,
                    Quantity = item.qty,
                    Image = item.image
                });

                order.TotalPrice += item.price * item.qty;
            }

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // email
            await _emailService.SendOrderEmailAsync(model);

            TempData["OrderSuccess"] = "Поръчката беше изпратена успешно.";
            return RedirectToAction("Index", "Home");
        }
    }
}