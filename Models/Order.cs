using System;
using System.Collections.Generic; // Задължително добави това
using System.ComponentModel.DataAnnotations;

namespace TKGroopBG.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? Address { get; set; }
        public string? Comment { get; set; }
        public string CartJson { get; set; } = "";
        public decimal TotalPrice { get; set; }

        public string CustomerName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
        public string Status { get; set; } = "Нова";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // ДОБАВИ ТОЗИ РЕД:
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}

