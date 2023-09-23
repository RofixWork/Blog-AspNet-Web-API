using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace demerge_blog_API.DTOs
{
    public class UserDTO
    {
        [Required, MaxLength(255), RegularExpression(@"^([a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})$", ErrorMessage = "Email Address Format is Invalid..."), DefaultValue("jhon@gmail.com")]
        public string? Email { get; set; }
        [Required, MinLength(6), DefaultValue("123456")]
        public string? Password { get; set; }
    }
}
