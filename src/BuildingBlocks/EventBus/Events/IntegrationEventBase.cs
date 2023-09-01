
namespace EventBus.Events
{
    public class IntegrationEventBase
    {
        public IntegrationEventBase()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
        }

        public IntegrationEventBase(Guid id, DateTime createdAt)
        {
            Id = id;
            CreatedAt = createdAt;
        }

        public Guid Id { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}