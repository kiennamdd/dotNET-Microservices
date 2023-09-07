
using Cart.API.Interfaces;
using Cart.API.Interfaces.Infrastructure;
using EventBus.Events;
using MassTransit;

namespace Cart.API.IntegrationEventConsumers
{
    public class ProductCouponCodeChangedEventConsumer : IConsumer<ProductCouponCodeChangedEvent>
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDiscountService _discountService;


        public ProductCouponCodeChangedEventConsumer(ICartItemRepository cartItemRepository, 
            IUnitOfWork unitOfWork, 
            IDiscountService discountService)
        {
            _cartItemRepository = cartItemRepository;    
            _unitOfWork = unitOfWork;
            _discountService = discountService;
        }

        public async Task Consume(ConsumeContext<ProductCouponCodeChangedEvent> context)
        {
            ProductCouponCodeChangedEvent changeEvent = context.Message;

            var list = await  _cartItemRepository.GetListAsync(o => o.ProductId == changeEvent.ProductId);

            if(string.IsNullOrEmpty(changeEvent.AppliedCouponCode))
            {
                // If coupon code is empty, reset product last price value
                foreach(var cartItem in list)
                {
                    cartItem.ProductLastPrice = cartItem.ProductOriginalPrice;
                    _cartItemRepository.Update(cartItem);
                }
            }
            else
            {
                // If coupon code is available, re-calculate discount value for product last price
                double newLastPrice = _discountService.GetFinalValueAfterDiscount(changeEvent.Price,
                                                                                changeEvent.DiscountPercent,
                                                                                changeEvent.DiscountAmount,
                                                                                -1, -1);;

                foreach(var cartItem in list)
                {
                    cartItem.ProductLastPrice = newLastPrice;
                    _cartItemRepository.Update(cartItem);
                }
            }
            
            await _unitOfWork.SaveChangesAsync();
        }
    }
}