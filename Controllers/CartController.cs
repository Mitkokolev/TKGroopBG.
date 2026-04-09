using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TKGroopBG.Data;
using TKGroopBG.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TKGroopBG.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize] // Само регистрирани потребители могат да поръчват
        public async Task<IActionResult> Order([FromBody] OrderRequest request)
        {
            // 1. Проверка за празна количка
            if (request == null || request.Items == null || !request.Items.Any())
            {
                return BadRequest(new { success = false, message = "Количката е празна!" });
            }

            // 2. Взимаме имейла на текущо логнатия потребител
            var userEmail = User.Identity?.Name;

            try
            {
                // 3. Създаваме основната поръчка
                var order = new Orders
                {
                    Email = userEmail, // Автоматично от профила
                    FirstName = request.FirstName ?? "",
                    LastName = request.LastName ?? "",
                    CustomerName = $"{request.FirstName} {request.LastName}".Trim(),
                    Phone = request.Phone ?? "",
                    Address = request.Address ?? "",
                    City = request.City ?? "",
                    Comment = request.Comment ?? "",
                    CartJson = "[]", // Можеш да сериализираш request.Items тук, ако ти трябва архив
                    TotalPrice = request.Items.Sum(i => i.Price * i.Quantity),
                    OrderDate = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    Status = "Нова"
                };

                // 4. Добавяме всеки продукт към поръчката
                foreach (var item in request.Items)
                {
                    order.OrderItems.Add(new OrderItems
                    {
                        ProductName = item.ProductName,
                        Price = item.Price,
                        Quantity = item.Quantity > 0 ? item.Quantity : 1,
                        Image = item.Image,
                        ConfigurationDetails = "" // Тук можеш да добавиш детайли от конфигуратора
                    });
                }

                // 5. Запис в базата данни
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Поръчката е приета успешно!" });
            }
            catch (Exception ex)
            {
                // Логване на грешката, ако нещо се счупи при записа
                return StatusCode(500, new { success = false, message = "Грешка при обработка на поръчката: " + ex.Message });
            }
        }
    }

    // Помощни класове (DTO) за приемане на JSON от JavaScript
    public class OrderRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Comment { get; set; }
        public List<CartItemRequest> Items { get; set; }
    }

    public class CartItemRequest
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; }
    }
}