using demerge_blog_API.Models;

namespace demerge_blog_API.Repository.Base
{
    public interface ICommentRepo : IRepository<Comment>
    {
        Task<IEnumerable<CommentView>> GetAllComments();

        Task<CommentDetails> GetCommentDetails(int id);
    }
}
