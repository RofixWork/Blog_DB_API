using System.Linq.Expressions;

namespace Blog_DB_API.Repository.Base
{
    public interface IRepository<T> where T : class
    {
        Task AddOne(T entity);

        Task<IEnumerable<T>> FindAll(Expression<Func<T, DateTime>> expression);

        Task<T> FindOne(Expression<Func<T, bool>> expression);

        Task SaveAsync();
    }
}
