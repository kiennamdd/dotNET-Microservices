
using Catalog.API.Models;
using FluentValidation;

namespace Catalog.API.Validators
{
    public class ProductUpdateRequestValidator: AbstractValidator<ProductUpdateRequest>
    {
        public ProductUpdateRequestValidator()
        {
            RuleFor(o => o.Id)
                .NotEmpty();

            RuleFor(o => o.Name)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(o => o.Description)
                .MaximumLength(500);

            RuleFor(o => o.Origin)
                .MaximumLength(10);

            RuleFor(o => o.Price)
                .Must(price => price >= 0)
                .WithMessage("[Price] can not have negative value.");

            RuleFor(o => o.CategoryName)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(o => o.BrandName)
                .MaximumLength(50);
        }
    }
}