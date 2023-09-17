using System.ComponentModel.DataAnnotations.Schema;

namespace Blog_DB_API.Models
{
    public record Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime PublicationDate { get; set; }
        public byte[]? Image { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
        [NotMapped]
        public string? PostImage
        {
            get
            {
                if (Image == null) return string.Empty;

                var image = Convert.ToBase64String(Image);

                return $"data:image/jpg;base64,{image}";
            }
        }
    }
}
