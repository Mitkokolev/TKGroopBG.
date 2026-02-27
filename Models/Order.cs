using System;
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

        // ДОБАВИ ТЕЗИ ПОЛЕТА, ЗА ДА ИЗЧЕЗНАТ ГРЕШКИТЕ:
        public string CustomerName { get; set; } = "";

        public string Phone { get; set; } = "";

        public string Email { get; set; } = "";

        public string Status { get; set; } = "Нова";

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}


