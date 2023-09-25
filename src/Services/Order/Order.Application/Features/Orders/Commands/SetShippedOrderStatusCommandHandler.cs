
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;

namespace Order.Application.Features.Orders.Commands
{
    public class SetShippedOrderStatusCommandHandler : IRequestHandler<SetShippedOrderStatusCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SetShippedOrderStatusCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;    
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(SetShippedOrderStatusCommand request, CancellationToken cancellationToken)
        {
            CustomerOrder? order = await _orderRepository.GetByIdAsync(request.OrderId);

            if(order is null)
                return false;

            order.SetStatusToShipped();
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}