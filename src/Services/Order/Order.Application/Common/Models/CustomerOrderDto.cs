using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Application.Common.Models
{
    public class CustomerOrderDto
    {
        public Guid Id { get; set; }
        public AddressDto? Address { get; set; }
        public double OrderTotal { get; set; }
        public DateTime OrderDate { get; set; } 
        public DateTime PaidDate { get; set; } 
        public OrderStatus Status { get; set; } = OrderStatus.Unknown;
        public string Description { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public IEnumerable<OrderItemDto> Items { get; set;} = new List<OrderItemDto>();
    }

    public static class CustomerOrderDtoExtension
    {
        public static CustomerOrderDto ToOrderDto(this CustomerOrder order)
        {
            return new CustomerOrderDto
            {
                Id = order.Id,
                Address = order.Address?.ToAddressDto(),
                OrderTotal = order.OrderTotal,
                OrderDate = order.OrderDate,
                PaidDate = order.PaidDate,
                Status = order.Status,
                Description = order.Description,
                IsPaid = order.IsPaid,
                Items = order.Items.ToOrderItemDtoList()
            };
        }

        public static IEnumerable<CustomerOrderDto> ToOrderDtoList(this IEnumerable<CustomerOrder> list)
        {
            return list.Select(order => order.ToOrderDto());
        }
    }
}