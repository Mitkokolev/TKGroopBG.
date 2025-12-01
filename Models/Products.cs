using System.ComponentModel.DataAnnotations;

namespace TKGroopBG.Models
{
    public class Products
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        public decimal Price { get; set; }

        [Required]
        [MaxLength(200)]
        public string Category { get; set; } 
    }
}


