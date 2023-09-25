
using System.Linq.Expressions;
using Order.Domain.Common;

namespace Order.Application.Common.Interfaces
{
    public interface IRepositoryBase<TEntity, TKey>
        where TKey: notnull
        where TEntity: Entity<TKey>
    {
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null,
                                                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                                string includeProperties = "");
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TKey id);
        void Delete(TEntity entity);
    }
}