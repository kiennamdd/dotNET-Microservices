using MediatR;

namespace Order.Application.Features.Orders.Commands
{
    public class SetShippedOrderStatusCommand: IRequest<bool>
    {
        public Guid OrderId { get; private set; }

        public SetShippedOrderStatusCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}