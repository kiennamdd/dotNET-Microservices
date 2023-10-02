using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Common.Interfaces;
using Order.Application.Common.Interfaces.Infrastructure;
using Order.Application.Common.Models;
using Order.Application.Features.Orders.Commands;
using Order.Application.Features.Orders.Queries;
using Order.Domain.Constants;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IStripeService _stripeService;
        private readonly ICartService _cartService;
        private readonly ICurrentUser _currentUser;
        private readonly ILogger<OrderController> _logger;
        private readonly IValidator<CheckoutRequest> _checkoutRequestValidator;

        public OrderController(IMediator mediator, 
            IStripeService stripeService, 
            ICartService cartService,
            ICurrentUser currentUser, 
            ILogger<OrderController> logger,
            IValidator<CheckoutRequest> checkoutRequestValidator)
        {
            _mediator = mediator;
            _stripeService = stripeService;
            _cartService = cartService;
            _currentUser = currentUser;
            _logger = logger;
            _checkoutRequestValidator = checkoutRequestValidator;
        }

        private async Task<CustomerOrder?> FindOrderByIdAsync(string orderId, Guid userId, string includeProperties = "")
        {
            if(!Guid.TryParse(orderId, out Guid parsedOrderId))
            {
                _logger.LogWarning($"Request with invalid order identifier. Request order ID: {orderId}");
                return null;
            }

            IEnumerable<CustomerOrder> list = await _mediator.Send(new GetOrderListQuery() { UserId = userId, IncludesProperties = includeProperties });
            CustomerOrder? order = list.FirstOrDefault(o => o.Id == parsedOrderId);

            return order;
        }

        private async Task<CustomerOrder?> GetOrderOfCurrentUserByIdAsync(string orderId, string includeProperties = "")
        {
            Guid userId = _currentUser.GetUserId();
            if(userId == Guid.Empty)
            {
                _logger.LogError("Can not retrieve user id.");
                return null;
            }

            CustomerOrder? order = await FindOrderByIdAsync(orderId, userId, includeProperties);
            return order;
        }

        [HttpPost]
        [Route("create")]
        public async Task<ResponseDto> CreateOrder([FromBody] CreateOrderRequest checkoutRequest)
        {
            Guid userId = _currentUser.GetUserId();
            if(userId == Guid.Empty)
            {
                _logger.LogError("Can not retrieve user id.");
                return ResponseDto.Fail("Invalid user identifier.");
            }

            string userName = _currentUser.GetUserName();
            if(string.IsNullOrEmpty(userName))
            {
                _logger.LogError($"Can not retrieve user name. User ID: {userId}");
                return ResponseDto.Fail("Invalid user name.");
            }

            ShoppingCartDto? currentUserCart = await _cartService.GetCartByUserId(userId);
            if(currentUserCart is null)
            {
                _logger.LogError($"Can not retrieve user's shopping cart. User ID: {userId}");
                return ResponseDto.Fail("User cart not found.");
            }

            if(currentUserCart.Id != checkoutRequest.CartId)
            {
                _logger.LogWarning($"Invalid shopping cart identifier in checkout request. User ID: {userId}. User Cart ID: {currentUserCart.Id}. Request Cart ID: {checkoutRequest.CartId}");
                return ResponseDto.Fail("Invalid shopping cart identifier in checkout request.");
            }

            IEnumerable<OrderItemDto> orderItems = currentUserCart.Items.ToOrderItemDtoList();

            var command = new CreateOrderCommand(userId, userName, 
                                            currentUserCart.AppliedCouponCode, currentUserCart.DiscountAmount, currentUserCart.DiscountPercent,
                                            checkoutRequest.Street, checkoutRequest.City, checkoutRequest.State, checkoutRequest.Country, checkoutRequest.ZipCode, 
                                            currentUserCart.CartTocal, orderItems); 

            Guid orderId = await _mediator.Send(command);
            return ResponseDto.Success(result: new CreateOrderResponse { OrderId = orderId });
        }

        [HttpPost]
        [Route("checkout")]
        public async Task<ResponseDto> Checkout([FromBody] CheckoutRequest checkoutRequest)
        {
            await _checkoutRequestValidator.ValidateAndThrowAsync(checkoutRequest);

            CustomerOrder? order = await GetOrderOfCurrentUserByIdAsync(checkoutRequest.OrderId, nameof(CustomerOrder.Items));
            if(order is null)
            {
                _logger.LogWarning($"Order not found. Request order ID: {checkoutRequest.OrderId}");
                return ResponseDto.Fail("Order not found.");
            }

            if(order.Status != OrderStatus.AwaitingPayment)
            {
                _logger.LogWarning($"Request checkout for order. Request order ID: {checkoutRequest.OrderId}");
                return ResponseDto.Fail("Checkout is not available for current order status.");
            }

            // Expire previous existing checkout session
            if(!string.IsNullOrEmpty(order.StripeSessionId))
            {
                var session = await _stripeService.GetSessionByIdAsync(order.StripeSessionId);

                if(session.Status == StripeSessionStatus.OPEN)
                {
                    await _stripeService.ExpireSessionAsync(session.Id);
                }
            }

            var checkoutOptions = _stripeService.GenerateSessionCreateOptionsForOrder(order.Items, checkoutRequest.SuccessUrl, checkoutRequest.CancelUrl, order.AppliedCouponCode);
            var checkoutSession = await _stripeService.CreateSessionAsync(checkoutOptions);

            var checkoutResponse = new CheckoutResponse
            {
                StripeSessionId = checkoutSession.Id,
                StripeCheckoutUrl = checkoutSession.Url
            };

            await _mediator.Send(new UpdateStripeCheckoutInformationCommand(order.Id, checkoutSession.Id));
            return ResponseDto.Success(result: checkoutResponse);
        }

        [HttpGet]
        [Route("confirm/{orderId}")]
        public async Task<ResponseDto> ConfirmCheckout(string orderId)
        {
            CustomerOrder? order = await GetOrderOfCurrentUserByIdAsync(orderId);
            if(order is null)
            {
                _logger.LogWarning($"Order not found. Request order ID: {orderId}");
                return ResponseDto.Fail("Order not found.");
            }

            if(order.Status != OrderStatus.AwaitingPayment)
            {
                return ResponseDto.Fail();
            }

            if(string.IsNullOrEmpty(order.StripeSessionId))
            {
                _logger.LogWarning($"Checkout has not been created for the order. Order ID: {order.Id}");
                return ResponseDto.Fail("Checkout has not been created for the order.");
            }

            var session = await _stripeService.GetSessionByIdAsync(order.StripeSessionId);
            if(string.IsNullOrEmpty(session.PaymentIntentId))
            {
                return ResponseDto.Fail("Checkout is not completed yet.");
            }

            var paymentIntent = await _stripeService.GetPaymentIntentByIdAsync(session.PaymentIntentId);
            // Payment success
            if(paymentIntent.Status == StripePaymentStatus.SUCCEEDED)
            {
                await _mediator.Send(new SetPaidOrderStatusCommand(order.Id, session.PaymentIntentId));
            }
            // Payment fail
            else if(paymentIntent.Status == StripePaymentStatus.REQUIRES_PAYMENT_METHOD)
            {
                await _mediator.Send(new SetCancelledOrderStatusCommand(order.Id));
            }

            return ResponseDto.Success();
        }

        [HttpPost]
        [Route("cancel/{orderId}")]
        public async Task<ResponseDto> CancelOrder(string orderId)
        {
            CustomerOrder? order = await GetOrderOfCurrentUserByIdAsync(orderId);

            if(order is null)
            {
                _logger.LogWarning($"Order not found. Request order ID: {orderId}");
                return ResponseDto.Fail("Order not found.");
            }

            if(order.Status == OrderStatus.Paid)
            {
                if(string.IsNullOrEmpty(order.StripeSessionId) || string.IsNullOrEmpty(order.StripePaymentIntentId))
                {
                    _logger.LogError($"Order status is paid but checkout has not been created for the order. Order ID: {order.Id}");
                    return ResponseDto.Fail("Checkout has not been created for the order");
                }

                var refundOptions = _stripeService.GenerateRefundCreateOptionsForOrder(order.StripePaymentIntentId, StripeRefundReason.REQUESTED_BY_CUSTOMER);
                await _stripeService.CreateRefundAsync(refundOptions);

                await _mediator.Send(new SetRefundedOrderStatusCommand(order.Id));
            }
            else if(order.Status == OrderStatus.AwaitingPayment)
            {
                if(!string.IsNullOrEmpty(order.StripeSessionId))
                {
                    var session = await _stripeService.GetSessionByIdAsync(order.StripeSessionId);

                    if(session.Status == StripeSessionStatus.OPEN)
                    {
                        await _stripeService.ExpireSessionAsync(session.Id);
                    }
                }

                await _mediator.Send(new SetCancelledOrderStatusCommand(order.Id));
            }
            else   
            {
                return ResponseDto.Fail();
            }

            // If order status is Paid or AwaitingPayment
            return ResponseDto.Success();
        }

        [HttpPost]
        [Route("mark-as-shipped/{orderId}")]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<ResponseDto> SetShippedOrderStatus(string orderId)
        {
            CustomerOrder? order = await FindOrderByIdAsync(orderId, Guid.Empty);

            if(order is null)
            {
                _logger.LogWarning($"Order not found. Request order ID: {orderId}");
                return ResponseDto.Fail("Order not found.");
            }

            if(order.Status != OrderStatus.Paid)
            {
                return ResponseDto.Fail();
            }

            await _mediator.Send(new SetShippedOrderStatusCommand(order.Id));
            return ResponseDto.Success();
        }

        [HttpGet]
        [Route("history")]
        public async Task<ResponseDto> OrderHistory()
        {
            Guid userId = _currentUser.GetUserId();
            if(userId == Guid.Empty)
            {
                _logger.LogError("Can not retrieve user id.");
                return ResponseDto.Fail();
            }

            var query = new GetOrderListQuery() 
            { 
                UserId = userId, 
            };

            IEnumerable<CustomerOrder> list = await _mediator.Send(query);
            return ResponseDto.Success(result: list.ToOrderDtoList());
        }

        [HttpGet]
        [Route("details/{orderId}")]
        public async Task<ResponseDto> OrderDetails(string orderId)
        {
            if(!Guid.TryParse(orderId, out Guid parsedOrderId))
            {
                _logger.LogWarning($"Request with invalid order identifier. Request order ID: {orderId}");
                return ResponseDto.Success(result: null);
            }

            // Admin can retrieve user order details
            if(_currentUser.IsInRole(Roles.ADMIN))
            {
                CustomerOrder? order = await _mediator.Send(new GetOrderDetailsQuery(parsedOrderId));
                return ResponseDto.Success(result: order?.ToOrderDto());
            }

            Guid userId = _currentUser.GetUserId();
            if(userId == Guid.Empty)
            {
                _logger.LogError("Can not retrieve user id.");
                return ResponseDto.Fail();
            }

            CustomerOrder? customerOrder = await _mediator.Send(new GetOrderDetailsQuery(parsedOrderId));
            if(customerOrder is null || customerOrder?.Buyer is null || customerOrder.Buyer.UserId != userId)
            {
                _logger.LogError($"Unauthorized user retrieving another user's order details. User ID: {userId}");
                return ResponseDto.Fail("Unauthorized.");
            }

            return ResponseDto.Success(result: customerOrder.ToOrderDto());
        }

        [HttpGet]
        [Authorize(Roles = Roles.ADMIN)]
        public async Task<ResponseDto> GetOrderList()
        {
            var query = new GetOrderListQuery();
            IEnumerable<CustomerOrder> list = await _mediator.Send(query);
            return ResponseDto.Success(result: list.ToOrderDtoList());
        }
    }
}
