
namespace Catalog.API.Models
{
    public class ProductImagesAddRequest
    {
        public string ProductId { get; set; } = string.Empty;
        
        public IEnumerable<IFormFile>? Images { get; set; } = null;
    }
}