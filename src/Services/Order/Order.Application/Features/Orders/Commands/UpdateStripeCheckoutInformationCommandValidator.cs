using FluentValidation;

namespace Order.Application.Features.Orders.Commands
{
    public class UpdateStripeCheckoutInformationCommandValidator: AbstractValidator<UpdateStripeCheckoutInformationCommand>
    {
        public UpdateStripeCheckoutInformationCommandValidator()
        {
            RuleFor(o => o.StripeSessionId).NotEmpty();
        }
    }
}