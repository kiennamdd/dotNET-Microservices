using EventBus.Events;
using MassTransit;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;
using Order.Domain.Events;

namespace Order.Application.Features.Orders.EventHandlers
{
    public class OrderStartedDomainEventHandler : INotificationHandler<OrderStartedDomainEvent>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderStartedDomainEventHandler(IPublishEndpoint publishEndpoint, IBuyerRepository buyerRepository, IUnitOfWork unitOfWork)
        {
            _publishEndpoint = publishEndpoint;
            _buyerRepository = buyerRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(OrderStartedDomainEvent domainEvent, CancellationToken cancellationToken)
        {
            string[] includeProperties = {
                nameof(Buyer.Addresses)
            };

            var list = await _buyerRepository.GetListAsync(o => o.UserId == domainEvent.UserId && o.UserEmail == domainEvent.UserName,
                                                            includeProperties: string.Join(",", includeProperties));

            // if list has more than 2 item, throw exception
            Buyer? buyer = list.SingleOrDefault();
            bool isBuyerExisted = buyer is not null;

            // if buyer is null, create new one
            if(buyer is null)
            {
                buyer = new Buyer(domainEvent.UserId, domainEvent.UserName);
            }

            Address verifiedAddress = buyer.VerifyOrAddAddress(domainEvent.Address);
            //PaymentMethod verifiedPaymentMethod = buyer.VerifyOrAddPaymentMethod(domainEvent.PaymentMethod);

            if(isBuyerExisted)
                _buyerRepository.Update(buyer);
            else
                _buyerRepository.Add(buyer);

            buyer.AddDomainEvent(new OrderInformationVerifiedDomainEvent(domainEvent.Order.Id, buyer, verifiedAddress));
            await _unitOfWork.SaveChangesAsync();

            // Publish integration event
            await _publishEndpoint.Publish(new OrderStartedIntegrationEvent(domainEvent.UserId, domainEvent.Order.Id));
        }
    }
}