
using System.Linq.Expressions;
using Cart.API.Data;
using Cart.API.Domain.Common;
using Cart.API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cart.API.Repositories
{
    public class RepositoryBase<TEntity, TKey> : IRepositoryBase<TEntity, TKey>
        where TEntity: EntityBase<TKey>
        where TKey: notnull
    {
        protected readonly ApplicationDbContext _db;
        protected readonly DbSet<TEntity> _dbSet;

        public RepositoryBase(ApplicationDbContext db)
        {
            _db = db;
            _dbSet = db.Set<TEntity>();    
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Delete(TKey id)
        {
            TEntity? entity = _dbSet.FirstOrDefault(o => o.Id.Equals(id));

            if(entity is not null)
            {
                _dbSet.Remove(entity);
            }
        }

        public void Delete(TEntity entity)
        {
            if(_db.Entry(entity).State == EntityState.Detached)
            {
                _db.Attach(entity);
            }

            _dbSet.Remove(entity);
        }

        public async Task<TEntity?> GetByIdAsync(TKey id)
        {
            TEntity? entity = await _dbSet.FirstOrDefaultAsync(o => o.Id.Equals(id));
            return entity;
        }

        public async Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>>? predicate = null, 
                                        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, 
                                        string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet.AsQueryable();

            if(predicate is not null)
            {
                query = query.Where(predicate);
            }

            if(!string.IsNullOrEmpty(includeProperties))
            {
                string[] props = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach(string prop in props)
                {
                    query.Include(prop);
                }
            }

            if(orderBy is not null)
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