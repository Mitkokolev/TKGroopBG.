using System.ComponentModel.DataAnnotations;

namespace TKGroopBG.Models
{
    public class Gallery
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Заглавието е задължително")]
        [StringLength(100)]
        public string? Title { get; set; }

        [StringLength(2000)] // Увеличих го, за да имаш място за материалите
        public string? Description { get; set; }

        // Това ще бъде "корицата" на албума в главния списък
        [StringLength(260)]
        public string? ImageFileName { get; set; }

        // НОВО: Списък от всички снимки, принадлежащи към този проект
        public List<GalleryImage> Images { get; set; } = new List<GalleryImage>();
    }

    // Нов модел за допълнителните снимки
    public class GalleryImage
    {
        public int Id { get; set; }

        [StringLength(260)]
        public string FileName { get; set; } = "";

        // Връзка към основната галерия
        public int GalleryId { get; set; }
        public Gallery? Gallery { get; set; }
    }
}

