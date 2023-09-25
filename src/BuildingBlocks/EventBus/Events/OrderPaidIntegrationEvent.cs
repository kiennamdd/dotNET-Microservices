using EventBus.Common;

namespace EventBus.Events
{
    public class OrderPaidIntegrationEvent: IntegrationEventBase
    {
        public Guid UserId { get; private set; }
        public Guid OrderId { get; private set; }
        public string UserEmail { get; private set; }

        public OrderPaidIntegrationEvent(Guid userId, Guid orderId, string userEmail)
        {
            UserId = userId;
            OrderId = orderId;
            UserEmail = userEmail;
        }
    }
}