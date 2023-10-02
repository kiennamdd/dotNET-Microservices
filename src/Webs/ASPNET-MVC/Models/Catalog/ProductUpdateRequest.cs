
using System.ComponentModel.DataAnnotations;

namespace ASPNET_MVC.Models.Catalog
{
    public class ProductUpdateRequest
    {
        [Required]
        public string Id { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public double Price { get; set; }

        [Required]
        public string AppliedCouponCode { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        public string Origin { get; set; } = string.Empty;

        [Required]
        public string CategoryName { get; set; } = string.Empty;
        
        [Required]
        public string BrandName { get; set; } = string.Empty;
    }
}