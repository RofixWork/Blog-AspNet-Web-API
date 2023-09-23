using demerge_blog_API.Models;
using System.Linq.Expressions;

namespace demerge_blog_API.Repository.Base
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> FindAll(params string[] eagers);

        Task AddOneAsync(T entity);

        Task<T> FindOne(Expression<Func<T, bool>> expression);

        Task DeleteOneAsync(T entity);

        Task SaveChanges();
    }
}
