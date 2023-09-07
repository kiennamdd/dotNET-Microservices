
using System.Linq.Expressions;
using Cart.API.Domain.Common;

namespace Cart.API.Interfaces
{
    public interface IRepositoryBase<TEntity, TKey>
        where TEntity: EntityBase<TKey>
        where TKey: notnull
    {
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null,
                                                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                                string includeProperties = "");
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TKey id);
        void Delete(TEntity entity);
    }
}