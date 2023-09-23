using demerge_blog_API.Data;
using demerge_blog_API.Models;
using demerge_blog_API.Repository.Base;
using Microsoft.EntityFrameworkCore;

namespace demerge_blog_API.Repository
{
    public class CommentRepo : MainRepository<Comment>, ICommentRepo
    {
        public CommentRepo(AppDbContext db) : base(db)
        {
            _db = db;
        }
        private readonly AppDbContext _db;
        public async Task<IEnumerable<CommentView>> GetAllComments()
        {
            return await _db.CommentViews.ToListAsync();
        }

        public async Task<CommentDetails> GetCommentDetails(int id)
        {
            return await _db.CommentDetails.FromSqlInterpolated($"select * from GetCommentDetails({id})").FirstOrDefaultAsync();
        }
    }
}
