
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Order.Domain.Common
{
    public abstract class Entity<TKey>: IEntity
        where TKey: notnull
    {
        public TKey Id { get; protected set; }

        [NotMapped]
        private List<DomainEvent> _domainEvents;

        [NotMapped]
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public Entity()
        {
            Id = default!;
            _domainEvents = new List<DomainEvent>();
        }

        public Entity(TKey id)
        {
            Id = id;
            _domainEvents = new List<DomainEvent>();
        }

        public void AddDomainEvent(DomainEvent @event)
        {
            _domainEvents.Add(@event);
        }

        public void RemoveDomainEvent(DomainEvent @event)
        {
            _domainEvents.Remove(@event);
        }

        public void ClearDomainEvent()
        {
            _domainEvents.Clear();
        }
    }
}