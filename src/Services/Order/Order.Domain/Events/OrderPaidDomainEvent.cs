
using Order.Domain.Common;
using Order.Domain.Entities;

namespace Order.Domain.Events
{
    public class OrderPaidDomainEvent: DomainEvent
    {
        public Guid OrderId { get; private set; }
        public IEnumerable<OrderItem> Items { get; private set; }

        public OrderPaidDomainEvent(Guid orderId, IEnumerable<OrderItem> items)
        {
            OrderId = orderId;
            Items = items;
        }
    }
}