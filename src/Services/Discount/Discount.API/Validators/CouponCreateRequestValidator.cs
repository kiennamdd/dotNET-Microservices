using Discount.API.Interfaces;
using Discount.API.Models;
using FluentValidation;

namespace Discount.API.Validators
{
    public class CouponCreateRequestValidator: AbstractValidator<CouponCreateRequest>
    {
        private readonly ICouponRepository _couponRepository;

        public CouponCreateRequestValidator(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;

            RegisterRules();
        }

        public void RegisterRules()
        {
            RuleFor(o => o.CouponCode).NotEmpty().WithMessage("{CouponCode} is required.")
                .MinimumLength(4).WithMessage("{CouponCode} must be greater than 4 characters.")
                .MaximumLength(20).WithMessage("{CouponCode} must not exceed 20 characters.")
                .Must(couponCode => IsNotDuplicateCode(couponCode))
                .WithMessage("{CouponCode} already exists.");

            RuleFor(o => o.MinOrderTotal).GreaterThanOrEqualTo(0).WithMessage("{MinOrderTotal} must greater than or equal 0.");

            RuleFor(o => o.DiscountPercent).InclusiveBetween(0, 100).WithMessage("{DiscountPercent} must be between 0 and 100.");

            RuleFor(o => o.DiscountAmount).GreaterThanOrEqualTo(0).WithMessage("{DiscountAmount} must greater than or equal 0.");

            RuleFor(o => o.MaxDiscountAmount).GreaterThanOrEqualTo(0).WithMessage("{MaxDiscountAmount} must greater than or equal 0.");

            RuleFor(o => new { o.DiscountAmount, o.DiscountPercent })
                .Must(obj => (obj.DiscountPercent > 0 || obj.DiscountAmount > 0))
                .WithName(obj => nameof(obj.DiscountAmount))
                .WithMessage("{DiscountAmount} must greater than 0 if {DiscountPercent} is not assigned.");
        }

        private bool IsNotDuplicateCode(string code)
        {
            var coupon = _couponRepository.GetByCodeAsync(code).GetAwaiter().GetResult();
            return coupon is null;
        }
    }
}
