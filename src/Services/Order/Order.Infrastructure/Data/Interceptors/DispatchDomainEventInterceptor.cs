using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Order.Domain.Common;

namespace Order.Infrastructure.Data.Interceptors
{
    public class DispatchDomainEventInterceptor: SaveChangesInterceptor
    {
        private readonly IMediator _mediator;

        public DispatchDomainEventInterceptor(IMediator mediator)
        {
            _mediator = mediator;    
        }

        public override int SavedChanges(SaveChangesCompletedEventData eventData, int result)
        {
            DispatchDomainEvent(eventData.Context).GetAwaiter().GetResult();
            return base.SavedChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            DispatchDomainEvent(eventData.Context).GetAwaiter().GetResult();
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public async Task DispatchDomainEvent(DbContext? dbContext)
        {
            if(dbContext is null)
                return;

            var entities = dbContext.ChangeTracker.Entries<IEntity>().Where(o => o.Entity.DomainEvents.Any()).Select(o => o.Entity);

            List<DomainEvent> domainEvents = entities.SelectMany(o => o.DomainEvents).ToList();
            
            foreach(var entity in entities)
            {
                entity.ClearDomainEvent();
            }

            foreach(DomainEvent domainEvent in domainEvents)
            {
                await _mediator.Publish(domainEvent);
            }
        }
    }
}