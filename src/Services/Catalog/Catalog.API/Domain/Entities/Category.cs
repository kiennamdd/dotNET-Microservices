
using Catalog.API.Domain.Common;

namespace Catalog.API.Domain.Entities
{
    public class Category: AuditableEntity<int>
    {
        public string Name { get; set; } = string.Empty;

        public virtual IEnumerable<Product> Products { get; set; } = default!;
    }
}