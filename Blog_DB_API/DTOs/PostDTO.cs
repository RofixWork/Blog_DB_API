using Blog_DB_API.Utitlity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog_DB_API.DTOs
{
    public class PostDTO
    {
        [Required, MaxLength(length:60), MinLength(5)]
        public string? Title { get; set; }
        [Required, MinLength(length:30)]
        public string? Content { get; set; }
        [Required, AllowedExtension(new string[] {"image/jpg", "image/png", "image/jpeg"})]
        public IFormFile? File { get; set; }
    }
}
