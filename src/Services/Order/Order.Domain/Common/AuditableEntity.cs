
namespace Order.Domain.Common
{
    public abstract class AuditableEntity<TKey>: Entity<TKey>, IAuditableEntity
        where TKey: notnull
    {
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string ModifiedBy { get; set; }

        public AuditableEntity()
        {
            DateTime now = DateTime.Now;
            
            CreatedAt = now;
            ModifiedAt = now;

            CreatedBy = string.Empty;
            ModifiedBy = string.Empty; 
        }

        public AuditableEntity(TKey id)
            : base(id) 
        {
            DateTime now = DateTime.Now;
            
            CreatedAt = now;
            ModifiedAt = now;

            CreatedBy = string.Empty;
            ModifiedBy = string.Empty; 
        }
    }
}