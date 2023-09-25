using Cart.API.Interfaces;
using EventBus.Events;
using MassTransit;

namespace Cart.API.IntegrationEventConsumers
{
    public class OrderStartedIntegrationEventConsumer : IConsumer<OrderStartedIntegrationEvent>
    {
        private readonly IShoppingCartRepository _cartRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderStartedIntegrationEventConsumer(IShoppingCartRepository cartRepository, IUnitOfWork unitOfWork)
        {
            _cartRepository = cartRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<OrderStartedIntegrationEvent> context)
        {
            var integrationEvent = context.Message;

            var list = await _cartRepository.GetListAsync(o => o.Id == integrationEvent.UserId);
            var cart = list.FirstOrDefault();
            if(cart != null)
            {
                _cartRepository.Delete(cart);
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }
}