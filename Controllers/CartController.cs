using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TKGroopBG.Models;
using TKGroopBG.Services;
using TKGroopBG.Data;

namespace TKGroopBG.Controllers
{
    public class CartController : Controller
    {
        private readonly IEmailService _emailService;
        private readonly ApplicationDbContext _context;

        public CartController(IEmailService emailService, ApplicationDbContext context)
        {
            _emailService = emailService;
            _context = context;
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
                ModelState.AddModelError(string.Empty, "Количката е празна.");

            if (!ModelState.IsValid)
                return View("Order", model);

            var order = new Order
            {
                CustomerName = model.CustomerName,
                Phone = model.Phone,
                Email = model.Email,
                Address = model.Address,
                Comment = model.Comment,
                CartJson = model.CartJson
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            await _emailService.SendOrderEmailAsync(model);

            TempData["OrderSuccess"] = "Заявката беше изпратена успешно.";
            return RedirectToAction("Index", "Home");
        }
    }
}



