namespace demerge_blog_API.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string? CommentText { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
