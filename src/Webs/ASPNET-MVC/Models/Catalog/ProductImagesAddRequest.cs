
using System.ComponentModel.DataAnnotations;
using ASPNET_MVC.Attributes;
using ASPNET_MVC.Enums;

namespace ASPNET_MVC.Models.Catalog
{
    public class ProductImagesAddRequest
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;
        
        [Required]
        [MaxFileSizeList(2)]
        [AllowedFileExtensionsList(FileType.IMAGE)]
        public IEnumerable<IFormFile>? Images { get; set; } = null;
    }
}