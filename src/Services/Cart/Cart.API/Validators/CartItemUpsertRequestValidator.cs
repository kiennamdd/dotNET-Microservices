
using Cart.API.Models;
using FluentValidation;

namespace Cart.API.Validators
{
    public class CartItemUpsertRequestValidator: AbstractValidator<CartItemUpsertRequest>
    {
        public CartItemUpsertRequestValidator()
        {
            RuleFor(o => o.ProductId)
                .NotEmpty();

            RuleFor(o => o.Quantity)
                .NotEmpty()
                .Must(quantity => quantity > 0)
                .WithMessage("'Quantity' must be greater 0.");
        }
    }
}