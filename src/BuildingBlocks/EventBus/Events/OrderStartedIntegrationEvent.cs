
using EventBus.Common;

namespace EventBus.Events
{
    public class OrderStartedIntegrationEvent: IntegrationEventBase
    {
        public Guid UserId { get; private set; }
        public Guid OrderId { get; private set; }

        public OrderStartedIntegrationEvent(Guid userId, Guid orderId)
        {
            UserId = userId;
            OrderId = orderId;
        }
    }
}