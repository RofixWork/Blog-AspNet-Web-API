using demerge_blog_API.Models;

namespace demerge_blog_API.Repository.Base
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Category> Categories { get; }
        IRepository<Post> Posts { get; }
        ICommentRepo Comments { get; }
    }
}
