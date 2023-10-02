
using System.ComponentModel.DataAnnotations;

namespace ASPNET_MVC.Models.Catalog
{
    public class ProductImagesDeleteRequest
    {
        [Required]
        public string ProductId { get; set; } = string.Empty;
        
        [Required]
        public IEnumerable<string>? ProductImageIds { get; set; }
    }
}