
using FluentValidation;
using Identity.API.Models;

namespace Identity.API.Validators
{
    public class SignInRequestValidator: AbstractValidator<SignInRequest>
    {
        public SignInRequestValidator()
        {
            RuleFor(o => o.Email).NotEmpty().EmailAddress();
            RuleFor(o => o.Password).NotEmpty();
        }
    }
}