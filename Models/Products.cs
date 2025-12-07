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
    }
}


