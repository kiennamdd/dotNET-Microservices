
using Order.Domain.Common;
using Order.Domain.Entities;

namespace Order.Domain.Events
{
    public class OrderRefundedDomainEvent: DomainEvent
    {
        public Guid OrderId { get; private set; }
        public double OrderTotal { get; set; }
        public IEnumerable<OrderItem> Items { get; private set; }

        public OrderRefundedDomainEvent(Guid orderId, double orderTotal, IEnumerable<OrderItem> items)
        {
            OrderId = orderId;
            OrderTotal = orderTotal;
            Items = items;
        }
    }
}