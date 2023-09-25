
using Cart.API.Interfaces;
using EventBus.Events;
using MassTransit;

namespace Cart.API.IntegrationEventConsumers
{
    public class CouponDeletedIntegrationEventConsumer : IConsumer<CouponDeletedIntegrationEvent>
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CouponDeletedIntegrationEventConsumer> _logger;


        public CouponDeletedIntegrationEventConsumer(ICartItemRepository cartItemRepository, 
            IUnitOfWork unitOfWork, 
            ILogger<CouponDeletedIntegrationEventConsumer> logger)
        {
            _cartItemRepository = cartItemRepository;    
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CouponDeletedIntegrationEvent> context)
        {
            CouponDeletedIntegrationEvent couponDeletedEvent = context.Message;

            if(string.IsNullOrEmpty(couponDeletedEvent.CouponCode))
            {
                _logger.LogWarning($"Coupon code is null or empty. Coupon ID: {couponDeletedEvent.CouponId}");
                return;
            }

            var list = await  _cartItemRepository.GetListAsync(predicate: o => o.ProductAppliedCouponCode == couponDeletedEvent.CouponCode);

            foreach(var cartItem in list)
            {
                cartItem.ProductLastPrice = cartItem.ProductOriginalPrice;
                cartItem.ProductAppliedCouponCode = string.Empty;
                _cartItemRepository.Update(cartItem);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}