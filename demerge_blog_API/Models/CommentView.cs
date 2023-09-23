namespace demerge_blog_API.Models
{
    public class CommentView
    {
        public int Id { get; set; }
        public string? CommentText { get; set; }
        public int PostId { get; set; }
        public string UserId { get; set; } = null!;  
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
