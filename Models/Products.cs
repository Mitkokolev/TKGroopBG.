using System.ComponentModel.DataAnnotations;

namespace TKGroopBG.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Category { get; set; }
        public string? ImageFileName { get; set; }

        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public virtual ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
        public virtual ICollection<ProductColor> Colors { get; set; } = new List<ProductColor>();
    }
    public class ProductImage
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public int ProductId { get; set; }
        public virtual Products Product { get; set; } // Връзка към основния модел Products
        public string? ColorHex { get; set; } // За да знаем тази снимка за кой цвят е
    }

    public class ProductVariant
    {
        public int Id { get; set; }
        public string Name { get; set; } // Напр. "Троен стъклопакет"
        public decimal AdditionalPrice { get; set; }
        public int ProductId { get; set; }
        public virtual Products Product { get; set; }
    }

    public class ProductColor
    {
        public int Id { get; set; }
        public string ColorName { get; set; }
        public string ColorHex { get; set; }
        public int ProductId { get; set; }
        public virtual Products Product { get; set; }
    }
}


