using Order.Domain.Common;
using Order.Domain.Entities;

namespace Order.Domain.Events
{
    public class OrderInformationVerifiedDomainEvent: DomainEvent
    {
        public Guid OrderId { get; private set; }
        public Buyer Buyer { get; private set; }
        public Address Address { get; private set; }

        public OrderInformationVerifiedDomainEvent(Guid orderId, Buyer buyer, Address address)
        {
            OrderId = orderId;
            Buyer = buyer;
            Address = address;
        }
    }
}