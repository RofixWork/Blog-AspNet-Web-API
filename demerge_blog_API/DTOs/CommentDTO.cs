using System.ComponentModel.DataAnnotations;

namespace demerge_blog_API.DTOs
{
    public class CommentDTO
    {
        [Required]
        public string? CommentText { get; set; }
        [Required]
        public int PostId { get; set; }
        [Required]
        public string? UserId { get; set; }
    }
}
