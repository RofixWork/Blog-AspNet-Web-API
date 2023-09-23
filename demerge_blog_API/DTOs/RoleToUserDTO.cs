using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace demerge_blog_API.DTOs
{
    public class RoleToUserDTO
    {
        [Required, MaxLength(255), RegularExpression(@"^([a-zA-Z0-9._%-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,})$", ErrorMessage = "Email Address Format is Invalid..."), DefaultValue("jhon@gmail.com")]
        public string? Email { get; set; }
        [Required]
        public string? RoleName { get; set; }
    }
}
