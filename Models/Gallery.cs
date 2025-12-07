using System.ComponentModel.DataAnnotations;

namespace TKGroopBG.Models
{
    public class Gallery
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Title { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        // НОВО – запазваме името на файла
        [StringLength(260)]
        public string? ImageFileName { get; set; }
    }
}

