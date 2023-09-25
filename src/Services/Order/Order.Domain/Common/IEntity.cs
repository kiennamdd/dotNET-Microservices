namespace Order.Domain.Common
{
    public interface IEntity
    {
        public IReadOnlyCollection<DomainEvent> DomainEvents { get; }
        void AddDomainEvent(DomainEvent @event);
        void RemoveDomainEvent(DomainEvent @event);
        void ClearDomainEvent();
    }
}