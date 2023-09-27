
using Catalog.API.Domain.Common;

namespace Catalog.API.Domain.Entities
{
    public class Product: AuditableEntity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        
        public string AppliedCouponCode { get; set; } = string.Empty;
        public double DiscountAmount { get;set; }
        public int DiscountPercent { get;set; }

        public string ThumbnailFileName { get; set; } = string.Empty;
        public string ThumbnailLocalPath { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public int BrandId { get; set; }
        public Brand? Brand { get; set; }

        public bool IsDeleted { get; set; } = false;

        public virtual IEnumerable<ProductImage> ProductImages { get; set; } = default!;
    }
}