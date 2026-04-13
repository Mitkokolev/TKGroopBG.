using System.Collections.Generic;

namespace TKGroopBG.Models
{
    public class OrderRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Note { get; set; }
        public string? Comment { get; set; }
        public List<CartItemRequest> Items { get; set; } = new List<CartItemRequest>();
    }

    public class CartItemRequest
    {
        public string? ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Image { get; set; }
        public string? Details { get; set; }
    }
}