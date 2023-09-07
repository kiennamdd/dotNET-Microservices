
using Cart.API.Models;
using FluentValidation;

namespace Cart.API.Validators
{
    public class CartItemUpsertRequestValidator: AbstractValidator<CartItemUpsertRequest>
    {
        public CartItemUpsertRequestValidator()
        {
            RuleFor(o => o.Quantity)
                .Must(quantity => quantity > 0)
                .WithMessage("'Quantity' must be greater 0.");
        }
    }
}