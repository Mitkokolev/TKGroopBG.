using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TKGroopBG.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        public string? Comment { get; set; }

        public decimal TotalPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Нова";
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}