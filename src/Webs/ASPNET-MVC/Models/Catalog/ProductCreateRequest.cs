
using System.ComponentModel.DataAnnotations;
using ASPNET_MVC.Attributes;
using ASPNET_MVC.Enums;

namespace ASPNET_MVC.Models.Catalog
{
    public class ProductCreateRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public double Price { get; set; }


        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Description { get; set; } = string.Empty;


        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string AppliedCouponCode { get; set; } = string.Empty;

        [Required]
        public string Origin { get; set; } = string.Empty;

        [Required]
        public string CategoryName { get; set; } = string.Empty;
        
        [Required]
        public string BrandName { get; set; } = string.Empty;

        [Required]
        [MaxFileSize(2)]
        [AllowedFileExtensions(FileType.IMAGE)]
        public IFormFile? Thumbnail { get; set; }
        
        [Required]
        [MaxFileSizeList(2)]
        [AllowedFileExtensionsList(FileType.IMAGE)]
        public virtual IEnumerable<IFormFile>? Images { get; set; }
    }
}