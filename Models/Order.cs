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

        // НОВОТО ПОЛЕ: Използва се за MyOrders страницата
        public string? CustomerEmail { get; set; }

        public string Status { get; set; } = "Нова";
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}