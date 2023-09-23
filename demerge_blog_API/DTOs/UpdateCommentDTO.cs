using System.ComponentModel.DataAnnotations;

namespace demerge_blog_API.DTOs
{
    public class UpdateCommentDTO
    {
        [Required]
        public string? CommentText { get; set; }
    }
}
