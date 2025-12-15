namespace TKGroopBG.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }

        public string VariantColor { get; set; }
        public int WidthMm { get; set; }
        public int HeightMm { get; set; }
        public double AreaM2 { get; set; }
        public decimal Price { get; set; }

        // Foreign Key
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}

