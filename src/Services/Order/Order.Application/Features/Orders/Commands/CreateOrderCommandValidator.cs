
using FluentValidation;

namespace Order.Application.Features.Orders.Commands
{
    public class CreateOrderCommandValidator: AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(o => o.UserName).NotEmpty();
            RuleFor(o => o.DiscountAmount).Must(value => value >= 0).WithMessage("DiscountAmount can not be negative");
            RuleFor(o => o.DiscountPercent).Must(value => value >= 0).WithMessage("DiscountPercent can not be negative");
            RuleFor(o => o.Street).NotEmpty();
            RuleFor(o => o.City).NotEmpty();
            RuleFor(o => o.State).NotEmpty();
            RuleFor(o => o.Country).NotEmpty();
            RuleFor(o => o.ZipCode).NotEmpty();
            RuleFor(o => o.OrderTotal).Must(value => value >= 0).WithMessage("OrderTotal can not be negative");
            RuleFor(o => o.OrderItemDtos).Must(list => list.Any()).WithMessage("Order must have at least one item.");
        }

        private bool IsNotNegativeValue(double value)
        {
            return value >= 0;
        }
    }
}