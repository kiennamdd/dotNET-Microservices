using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Order.Application.Common.Interfaces;
using Order.Domain.Common;

namespace Order.Infrastructure.Data.Interceptors
{
    public class AuditableEntityInterceptor: SaveChangesInterceptor
    {
        private readonly ICurrentUser _currentUser;

        public AuditableEntityInterceptor(ICurrentUser currentUser)
        {
            _currentUser = currentUser;    
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public void UpdateEntities(DbContext? context)
        {
            if(context is null)
                return;

            foreach(var entry in context.ChangeTracker.Entries<IAuditableEntity>())
            {
                var now = DateTime.Now;
                Guid userId = _currentUser.GetUserId();
                var user = userId != Guid.Empty ? userId.ToString() : "system";

                if(entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    entry.Entity.CreatedBy = user;
                }

                if(entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    entry.Entity.ModifiedAt = now;
                    entry.Entity.ModifiedBy = user;
                }
            }
        }
    }
}