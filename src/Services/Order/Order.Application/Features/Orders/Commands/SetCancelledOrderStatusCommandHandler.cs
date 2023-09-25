using MediatR;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;

namespace Order.Application.Features.Orders.Commands
{
    public class SetCancelledOrderStatusCommandHandler : IRequestHandler<SetCancelledOrderStatusCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SetCancelledOrderStatusCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;    
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(SetCancelledOrderStatusCommand request, CancellationToken cancellationToken)
        {
            CustomerOrder? order = await _orderRepository.GetByIdAsync(request.OrderId);

            if(order is null)
                return false;

            order.SetStatusToCancelled();
            _orderRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}