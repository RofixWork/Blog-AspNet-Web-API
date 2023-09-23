using System.ComponentModel.DataAnnotations.Schema;

namespace demerge_blog_API.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public string? UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public byte[]? Image { get; set; }
        [NotMapped]
        public string PostImage
        {
            get
            {
                if (Image == null)
                    return string.Empty;

                var image = Convert.ToBase64String(Image);
                return $"data:image/jpg;base64,{image}";
            }
        }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
