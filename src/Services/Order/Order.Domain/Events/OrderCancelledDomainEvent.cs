
using Order.Domain.Common;
using Order.Domain.Entities;

namespace Order.Domain.Events
{
    public class OrderCancelledDomainEvent: DomainEvent
    {
        public Guid OrderId { get; private set; }
        public IEnumerable<OrderItem> Items { get; private set; }

        public OrderCancelledDomainEvent(Guid orderId, IEnumerable<OrderItem> items)
        {
            OrderId = orderId;
            Items = items;
        }             
    }
}