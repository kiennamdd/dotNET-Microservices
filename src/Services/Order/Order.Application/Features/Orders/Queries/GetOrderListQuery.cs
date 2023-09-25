
using MediatR;
using Order.Domain.Entities;

namespace Order.Application.Features.Orders.Queries
{
    public class GetOrderListQuery: IRequest<IEnumerable<CustomerOrder>>
    {
        public Guid UserId { get; set; } = Guid.Empty;
        public string IncludesProperties { get; set; } = string.Empty;
    }
}