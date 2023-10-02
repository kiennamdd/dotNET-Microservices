
using System.ComponentModel.DataAnnotations;

namespace Catalog.API.Models
{
    public class ProductCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Description { get; set; } = string.Empty;

        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AppliedCouponCode { get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;

        public IFormFile? Thumbnail { get; set; }
        public virtual IEnumerable<IFormFile>? Images { get; set; }
    }
}