using MediatR;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;

namespace Order.Application.Features.Orders.Commands
{
    public class UpdateStripeCheckoutInformationCommandHandler : IRequestHandler<UpdateStripeCheckoutInformationCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStripeCheckoutInformationCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;    
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateStripeCheckoutInformationCommand request, CancellationToken cancellationToken)
        {
            CustomerOrder? order = await _orderRepository.GetByIdAsync(request.OrderId);

            if(order is null || order.BuyerId is null)
                return false;

            order.SetStripeSessionId(request.StripeSessionId);
            _orderRepository.Update(order);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}