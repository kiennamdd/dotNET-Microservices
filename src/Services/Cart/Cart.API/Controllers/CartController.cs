using AutoMapper;
using Cart.API.Domain.Constants;
using Cart.API.Domain.Entities;
using Cart.API.Interfaces;
using Cart.API.Interfaces.Infrastructure;
using Cart.API.Models;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cart.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShoppingCartRepository _shoppingCartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly ICatalogService _catalogService;
        private readonly IDiscountService _discountService;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<CartController> _logger;
        private readonly IValidator<CartItemUpsertRequest> _upsertValidator;

        public CartController(IUnitOfWork unitOfWork, 
            IShoppingCartRepository shoppingCartRepository,
            ICartItemRepository cartItemRepository,
            ICatalogService catalogService,
            IDiscountService discountService,
            IMapper mapper,
            ICurrentUser currentUser,
            ILogger<CartController> logger,
            IValidator<CartItemUpsertRequest> upsertValidator)
        {
            _unitOfWork = unitOfWork;
            _shoppingCartRepository = shoppingCartRepository;
            _cartItemRepository = cartItemRepository;

            _catalogService = catalogService;
            _discountService = discountService;

            _mapper = mapper;
            _currentUser = currentUser;
            _logger = logger;

            _upsertValidator = upsertValidator;
        }

        [HttpGet]
        [Route("{userId}/Details")]
        public async Task<ResponseDto> GetDetails(string userId)
        {
            if(!_currentUser.IsMatchCurrentUserId(userId, out Guid parsedUserId) && !_currentUser.IsInRole(Roles.ADMIN))
            {
                return ResponseDto.Fail("Invalid user identifier.");
            }

            if(parsedUserId == Guid.Empty)
            {
                _logger.LogWarning("Can not retrieve user identifier.");
                return ResponseDto.Fail("Can not retrieve user identifier.");
            }

            IEnumerable<ShoppingCart> list = await _shoppingCartRepository.GetListAsync(predicate: o=>o.Id.Equals(parsedUserId), 
                                                                                    includeProperties: nameof(ShoppingCart.Items));

            ShoppingCart shoppingCart = list.FirstOrDefault() ?? new ShoppingCart(parsedUserId);

            shoppingCart.CartTocal = shoppingCart.GetTotal();

            if(!string.IsNullOrEmpty(shoppingCart.AppliedCouponCode))
            {
                CouponDto? couponDto = await _discountService.GetCouponByCodeAsync(shoppingCart.AppliedCouponCode);
                if(couponDto is null)
                {
                    shoppingCart.AppliedCouponCode = string.Empty;
                }
                else
                {
                    shoppingCart.DiscountAmount = couponDto.DiscountAmount;
                    shoppingCart.DiscountPercent = couponDto.DiscountPercent;

                    shoppingCart.CartTocal = _discountService.GetFinalValueAfterDiscount(shoppingCart.CartTocal,
                                                                                        couponDto.DiscountPercent,
                                                                                        couponDto.DiscountAmount,
                                                                                        couponDto.MinOrderTotal,
                                                                                        couponDto.MaxDiscountAmount);
                }
            }

            return ResponseDto.Success(result: _mapper.Map<ShoppingCartDto>(shoppingCart));
        }

        [HttpPost]
        [Route("Upsert")]
        public async Task<ResponseDto> Upsert(CartItemUpsertRequest upsertRequest)
        {
            await _upsertValidator.ValidateAndThrowAsync(upsertRequest);

            Guid userId = _currentUser.GetUserId();

            if(userId == Guid.Empty)
            {
                _logger.LogWarning("Can not retrieve user identifier.");

                return ResponseDto.Fail("Can not retrieve user identifier.");
            }
            
            ShoppingCart? shoppingCart = await _shoppingCartRepository.GetByIdAsync(userId);

            if(shoppingCart is null)
            {
                shoppingCart = new ShoppingCart(userId);

                await _shoppingCartRepository.AddAsync(shoppingCart);
                await _unitOfWork.SaveChangesAsync();
            }

            IEnumerable<CartItem> list = await _cartItemRepository.GetListAsync(
                                            predicate: o => o.ProductId == upsertRequest.ProductId && o.ShoppingCartId == shoppingCart.Id);

            CartItem? existedCartItem = list.FirstOrDefault();

            if(existedCartItem != null)
            {
                existedCartItem.Quantity += upsertRequest.Quantity;
                _cartItemRepository.Update(existedCartItem);
                await _unitOfWork.SaveChangesAsync();
                return ResponseDto.Success("Shopping cart updated successfully.");
            }
            
            ProductDto? productDto = await _catalogService.GetProductByIdAsync(upsertRequest.ProductId);
            if(productDto is null)
            {
                _logger.LogWarning($"Can not retrieve product from Catalog service. Product ID: {upsertRequest.ProductId}");
                return ResponseDto.Success("Fail to upsert cart.");
            }

            CartItem cartItem = new ()
            {
                ShoppingCartId = shoppingCart.Id,
                ProductId = productDto.Id,
                ProductName = productDto.Name,
                ProductOriginalPrice = productDto.Price,
                ProductLastPrice = productDto.Price,
                ProductThumbnailUrl = productDto.ThumbnailUrl,
                Quantity = upsertRequest.Quantity,
                ProductAppliedCouponCode = productDto.AppliedCouponCode
            };

            if(!string.IsNullOrEmpty(productDto.AppliedCouponCode))
            {
                cartItem.ProductLastPrice = _discountService.GetFinalValueAfterDiscount(cartItem.ProductOriginalPrice,
                                                                                        productDto.DiscountPercent,
                                                                                        productDto.DiscountAmount,
                                                                                        -1, -1);
            }
            
            await _cartItemRepository.AddAsync(cartItem);
            await _unitOfWork.SaveChangesAsync();
            return ResponseDto.Success("Shopping cart updated successfully.");
        }

        [HttpDelete]
        [Route("RemoveItem/{cartItemId}")]
        public async Task<ResponseDto> RemoveItem(int cartItemId)
        {
            Guid userId = _currentUser.GetUserId();

            if(userId == Guid.Empty)
            {
                _logger.LogWarning("Can not retrieve user identifier.");
                return ResponseDto.Fail("Can not retrieve user identifier.");
            }
            
            IEnumerable<CartItem> list = await _cartItemRepository.GetListAsync(o => o.ShoppingCartId == userId && o.Id == cartItemId);
            CartItem? cartItem = list.FirstOrDefault();

            if(cartItem != null)
            {
                _cartItemRepository.Delete(cartItem);
                await _unitOfWork.SaveChangesAsync();
            }

            return ResponseDto.Success();
        }

        [HttpPost]
        [Route("ApplyCoupon")]
        public async Task<ResponseDto> ApplyCoupon([FromBody] string couponCode)
        {
            couponCode = couponCode.Trim();

            Guid userId = _currentUser.GetUserId();
            if(userId == Guid.Empty)
            {
                _logger.LogWarning("Can not retrieve user identifier.");
                return ResponseDto.Fail("Can not retrieve user identifier.");
            }
            
            ShoppingCart? shoppingCart = await _shoppingCartRepository.GetByIdAsync(userId);
            if(shoppingCart is null)
                return ResponseDto.Fail();

            if(couponCode == string.Empty && !string.IsNullOrEmpty(shoppingCart.AppliedCouponCode))
            {
                shoppingCart.AppliedCouponCode = string.Empty;
                _shoppingCartRepository.Update(shoppingCart);
                await _unitOfWork.SaveChangesAsync();

                return ResponseDto.Success();
            }

            CouponDto? couponDto = await _discountService.GetCouponByCodeAsync(couponCode);
            if(couponDto != null)
            {
                shoppingCart.AppliedCouponCode = couponCode;
                _shoppingCartRepository.Update(shoppingCart);
                await _unitOfWork.SaveChangesAsync();
            }

            return ResponseDto.Success();
        }
    }
}