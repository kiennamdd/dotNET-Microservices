
using FluentValidation;
using Identity.API.Domain.Entities;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.API.Validators
{
    public class RegisterRequestValidator: AbstractValidator<RegisterRequest>
    {
        public RegisterRequestValidator()
        {
            RuleFor(o => o.PhoneNumber)
                .NotEmpty()
                .MaximumLength(10);

            RuleFor(o => o.Email)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(50);

            RuleFor(o => o.FullName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(o => o.Password)
                .NotEmpty()
                .MinimumLength(4)
                .Matches(@"[a-z]+").WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"[0-9]+").WithMessage("Password must contain at least one number.");
        }
    }
}