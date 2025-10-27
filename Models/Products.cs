using System.ComponentModel.DataAnnotations;

namespace TKGroopBG.Models
{
    public class Products
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Range(0, 100000)]
        public decimal Price { get; set; }

        [MaxLength(50)]
        public string? Category { get; set; }
    }
}

