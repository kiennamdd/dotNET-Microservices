
namespace Catalog.API.Models
{
    public class ProductImagesDeleteRequest
    {
        public string ProductId { get; set; } = string.Empty;
        
        public IEnumerable<string>? ProductImageIds { get; set; }
    }
}