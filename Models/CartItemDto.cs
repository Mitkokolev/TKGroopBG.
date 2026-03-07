namespace TKGroopBG.Models
{
    public class CartItemDto
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        // Количеството, избрано от потребителя
        public int Qty { get; set; }

        // Пътят към изображението, за да се показва в "Моите поръчки"
        public string? Image { get; set; }
    }
}

