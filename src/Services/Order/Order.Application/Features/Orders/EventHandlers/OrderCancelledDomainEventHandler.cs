using EventBus.Events;
using MassTransit;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;
using Order.Domain.Events;

namespace Order.Application.Features.Orders.EventHandlers
{
    public class OrderCancelledDomainEventHandler : INotificationHandler<OrderCancelledDomainEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IOrderRepository _orderRepository;

        public OrderCancelledDomainEventHandler(IPublishEndpoint publishEndpoint, IBuyerRepository buyerRepository, IOrderRepository orderRepository)
        {
            _publishEndpoint = publishEndpoint;    
            _buyerRepository = buyerRepository;
            _orderRepository = orderRepository;
        }

        public async Task Handle(OrderCancelledDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            CustomerOrder? order = await _orderRepository.GetByIdAsync(domainEvent.OrderId);
            if(order is null)
                throw new KeyNotFoundException(nameof(domainEvent.OrderId));

            Buyer? buyer = await _buyerRepository.GetByIdAsync(order.BuyerId ?? 0);
            if(buyer is null)
                throw new KeyNotFoundException($"Invalid buyer identifier of order with ID: {domainEvent.OrderId}");

            await _publishEndpoint.Publish(new OrderCancelledIntegrationEvent(buyer.UserId, order.Id, buyer.UserEmail));
        }
    }
}