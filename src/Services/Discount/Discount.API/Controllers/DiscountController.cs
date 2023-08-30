using AutoMapper;
using Discount.API.Domain.Constants;
using Discount.API.Domain.Entities;
using Discount.API.Interfaces;
using Discount.API.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Discount.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;
        private readonly IMapper _mapper;
        private readonly IStripeService _stripeService;
        private readonly IValidator<CouponCreateRequest> _couponCreateValidator;

        public DiscountController(ICouponRepository couponRepository, 
            IMapper mapper,
            IStripeService stripeService,
            IValidator<CouponCreateRequest> couponCreateValidator)
        {
            _couponRepository = couponRepository;
            _mapper = mapper;
            _stripeService = stripeService;
            _couponCreateValidator = couponCreateValidator;
        }

        [HttpGet]
        [Route("coupons")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<ResponseDto> GetCouponList()
        {
            var list = await _couponRepository.GetListAsync();

            return ResponseDto.Success(result: _mapper.Map<IEnumerable<CouponDto>>(list));
        }

        [HttpGet]
        [Route("coupons/{id}")]
        public async Task<ResponseDto> GetCouponById(string id)
        {
            if(!ObjectId.TryParse(id, out _))
            {
                return ResponseDto.Fail("Invalid coupon identifier.");
            }

            var coupon = await _couponRepository.GetByIdAsync(id);
            if(coupon is null)
            {
                return ResponseDto.Fail("Not found");
            }

            return ResponseDto.Success(result: _mapper.Map<CouponDto>(coupon));
        }

        [HttpGet]
        [Route("coupons/withcode/{code}")]
        public async Task<ResponseDto> GetCouponByCode(string code)
        {
            var coupon = await _couponRepository.GetByCodeAsync(code);
            if (coupon is null)
            {
                return ResponseDto.Fail("Not found.");
            }

            return ResponseDto.Success(result: _mapper.Map<CouponDto>(coupon));
        }

        [HttpPost]
        [Route("coupons")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<ResponseDto> Post([FromBody] CouponCreateRequest couponCreateRequest)
        {
            _couponCreateValidator.ValidateAndThrow(couponCreateRequest);

            var coupon = _mapper.Map<Coupon>(couponCreateRequest);

            var success = await _stripeService.CreateCouponAsync(coupon);
            if(success == false)
            {
                return ResponseDto.Fail("Fail to create coupon in Stripe");
            }

            string couponId = await _couponRepository.AddAsync(coupon);
            if (string.IsNullOrEmpty(couponId))
            {
                return ResponseDto.Fail("Fail to add coupon in database");
            }

            return ResponseDto.Success(result: _mapper.Map<CouponDto>(coupon));
        }

        [HttpDelete]
        [Route("coupons/{id}")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<ResponseDto> Delete(string id)
        {
            if (!ObjectId.TryParse(id, out _))
            {
                return ResponseDto.Fail("Invalid coupon identifier.");
            }

            var existedCoupon = await _couponRepository.GetByIdAsync(id);
            if (existedCoupon is null)
            {
                return ResponseDto.Fail("Not found.");
            }

            var stripeCoupon = await _stripeService.FindCouponAsync(existedCoupon.CouponCode);
            if(stripeCoupon != null)
            {
                await _stripeService.DeleteCouponByCodeAsync(existedCoupon.CouponCode);
            }

            bool success = await _couponRepository.DeleteAsync(existedCoupon.Id);
            if (!success)
            {
                return ResponseDto.Fail("Fail to delete coupon in database");
            }

            return ResponseDto.Success("Coupon deleted successfully.");
        }
    }
}
