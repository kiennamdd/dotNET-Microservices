
using Order.Domain.Common;
using Order.Domain.Exceptions;

namespace Order.Domain.Entities
{
    public class OrderItem: AuditableEntity<int>
    {
        public Guid OrderId { get; private set; }

        public int Quantity { get; private set; }
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; }
        public double ProductOriginalPrice { get; private set; }
        public double ProductLastPrice { get; private set; }
        public string ProductThumbnailUrl { get; private set; }

        // For migration
        public OrderItem() 
        {
            ProductName = string.Empty;
            ProductThumbnailUrl = string.Empty;
        }

        public OrderItem(int quantity, 
                        Guid productId, 
                        string productName, 
                        double productOriginalPrice, 
                        double productLastPrice, 
                        string productThumbnailUrl) 
        {
            OrderId = Guid.Empty;
            Quantity = quantity > 0 ? quantity : throw new OrderDomainException("Quantity value must be positive value.");

            ProductId = productId != Guid.Empty ? productId : throw new OrderDomainException("ProductId can not be empty Guid");

            ProductName = !string.IsNullOrWhiteSpace(productName) 
                                ? productName 
                                : throw new OrderDomainException("ProductName can not be empty.");

            ProductOriginalPrice = productOriginalPrice >= 0 
                                        ? productOriginalPrice 
                                        : throw new OrderDomainException("ProductOriginalPrice can not be negative value.");

            ProductLastPrice = productLastPrice >= 0 
                                        ? productLastPrice 
                                        : throw new OrderDomainException("ProductOriginalPrice can not be negative value.");
                                        
            ProductThumbnailUrl = productThumbnailUrl;
        }

        public void IncreaseQuantity(int quantity)
        {
            if(quantity < 0)
                throw new OrderDomainException("Quantity value can not be negative.");

            Quantity += quantity;
        }
    }
}