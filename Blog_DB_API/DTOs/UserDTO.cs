using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Blog_DB_API.DTOs
{
    public class UserDTO
    {
        [Required, RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Email Adress is Invalid"), DefaultValue("jhon@gmail.com")]
        public string? Email { get; set; }
        [Required, MinLength(6), DefaultValue("123456")]
        public string? Password { get; set; }
    }
}
