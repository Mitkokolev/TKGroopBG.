using System;
using System.Collections.Generic;

namespace TKGroopBG.Models
{
    public class Order
    {
        public int Id { get; set; }

        public string CustomerName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";

        public string? Address { get; set; }
        public string? Comment { get; set; }

        public string CartJson { get; set; } = "";

        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}


