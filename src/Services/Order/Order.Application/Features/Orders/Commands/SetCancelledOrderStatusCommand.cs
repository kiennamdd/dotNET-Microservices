using MediatR;

namespace Order.Application.Features.Orders.Commands
{
    public class SetCancelledOrderStatusCommand: IRequest<bool>
    {
        public Guid OrderId { get; private set; }

        public SetCancelledOrderStatusCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}