
using System.Linq.Expressions;
using Catalog.API.Domain.Common;

namespace Catalog.API.Interfaces
{
    public interface IRepositoryBase<TEntity, TKey>
        where TEntity: AuditableEntity<TKey>
        where TKey: notnull
    {
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TKey id);
        void Delete(TEntity entity);
        Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null,
                                 Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                 string includeProperties = "");
        Task<TEntity?> GetByIdAsync(TKey id);
    }
}