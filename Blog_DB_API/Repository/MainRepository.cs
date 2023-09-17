using Blog_DB_API.Data;
using Blog_DB_API.Repository.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Blog_DB_API.Repository
{
    public class MainRepository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _db;

        public MainRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddOne(T entity)
        {
            await _db.Set<T>().AddAsync(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> FindAll(Expression<Func<T, DateTime>> expression)
        {
            return await _db.Set<T>().OrderByDescending(expression).ToListAsync();
        }

        public async Task<T> FindOne(Expression<Func<T, bool>> expression)
        {
            return await _db.Set<T>().FirstOrDefaultAsync(expression);
        }

        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}
