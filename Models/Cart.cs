using System.ComponentModel.DataAnnotations;

namespace TKGroopBG.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        // Това е ключът към потребителя - използваме Email или UserId
        [Required]
        public string UserEmail { get; set; }

        // Връзка към продукта
        [Required]
        public int ProductId { get; set; }
        public Products Product { get; set; } // Навигационно свойство

        [Required]
        public int Quantity { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}