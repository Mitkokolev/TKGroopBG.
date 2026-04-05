using System;
using System.Collections.Generic;
using System.Linq;
<<<<<<< HEAD
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
=======
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
>>>>>>> 50a922d206104454f183829c34a980899f24736a
using TKGroopBG.Data;
using TKGroopBG.Models;
using TKGroopBG.Services;

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
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Order([FromBody] OrderRequest request)
        {
            if (request == null || request.Items == null || !request.Items.Any())
                return BadRequest(new { message = "Количката е празна!" });

            try
            {
<<<<<<< HEAD
                var order = new Orders
                {
                    CustomerName = request.CustomerName ?? $"{request.FirstName} {request.LastName}",
                    FirstName = request.FirstName ?? "",
                    LastName = request.LastName ?? "",
                    Email = request.Email ?? "",
                    Phone = request.Phone ?? "",
                    Address = request.Address ?? "",
                    City = request.City ?? "",
                    Note = request.Note ?? request.Comment,
                    OrderDate = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    Status = "Нова",
                    TotalPrice = request.Items.Sum(i => i.Price)
                };
=======
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
                Status = "Нова",

                // ВАЖНО: Записваме кой прави поръчката, за да излиза в "Моите поръчки"
                CustomerEmail = User.Identity?.IsAuthenticated == true ? User.Identity.Name : model.Email
            };

            // 3. Добавяме артикулите
            foreach (var item in items)
            {
                order.Items.Add(new OrderItem
                {
                    Name = item.Name,
                    Price = item.Price,
                    Quantity = item.Qty,

                    // ЗАЩИТА: Ако няма снимка от JS количката, слагаме празен текст, за да не гърми БД
                    Image = item.Image ?? ""
                });
>>>>>>> 50a922d206104454f183829c34a980899f24736a

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in request.Items)
                {
                    _context.OrderItems.Add(new OrderItems
                    {
                        OrderId = order.Id,
                        ProductName = item.ProductName ?? "Продукт",
                        Quantity = 1,
                        Price = item.Price,
                        Image = item.Image ?? "default.jpg",
                        ConfigurationDetails = item.Details ?? ""
                    });
                }

                await _context.SaveChangesAsync();

                try { await _emailService.SendOrderEmailAsync(request); } catch { }

                return Ok(new { success = true, orderId = order.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Грешка при запис: " + ex.Message });
            }
        }
    }
}