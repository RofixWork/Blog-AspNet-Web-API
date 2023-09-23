using Microsoft.AspNetCore.Identity;

namespace demerge_blog_API.Models
{
    public class User : IdentityUser
    {
        public ICollection<Post> Posts { get; set; } = new List<Post>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}
