using FluentValidation;

namespace Order.Application.Features.Orders.Commands
{
    public class SetPaidOrderStatusCommandValidator: AbstractValidator<SetPaidOrderStatusCommand>
    {
        public SetPaidOrderStatusCommandValidator()
        {
            RuleFor(o => o.StripePaymentIntentId).NotEmpty();
        }
    }
}