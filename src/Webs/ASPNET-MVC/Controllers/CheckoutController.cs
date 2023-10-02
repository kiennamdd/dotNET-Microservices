using ASPNET_MVC.Interfaces;
using ASPNET_MVC.Models.Cart;
using ASPNET_MVC.Models.Order;
using Cart.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASPNET_MVC.Controllers
{
    [Route("[controller]")]
    public class CheckoutController : Controller
    {
        private readonly ILogger<CheckoutController> _logger;
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly ICurrentUser _currentUser;

        public CheckoutController(ILogger<CheckoutController> logger, 
            IOrderService orderService, 
            ICurrentUser currentUser,
            ICartService cartService)
        {
            _logger = logger;
            _orderService = orderService;
            _currentUser = currentUser;
            _cartService = cartService;
        }

        [HttpGet]
        [Route("success/{orderId}")]
        public IActionResult CheckoutSuccess(string orderId)
        {
            return View(nameof(CheckoutSuccess), orderId);
        }

        [HttpGet]
        [Route("cancel/{orderId}")]
        public IActionResult CheckoutCancel(string orderId)
        {
            return View(nameof(CheckoutCancel), orderId);
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            Guid userId = _currentUser.GetUserId();
            ResponseDto<ShoppingCartDto> response = await _cartService.GetCartDetails(userId.ToString());
            if(response.IsSuccess == false || response.Result is null)
            {
                TempData["Error"] = !string.IsNullOrEmpty(response.Message) ? response.Message : "Can not retrieve user cart.";
                return LocalRedirect("/");
            }

            if(!response.Result.Items.Any())
            {
                TempData["Error"] = "User cart is empty.";
                return BadRequest();
            }

            var model = new CreateOrderRequest()
            {
                CartId = response.Result.Id,
                Street = "Street",
                State = "State",
                City = "City",
                Country = "Country",
                ZipCode = "ZipCode"
            };

            ViewBag.Cart = response.Result;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout([FromForm] CreateOrderRequest createOrderRequest)
        {
            if(!ModelState.IsValid)
                return View(createOrderRequest);

            ResponseDto<CreateOrderResponse> createOrderResponse = await _orderService.CreateOrder(createOrderRequest);
            if(createOrderResponse.IsSuccess == false || createOrderResponse.Result is null)
            {
                TempData["Error"] = !string.IsNullOrEmpty(createOrderResponse.Message) ? createOrderResponse.Message : "Create order fail!";
                return View(createOrderRequest);
            }

            string baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
            string orderId = createOrderResponse.Result.OrderId.ToString();
            var checkoutRequest = new CheckoutRequest
            {
                OrderId = orderId,
                SuccessUrl = baseUrl + $"/checkout/success/{orderId}",
                CancelUrl = baseUrl + $"/checkout/cancel/{orderId}"
            };

            ResponseDto<CheckoutResponse> createCheckoutResponse = await _orderService.CreateCheckout(checkoutRequest);
            if(createCheckoutResponse.IsSuccess == false || createCheckoutResponse.Result is null)
            {
                TempData["Error"] = !string.IsNullOrEmpty(createCheckoutResponse.Message) ? createCheckoutResponse.Message : "Create order fail!";
                return View(createOrderRequest);
            }

            CheckoutResponse checkoutResponse = createCheckoutResponse.Result;
            return Redirect(checkoutResponse.StripeCheckoutUrl);
        }
    }
}