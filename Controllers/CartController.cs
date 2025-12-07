using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TKGroopBG.Models;
using TKGroopBG.Services;

namespace TKGroopBG.Controllers
{
    public class CartController : Controller
    {
        private readonly IEmailService _emailService;

        public CartController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        // GET: /Cart
        [HttpGet]
        public IActionResult Index()
        {
            // прост view – количката се рисува с JS от localStorage
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
                ModelState.AddModelError(string.Empty, "Количката е празна.");
            }

            if (!ModelState.IsValid)
            {
                return View("Order", model);
            }

            await _emailService.SendOrderEmailAsync(model);

            TempData["OrderSuccess"] = "Заявката беше изпратена успешно.";
            return RedirectToAction("Index", "Home");
        }
    }
}


