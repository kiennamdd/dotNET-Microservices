
namespace Catalog.API.Models
{
    public class ProductDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;

        public string AppliedCouponCode { get; set; } = string.Empty;
        public double DiscountAmount { get;set; }
        public double DiscountPercent { get;set; }

        public string ThumbnailUrl { get; set; } = string.Empty;

        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;

        public IEnumerable<ProductImageDto>? ProductImages { get; set; } = null;
    }
}