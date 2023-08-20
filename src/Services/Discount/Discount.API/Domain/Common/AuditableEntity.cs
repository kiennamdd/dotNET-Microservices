namespace Discount.API.Domain.Common
{
    public abstract class AuditableEntity<Key> : EntityBase<Key>
    {
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
    }
}
