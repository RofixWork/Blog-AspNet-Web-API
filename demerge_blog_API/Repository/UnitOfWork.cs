using demerge_blog_API.Data;
using demerge_blog_API.Models;
using demerge_blog_API.Repository.Base;

namespace demerge_blog_API.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;

        public UnitOfWork(AppDbContext db)
        {
            _db = db;
            Categories = new MainRepository<Category>(db);
            Posts = new MainRepository<Post>(db);
            Comments = new CommentRepo(db);
        }

        public IRepository<Category> Categories { get; private set; }

        public IRepository<Post> Posts { get; private set; }

        public ICommentRepo Comments { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
