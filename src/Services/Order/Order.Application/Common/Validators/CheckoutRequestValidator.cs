using FluentValidation;
using Order.Application.Common.Models;

namespace Order.Application.Common.Validators
{
    public class CheckoutRequestValidator: AbstractValidator<CheckoutRequest>
    {
        public CheckoutRequestValidator()
        {
            RuleFor(o => o.SuccessUrl).NotEmpty();
            RuleFor(o => o.CancelUrl).NotEmpty();
        }
    }
}