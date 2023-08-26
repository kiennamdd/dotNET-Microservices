using System.Linq.Expressions;
using Catalog.API.Data;
using Catalog.API.Domain.Common;
using Catalog.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Repositories
{
    public class RepositoryBase<TEntity, TKey> : IRepositoryBase<TEntity, TKey>
        where TEntity: AuditableEntity<TKey>
        where TKey: notnull
    {
        protected readonly ApplicationDbContext _db;
        protected readonly DbSet<TEntity> _dbSet;

        public RepositoryBase(ApplicationDbContext db)
        {
            _db = db;
            _dbSet = db.Set<TEntity>();   
        }

        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public void Delete(TKey id)
        {
            TEntity? entity = _dbSet.FirstOrDefault(o => o.Id.Equals(id));

            if(entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public void Delete(TEntity entity)
        {
            if(_db.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            _dbSet.Remove(entity);
        }

        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            return await _dbSet.FirstOrDefaultAsync(o => o.Id.Equals(id));
        }

        public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null,
                                 Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                 string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();

            if(predicate != null)
            {
                query = query.Where(predicate);
            }

            string[] properties = includeProperties.Split(new char[] {','}, StringSplitOptions.RemoveEmptyEntries);
            foreach(string prop in properties)
            {
                query = query.Include(prop);
            }

            if(orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }
    }
}