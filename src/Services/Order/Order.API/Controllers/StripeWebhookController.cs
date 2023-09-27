using MediatR;
using Microsoft.AspNetCore.Mvc;
using Order.Application.Common.Interfaces.Infrastructure;
using Order.Application.Common.Models;
using Order.Application.Features.Orders.Commands;
using Order.Application.Features.Orders.Queries;
using Order.Domain.Constants;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Stripe;
using Stripe.Checkout;

namespace Order.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StripeWebhook : ControllerBase
    {
        private readonly ILogger<StripeWebhook> _logger;
        private readonly IStripeService _stripeService;
        private readonly IMediator _mediator;

        public StripeWebhook(ILogger<StripeWebhook> logger, IStripeService stripeService, IMediator mediator)
        {
            _logger = logger;   
            _stripeService = stripeService; 
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                // if(string.IsNullOrEmpty(StripeSettings.EndpointSecret))
                //     throw new ArgumentNullException(nameof(StripeSettings.EndpointSecret));
                // var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], StripeSettings.EndpointSecret);

                var stripeEvent = EventUtility.ParseEvent(json);

                // Handle the event
                if (stripeEvent.Type == Events.CheckoutSessionCompleted)
                {
                    var session = stripeEvent.Data.Object as Session;

                    if(session is null)
                    {
                        _logger.LogWarning("Caught Stripe CheckoutSessionCompleted event but session value is null. Stripe Event ID: {0}", stripeEvent.Id);
                        return BadRequest();
                    }

                    var list = await _mediator.Send(new GetOrderListQuery(Guid.Empty, (order) => order.StripeSessionId == session.Id));
                    CustomerOrder? order = list.FirstOrDefault();
                    if(order is null)
                    {
                        _logger.LogWarning("Caught Stripe CheckoutSessionCompleted event but no order is found. Checkout Session ID: {0}", session.Id);
                        return BadRequest();
                    }

                    if(order.Status != OrderStatus.AwaitingPayment)
                    {
                        return Ok();
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
                }
                else
                {
                    _logger.LogWarning("Unhandled Stripe event type: {0}", stripeEvent.Type);
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                _logger.LogWarning(ex, "Unhandled Stripe exception occurred.");
                return BadRequest();
            }
        }
    }
}