
using MediatR;
using Order.Application.Common.Models;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Application.Features.Orders.Commands
{
    public class CreateOrderCommand: IRequest<Guid>
    {
        public Guid UserId { get; private set; }
        public string UserName { get; private set; }

        public string? AppliedCouponCode { get; private set; }
        public double DiscountAmount { get; private set; }
        public double DiscountPercent { get; private set; }

        public string Street { get; private set; }
        public string City { get; private set; }
        public string State { get; private set; }
        public string Country { get; private set; }
        public string ZipCode { get; private set; }

        public double OrderTotal { get; private set; }
        public IEnumerable<OrderItemDto> OrderItemDtos;

        public CreateOrderCommand(Guid userId, string userName, 
                string? appliedCouponCode, double discountAmount, double discountPercent, 
                string street, string city, string state, string country, string zipCode, 
                double orderTotal, IEnumerable<OrderItemDto> orderItemDtos)
        {
            UserId = userId;
            UserName = userName;
            AppliedCouponCode = appliedCouponCode;
            DiscountAmount = discountAmount;
            DiscountPercent = discountPercent;
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipCode;
            OrderTotal = orderTotal;
            OrderItemDtos = orderItemDtos;
        }
    }
}