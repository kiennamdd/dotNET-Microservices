
using Order.Domain.Common;
using Order.Domain.Entities;
using Order.Domain.Exceptions;

namespace Order.Domain.Events
{
    public class OrderStartedDomainEvent: DomainEvent
    {
        public Guid UserId { get; private set; }
        public string UserName { get; private set; }
        public Address Address { get; private set; }
        public CustomerOrder Order { get; private set; }

        public OrderStartedDomainEvent(Guid userId, string userName, Address address, CustomerOrder order)
        {
            UserId = userId;
            UserName = !string.IsNullOrWhiteSpace(userName) ? userName : throw new OrderDomainException($"{nameof(userName)} value is invalid.");
            Address = address;
            Order = order;
        }        
    }
}