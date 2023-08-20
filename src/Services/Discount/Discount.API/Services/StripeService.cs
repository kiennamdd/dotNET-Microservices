using Discount.API.Domain.Constants;
using Discount.API.Domain.Entities;
using Discount.API.Interfaces;
using System.Net;

namespace Discount.API.Services
{
    public class StripeService : IStripeService
    {
        public async Task<bool> CreateCouponAsync(Coupon coupon)
        {
            try
            {
                var couponOptions = new Stripe.CouponCreateOptions
                {
                    Currency = Currency.USD,
                    Name = coupon.CouponCode,
                    Id = coupon.CouponCode
                };

                if (coupon.DiscountAmount > 0)
                {
                    couponOptions.AmountOff = (long)(coupon.DiscountAmount * 100);
                }

                if (coupon.DiscountPercent > 0)
                {
                    couponOptions.PercentOff = coupon.DiscountPercent;
                }

                var service = new Stripe.CouponService();
                Stripe.Coupon stripeCoupon = await service.CreateAsync(couponOptions);
                return (stripeCoupon.StripeResponse.StatusCode == HttpStatusCode.OK);
            }
            catch (Stripe.StripeException e)
            {
                return false;
            }
        }

        public async Task<bool> DeleteCouponByCodeAsync(string couponCode)
        {
            try
            {
                var service = new Stripe.CouponService();
                Stripe.Coupon stripeCoupon = await service.DeleteAsync(couponCode);
                return (stripeCoupon.StripeResponse.StatusCode == HttpStatusCode.OK);
            }
            catch (Stripe.StripeException e)
            {
                return false;
            }
        }

        public async Task<Stripe.Coupon?> FindCouponAsync(string couponCode)
        {
            try
            {
                var service = new Stripe.CouponService();
                Stripe.Coupon stripeCoupon = await service.GetAsync(couponCode);

                return stripeCoupon;
            }
            catch(Stripe.StripeException e)
            {
                return null;
            }
        }
    }
}
