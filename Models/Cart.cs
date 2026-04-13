using System.ComponentModel.DataAnnotations;

namespace TKGroopBG.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserEmail { get; set; }

        [Required]
        public int ProductId { get; set; }

        public virtual Products Product { get; set; }

        [Required]
        public int Quantity { get; set; }

        public string? Details { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}