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
            IEnumerable<CustomerOrder> list = new List<CustomerOrder>();
            

            if(request.UserId != Guid.Empty)
            {
                Buyer? buyer = await _buyerRepository.GetByUserIdAsync(request.UserId);

                if(buyer != null)
                    list = await _orderRepository.GetListAsync(o => o.BuyerId == buyer.Id, includeProperties: request.IncludesProperties);
            }
            else // If userId is not assigned value, return all order (for admin user purposes)
            {
                list = await _orderRepository.GetListAsync(includeProperties: request.IncludesProperties);
            }

            return list;
        }
    }
}