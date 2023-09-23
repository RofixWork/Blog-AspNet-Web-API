using System.ComponentModel.DataAnnotations;

namespace demerge_blog_API.DTOs
{
    public record CategoryDTO
    {
        [Required, MaxLength(60)]
        public string Name { get; set; } = null!;
    }
}
