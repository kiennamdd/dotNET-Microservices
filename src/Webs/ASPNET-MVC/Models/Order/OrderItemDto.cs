namespace ASPNET_MVC.Models.Order
{
    public class OrderItemDto
    {
        public int Quantity { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public double ProductOriginalPrice { get; set; }
        public double ProductLastPrice { get; set; }
        public string ProductThumbnailFileName { get; set; } = string.Empty;
        public string ProductThumbnailUrl{ get; set; } = string.Empty;
    }
}