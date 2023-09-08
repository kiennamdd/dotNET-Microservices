
using Cart.API.Interfaces;
using EventBus.Events;
using MassTransit;

namespace Cart.API.IntegrationEventConsumers
{
    public class CouponDeletedEventConsumer : IConsumer<CouponDeletedEvent>
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CouponDeletedEventConsumer> _logger;


        public CouponDeletedEventConsumer(ICartItemRepository cartItemRepository, 
            IUnitOfWork unitOfWork, 
            ILogger<CouponDeletedEventConsumer> logger)
        {
            _cartItemRepository = cartItemRepository;    
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CouponDeletedEvent> context)
        {
            CouponDeletedEvent couponDeletedEvent = context.Message;

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