using MediatR;
using Order.Domain.Enums;

namespace Order.Application.Features.Orders.Commands
{
    public class SetPaidOrderStatusCommand: IRequest<bool>
    {
        public Guid OrderId { get; private set; }
        public string StripePaymentIntentId { get; private set; }

        public SetPaidOrderStatusCommand(Guid orderId, string stripePaymentIntentId)
        {
            OrderId = orderId;
            StripePaymentIntentId = stripePaymentIntentId;
        }
    }
}