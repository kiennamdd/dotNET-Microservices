
namespace Catalog.API.Models
{
    public class ProductImageDto
    {
        public Guid Id { get; set; }
        public string ImageName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}