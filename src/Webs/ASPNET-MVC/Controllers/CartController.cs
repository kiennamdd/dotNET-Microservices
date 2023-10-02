using System;
using System.Collections.Generic;
using ASPNET_MVC.Interfaces;
using ASPNET_MVC.Models.Cart;
using Cart.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET_MVC.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;
        private readonly ICartService _cartService;
        private readonly ICurrentUser _currentUser;

        public CartController(ILogger<CartController> logger, ICartService cartService, ICurrentUser currentUser)
        {
            _logger = logger;
            _cartService = cartService;
            _currentUser = currentUser;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> CartDetails()
        {
            Guid userId = _currentUser.GetUserId();
            if(userId == Guid.Empty)
            {
                _logger.LogWarning("User is authorized but can not retrieve user id.");
                return RedirectToAction(nameof(IdentityController.SignIn), "Identity");
            }

            ResponseDto<ShoppingCartDto> response = await _cartService.GetCartDetails(userId.ToString());
            ShoppingCartDto cart = response.Result ?? new ShoppingCartDto();
            return View(cart);
        }

        [HttpPost]
        [Route("upsert")]
        public async Task<IActionResult> CartUpsertItem([FromForm] CartItemUpsertRequest cartItemUpsertRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            ResponseDto response = await _cartService.UpsertCartItem(cartItemUpsertRequest);
            if(response.IsSuccess == false)
            {
                TempData["Error"] = response.Message;
            }
            else
            {
                TempData["Success"] = "Item added to cart.";
            }

            return RedirectToAction(nameof(CartDetails));
        }

        [HttpPost]
        [Route("remove/{itemId}")]
        public async Task<IActionResult> CartRemoveItem(int itemId)
        {
            ResponseDto response = await _cartService.RemoveCartItem(itemId);
            if(response.IsSuccess == false)
            {
                TempData["Error"] = response.Message;
            }
            else
            {
                TempData["Success"] = "Item removed from cart.";
            }

            return RedirectToAction(nameof(CartDetails));
        }

        [HttpPost]
        [Route("apply-coupon")]
        public async Task<IActionResult> CartApplyCoupon([FromForm] string couponCode)
        {
            ResponseDto response = await _cartService.ApplyCouponForCart(couponCode);
            if(response.IsSuccess == false)
            {
                TempData["Error"] = !string.IsNullOrEmpty(response.Message) ? response.Message : "Apply coupon failed.";
            }
            else
            {
                TempData["Success"] = "Coupon changed successfully!";
            }

            return RedirectToAction(nameof(CartDetails));
        }

        [HttpPost]
        [Route("remove-coupon")]
        public async Task<IActionResult> CartRemoveCoupon()
        {
            ResponseDto response = await _cartService.ApplyCouponForCart(string.Empty);
            if(response.IsSuccess == false)
            {
                TempData["Error"] = response.Message;
            }
            else
            {
                TempData["Success"] = "Coupon changed successfully!";
            }

            return RedirectToAction(nameof(CartDetails));
        }
    }
}