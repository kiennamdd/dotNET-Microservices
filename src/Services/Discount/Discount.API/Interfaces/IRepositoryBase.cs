using Discount.API.Domain.Common;
using System.Linq.Expressions;

namespace Discount.API.Interfaces
{
    public interface IRepositoryBase<T, Key>
        where T: AuditableEntity<Key>
        where Key: notnull
    {
        Task<Key> AddAsync(T entity);
        Task<bool> DeleteAsync(Key id);
        Task<IEnumerable<T>> GetListAsync();
        Task<IEnumerable<T>> GetFilteredListAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetByIdAsync(Key id);
    }
}
