namespace TKGroopBG.Models
{
    public class CartItemDto
    {
        public int productId { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public decimal basePrice { get; set; }

        public int heightMm { get; set; }
        public int widthMm { get; set; }
        public double areaM2 { get; set; }

        public string variantColor { get; set; }
        public decimal totalPrice { get; set; }
    }
}

