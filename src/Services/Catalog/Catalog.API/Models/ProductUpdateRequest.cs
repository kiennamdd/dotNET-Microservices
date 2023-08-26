
namespace Catalog.API.Models
{
    public class ProductUpdateRequest
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string AppliedCouponCode { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
    }
}