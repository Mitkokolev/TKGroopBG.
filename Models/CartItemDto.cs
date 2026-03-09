namespace TKGroopBG.Models
{
    public class CartItemDto
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Qty { get; set; }

        public string? Image { get; set; }
    }
}



