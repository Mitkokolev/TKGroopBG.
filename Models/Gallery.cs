using System.ComponentModel.DataAnnotations;

namespace TKGroopBG.Models
{
    public class Gallery
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; }

        [MaxLength(300)]
        public string? Description { get; set; }
    }
}

