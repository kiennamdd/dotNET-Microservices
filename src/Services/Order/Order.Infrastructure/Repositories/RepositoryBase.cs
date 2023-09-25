
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Domain.Common;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.Repositories
{
    public class RepositoryBase<TEntity, TKey>: IRepositoryBase<TEntity, TKey>
        where TKey: notnull
        where TEntity: Entity<TKey>
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

            if(entity is null)
            {
                entity = _dbSet.Local.FirstOrDefault(o => o.Id.Equals(id));
            }

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
                    query = query.Include(prop);
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