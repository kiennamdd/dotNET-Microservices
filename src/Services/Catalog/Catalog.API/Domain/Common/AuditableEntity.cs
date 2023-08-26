

namespace Catalog.API.Domain.Common
{
    public class AuditableEntity<TKey>: EntityBase<TKey>, IAuditableEntity
        where TKey: notnull
    {
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;
    }
}