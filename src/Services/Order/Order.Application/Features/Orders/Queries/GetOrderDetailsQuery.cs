using MediatR;
using Order.Domain.Entities;

namespace Order.Application.Features.Orders.Queries
{
    public class GetOrderDetailsQuery: IRequest<CustomerOrder?>
    {
        public Guid OrderId { get; private set; }
        
        public GetOrderDetailsQuery(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}