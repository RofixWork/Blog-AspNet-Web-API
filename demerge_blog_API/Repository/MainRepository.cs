using demerge_blog_API.Data;
using demerge_blog_API.Models;
using demerge_blog_API.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace demerge_blog_API.Repository
{
    public class MainRepository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _db;

        public MainRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddOneAsync(T entity)
        {
            await _db.Set<T>().AddAsync(entity);
            await _db.SaveChangesAsync();
        }


        public async Task DeleteOneAsync(T entity)
        {
            _db.Set<T>().Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> FindAll(params string[] eagers)
        {
            IQueryable<T> query = _db.Set<T>();
            if (eagers.Any())
            {
                foreach (var eager in eagers)
                {
                    query = query.Include(eager);
                }
            }
            return await query.ToListAsync();
        }


        public async Task<T> FindOne(Expression<Func<T, bool>> expression)
        {
            return await _db.Set<T>().FirstOrDefaultAsync(expression);
        }

        public async Task SaveChanges() => await _db.SaveChangesAsync();
    }
}
