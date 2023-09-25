using MediatR;

namespace Order.Application.Features.Orders.Commands
{
    public class UpdateStripeCheckoutInformationCommand: IRequest<bool>
    {
        public Guid OrderId { get; private set; }
        public string StripeSessionId { get; private set; }

        public UpdateStripeCheckoutInformationCommand(Guid orderId, string stripeSessionId)
        {
            OrderId = orderId;
            StripeSessionId = stripeSessionId;
        }
    }
}