using EventBus.Common;

namespace EventBus.Events
{
    public class UserCreatedIntegrationEvent: IntegrationEventBase
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}