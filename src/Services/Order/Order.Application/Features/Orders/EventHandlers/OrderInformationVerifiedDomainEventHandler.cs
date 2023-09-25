using MediatR;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;
using Order.Domain.Events;

namespace Order.Application.Features.Orders.EventHandlers
{
    public class OrderInformationVerifiedDomainEventHandler : INotificationHandler<OrderInformationVerifiedDomainEvent>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderInformationVerifiedDomainEventHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository; 
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(OrderInformationVerifiedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            CustomerOrder? order = await _orderRepository.GetByIdAsync(domainEvent.OrderId);

            if(order is null)
                throw new KeyNotFoundException(nameof(domainEvent.OrderId));

            order.SetBuyer(domainEvent.Buyer);
            order.SetAddress(domainEvent.Address);

            //this will cause error: Violate foreign key constaint
            //_orderRepository.Update(order);
        }
    }
}