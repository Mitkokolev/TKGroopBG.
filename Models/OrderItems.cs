namespace TKGroopBG.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";

        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; } = "";

        // Foreign Key
        public int OrderId { get; set; }
        public Orders Order { get; set; } = null!;
    }
}

