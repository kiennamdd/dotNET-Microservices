using Discount.API.Domain.Common;
using Discount.API.Interfaces;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Discount.API.Repositories
{
    public class RepositoryBase<T, Key> : IRepositoryBase<T, Key>
        where T: AuditableEntity<Key>
        where Key: notnull
    {
        protected readonly IMongoCollection<T> _collection;

        public RepositoryBase(IApplicationDbContext db)
        {
            _collection = db.GetCollection<T>();
        }

        public async Task<Key> AddAsync(T entity)
        {
            UpdateAuditInfo(entity);

            await _collection.InsertOneAsync(entity);

            return entity.Id;
        }

        public async Task<bool> DeleteAsync(Key id)
        {
            var result = await _collection.DeleteOneAsync(o => o.Id.Equals(id));
            return result.IsAcknowledged;
        }

        public async Task<T?> GetByIdAsync(Key id)
        {
            var entity = await _collection.Find(o => o.Id.Equals(id)).FirstOrDefaultAsync();
            return entity;
        }

        public async Task<IEnumerable<T>> GetFilteredListAsync(Expression<Func<T, bool>> predicate)
        {
            var list = await _collection.Find(predicate).ToListAsync();
            return list;
        }

        public async Task<IEnumerable<T>> GetListAsync()
        {
            var list = await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
            return list;
        }

        protected void UpdateAuditInfo(T entity, bool isCreation = false)
        {
            if (entity is null)
                return;

            string userId = "";
            DateTime now = DateTime.Now;

            if (isCreation)
            {
                entity.CreatedAt = now;
                entity.CreatedBy = userId;
            }

            entity.ModifiedAt = now;
            entity.ModifiedBy = userId;
        }
    }
}
