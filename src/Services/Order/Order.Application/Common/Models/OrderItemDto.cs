
using Order.Domain.Entities;

namespace Order.Application.Common.Models
{
    public class OrderItemDto
    {
        public int Quantity { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public double ProductOriginalPrice { get; set; }
        public double ProductLastPrice { get; set; }
        public string ProductThumbnailUrl { get; set; } = string.Empty;
    }

    public static class OrderItemDtoExtension
    {
        public static OrderItemDto ToOrderItemDto(this CartItemDto cartItem)
        {
            return new OrderItemDto
            {
                Quantity = cartItem.Quantity, 
                ProductId = cartItem.ProductId, 
                ProductName = cartItem.ProductName, 
                ProductOriginalPrice = cartItem.ProductOriginalPrice,
                ProductLastPrice = cartItem.ProductLastPrice, 
                ProductThumbnailUrl = "ProductThumbnailUrl"
            };
        }

        public static OrderItemDto ToOrderItemDto(this OrderItem orderItem)
        {
            return new OrderItemDto
            {
                Quantity = orderItem.Quantity, 
                ProductId = orderItem.ProductId, 
                ProductName = orderItem.ProductName, 
                ProductOriginalPrice = orderItem.ProductOriginalPrice,
                ProductLastPrice = orderItem.ProductLastPrice, 
                ProductThumbnailUrl = orderItem.ProductThumbnailUrl
            };
        }

        public static IEnumerable<OrderItemDto> ToOrderItemDtoList(this IEnumerable<CartItemDto> list)
        {
            return list.Select(cartItem => cartItem.ToOrderItemDto());
        }

        public static IEnumerable<OrderItemDto> ToOrderItemDtoList(this IEnumerable<OrderItem> list)
        {
            return list.Select(orderItem => orderItem.ToOrderItemDto());
        }
    }
}