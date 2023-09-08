
using EventBus.Common;

namespace EventBus.Events
{
    public class ProductDeletedEvent: IntegrationEventBase
    {
        public Guid ProductId { get; set; }
    }
}