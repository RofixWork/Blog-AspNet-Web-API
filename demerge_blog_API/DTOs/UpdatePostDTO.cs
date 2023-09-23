using demerge_blog_API.Utility;
using System.ComponentModel.DataAnnotations;

namespace demerge_blog_API.DTOs
{
    public record UpdatePostDTO
    {
        [Required, MaxLength(255), MinLength(6, ErrorMessage = "Use at least 6 characters in the Title")]
        public string? Title { get; set; }
        [Required, MinLength(20, ErrorMessage = "Use at least 20 characters in the Content")]
        public string? Content { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required, AllowExtensions(new string[] { "image/jpeg", "image/png", "image/avif" })]
        public IFormFile? Image { get; set; }
    }
}
