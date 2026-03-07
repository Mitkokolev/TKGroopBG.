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
        private readonly ApplicationDbContext _context;

        public CartController(IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // прост view – количката се рисува с JS от localStorage
            return View();
        }

        [HttpGet]
        public IActionResult Order() => View(new OrderRequest());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitOrder(OrderRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.CartJson))
            {
                ModelState.AddModelError(string.Empty, "Количката е празна.");
            }

            if (!ModelState.IsValid)
                return View("Order", model);

            var order = new Order
            {
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


