
using Catalog.API.Domain.Entities;
using Catalog.API.Interfaces;
using EventBus.Events;
using MassTransit;

namespace Catalog.API.EventConsumers
{
    public class CouponDeletedEventConsumer : IConsumer<CouponDeletedIntegrationEvent>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<CouponDeletedEventConsumer> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CouponDeletedEventConsumer(IProductRepository productRepository, 
            IUnitOfWork unitOfWork,
            ILogger<CouponDeletedEventConsumer> logger)
        {
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CouponDeletedIntegrationEvent> context)
        {
            CouponDeletedIntegrationEvent coupon = context.Message;

            if(string.IsNullOrEmpty(coupon.CouponCode))
            {
                _logger.LogWarning($"Coupon code is null or empty. Coupon ID: {coupon.CouponId}");
                return;
            }

            IEnumerable<Product> productList = await _productRepository.GetListAsync(predicate: o => o.AppliedCouponCode.Equals(coupon.CouponCode));

            foreach(var product in productList)
            {
                product.AppliedCouponCode = string.Empty;
                product.DiscountAmount = 0;
                product.DiscountPercent = 0;

                _productRepository.Update(product);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}