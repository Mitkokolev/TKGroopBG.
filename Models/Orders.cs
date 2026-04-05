using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TKGroopBG.Models
{
    // 1. Основният модел за базата данни - оправя грешките от image_62e15e.png
    public class Orders
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = "";
        public string FirstName { get; set; } = ""; // Оправя CS0117
        public string LastName { get; set; } = "";  // Оправя CS0117
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string? Comment { get; set; }         // Оправя CS1061
        public string? Note { get; set; }
        public string CartJson { get; set; } = "";   // Оправя CS1061
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Нова";
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
    }

    public class OrderItems
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Orders? Order { get; set; }
        public string ProductName { get; set; } = "";
        public int Quantity { get; set; } = 1;
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public string? ConfigurationDetails { get; set; }
    }

    // 2. Класът за приемане на поръчката - оправя грешката от image_61fc67.png
    public class OrderRequests
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Comment { get; set; }
        public string? Note { get; set; }
        public List<CartItemRequest> Items { get; set; } = new List<CartItemRequest>();
    }

    public class CartItemRequests
    {
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public string? Details { get; set; }
    }
}