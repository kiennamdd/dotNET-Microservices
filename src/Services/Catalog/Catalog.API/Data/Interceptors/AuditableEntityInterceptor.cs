using Catalog.API.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Catalog.API.Data.Interceptors
{
    public class AuditableEntityInterceptor: SaveChangesInterceptor
    {
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
                var user = "";

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