using MediatR;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;

namespace Order.Application.Features.Orders.Commands
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var address = new Address(request.Street, request.City, request.State, request.Country, request.ZipCode);

            var order = new CustomerOrder(request.UserId, request.UserName, address, 
                                            request.AppliedCouponCode, request.DiscountAmount, request.DiscountPercent,
                                            request.OrderTotal);

            foreach(var dto in request.OrderItemDtos)
            {
                var orderItem = new OrderItem(dto.Quantity, dto.ProductId, dto.ProductName, dto.ProductOriginalPrice, dto.ProductLastPrice, dto.ProductThumbnailFileName);
                order.AddOrderItem(orderItem);
            }

            _orderRepository.Add(order);

            await _unitOfWork.SaveChangesAsync();

            return order.Id;
        }
    }
}