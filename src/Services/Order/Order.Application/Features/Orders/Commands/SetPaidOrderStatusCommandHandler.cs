
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;
using Order.Domain.Events;

namespace Order.Application.Features.Orders.Commands
{
    public class SetPaidOrderStatusCommandHandler : IRequestHandler<SetPaidOrderStatusCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBuyerRepository _buyerRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SetPaidOrderStatusCommandHandler(IOrderRepository orderRepository, IBuyerRepository buyerRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;    
            _unitOfWork = unitOfWork;
            _buyerRepository = buyerRepository;
        }

        public async Task<bool> Handle(SetPaidOrderStatusCommand request, CancellationToken cancellationToken)
        {
            CustomerOrder? order = await _orderRepository.GetByIdAsync(request.OrderId);

            if(order is null || order.BuyerId is null)
                return false;

            order.SetStatusToPaid();
            order.SetStripePaymentIntentId(request.StripePaymentIntentId);
            _orderRepository.Update(order);

            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}