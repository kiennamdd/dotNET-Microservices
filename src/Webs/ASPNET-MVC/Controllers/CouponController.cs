using ASPNET_MVC.Constants;
using ASPNET_MVC.Interfaces;
using ASPNET_MVC.Models.Discount;
using Cart.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET_MVC.Controllers
{
    [Route("[controller]")]
    public class CouponController : Controller
    {
        private readonly ILogger<CouponController> _logger;
        private readonly IDiscountService _discountService;

        public CouponController(ILogger<CouponController> logger, IDiscountService discountService)
        {
            _logger = logger;
            _discountService = discountService;
        }

        [HttpGet]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> CouponIndex()
        {
            ResponseDto<IEnumerable<CouponDto>> response = await _discountService.GetCouponList();

            IEnumerable<CouponDto> list = response.Result ?? new List<CouponDto>();

            return View(list);
        }

        [HttpGet]
        [Route("create")]
        [Authorize(Roles = Roles.ADMIN)]
        public IActionResult CouponCreate()
        {
            return View();
        }

        [HttpPost]
        [Route("create")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> CouponCreate([FromForm] CouponCreateRequest couponCreateRequest)
        {
            if(!ModelState.IsValid)
            {
                return View(couponCreateRequest);
            }

            ResponseDto response = await _discountService.CreateCoupon(couponCreateRequest);
            if(!response.IsSuccess)
            {
                TempData["Error"] = response.Message;
            }
            else
            {
                TempData["Success"] = "Coupon created successfully!";
            }

            return RedirectToAction(nameof(CouponIndex));
        }

        [HttpPost]
        [Route("{couponId}")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<IActionResult> CouponDelete(string couponId)
        {
            ResponseDto response = await _discountService.DeleteCoupon(couponId);
            if(response.IsSuccess == false)
            {
                TempData["Error"] = response.Message;
            }
            else
            {
                TempData["Success"] = "Coupon deleted successfully!";
            }

            return RedirectToAction(nameof(CouponIndex));
        }
    }
}