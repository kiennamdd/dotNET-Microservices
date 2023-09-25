using MediatR;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;

namespace Order.Application.Features.Orders.Queries
{
    public class GetOrderDetailsQueryHandler : IRequestHandler<GetOrderDetailsQuery, CustomerOrder?>
    {
        private readonly IOrderRepository _orderRepository;

        public GetOrderDetailsQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;   
        }

        public async Task<CustomerOrder?> Handle(GetOrderDetailsQuery request, CancellationToken cancellationToken)
        {
            string[] includeProperties = { 
                nameof(CustomerOrder.Buyer), 
                nameof(CustomerOrder.Address),
                nameof(CustomerOrder.Items)
            };

            var list = await _orderRepository.GetListAsync(predicate: o => o.Id == request.OrderId, 
                                                includeProperties: string.Join(",", includeProperties));

            CustomerOrder? order = list.SingleOrDefault();

            return order;
        }
    }
}