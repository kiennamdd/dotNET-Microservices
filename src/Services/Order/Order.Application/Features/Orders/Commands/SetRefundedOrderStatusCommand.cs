using MediatR;

namespace Order.Application.Features.Orders.Commands
{
    public class SetRefundedOrderStatusCommand: IRequest<bool>
    {
        public Guid OrderId { get; private set; }

        public SetRefundedOrderStatusCommand(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}