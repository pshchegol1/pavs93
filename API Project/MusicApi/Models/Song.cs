
using System.ComponentModel.DataAnnotations;

namespace MusicApi.Models
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Duration { get; set; }

        public DateTime UploadedDate { get; set; }

        public bool IsFeatured { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public IFormFile Image { get; set; }

        public string ImageUrl { get; set; }

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public IFormFile AudioFile { get; set; }

        public string AudioUrl { get; set; }

        public int ArtistId { get; set; }

        public int? AlbumId { get; set; }
    }
}
