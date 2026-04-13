using System.ComponentModel.DataAnnotations;

namespace TKGroopBG.Models
{
    public class Products
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string? Category { get; set; }

        public string? ImageFileName { get; set; }

        // --- НОВИ ПОЛЯ ЗА ФУНКЦИОНАЛНОСТТА ---

        public bool IsCustomSize { get; set; } = false;
        public int StockQuantity { get; set; } = 1;

        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public virtual ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public virtual ICollection<ProductColor> Colors { get; set; } = new List<ProductColor>();
    }

    public class ProductImage
    {
        public int Id { get; set; }
        public string ImagePath { get; set; } = null!;
        public int ProductId { get; set; }
        public virtual Products Product { get; set; } = null!;
        public string? ColorHex { get; set; }
    }

    public class ProductVariant
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal AdditionalPrice { get; set; }
        public int ProductId { get; set; }
        public virtual Products Product { get; set; } = null!;
    }

    public class ProductColor
    {
        public int Id { get; set; }
        public string ColorName { get; set; } = null!;
        public string ColorHex { get; set; } = null!;
        public int ProductId { get; set; }
        public virtual Products Product { get; set; } = null!;
    }
}