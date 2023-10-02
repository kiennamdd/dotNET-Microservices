using MediatR;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;

namespace Order.Application.Features.Orders.Queries
{
    public class GetOrderListQueryHandler : IRequestHandler<GetOrderListQuery, IEnumerable<CustomerOrder>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IBuyerRepository _buyerRepository;

        public GetOrderListQueryHandler(IOrderRepository orderRepository, IBuyerRepository buyerRepository)
        {
            _orderRepository = orderRepository;   
            _buyerRepository = buyerRepository; 
        }

        public async Task<IEnumerable<CustomerOrder>> Handle(GetOrderListQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<CustomerOrder> list = await _orderRepository.GetListAsync(predicate: request.Predicate, 
                                                                                    orderBy: request.OrderBy,
                                                                                    includeProperties: request.IncludesProperties);
            
            if(request.UserId != Guid.Empty)
            {
                Buyer? buyer = await _buyerRepository.GetByUserIdAsync(request.UserId);

                if(buyer != null)
                    list = list.Where(o => o.BuyerId == buyer.Id);
                else
                    return new List<CustomerOrder>();
            }

            // If userId is not assigned value, return all order (for admin user purposes)
            return list;
        }
    }
}