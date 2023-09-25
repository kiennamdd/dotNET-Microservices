
using EventBus.Common;

namespace EventBus.Events
{
    public class ProductDeletedIntegrationEvent: IntegrationEventBase
    {
        public Guid ProductId { get; set; }
    }
}