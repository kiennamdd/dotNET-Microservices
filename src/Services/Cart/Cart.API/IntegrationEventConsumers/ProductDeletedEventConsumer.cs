using Cart.API.Interfaces;
using EventBus.Events;
using MassTransit;

namespace Cart.API.IntegrationEventConsumers
{
    public class ProductDeletedEventConsumer : IConsumer<ProductDeletedEvent>
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductDeletedEventConsumer(ICartItemRepository cartItemRepository, IUnitOfWork unitOfWork)
        {
            _cartItemRepository = cartItemRepository;    
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<ProductDeletedEvent> context)
        {
            ProductDeletedEvent productDeletedEvent = context.Message;

            var list = await  _cartItemRepository.GetListAsync(o => o.ProductId == productDeletedEvent.ProductId);

            foreach(var cartItem in list)
            {
                _cartItemRepository.Delete(cartItem);
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}