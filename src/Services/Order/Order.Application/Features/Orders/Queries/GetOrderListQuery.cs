
using System.Linq.Expressions;
using MediatR;
using Order.Domain.Entities;

namespace Order.Application.Features.Orders.Queries
{
    public class GetOrderListQuery: IRequest<IEnumerable<CustomerOrder>>
    {
        public Guid UserId { get; set; } = Guid.Empty;
        public string IncludesProperties { get; set; } = string.Empty;
        public Expression<Func<CustomerOrder, bool>>? Predicate { get; set; } = null;
        public Func<IQueryable<CustomerOrder>, IOrderedQueryable<CustomerOrder>>? OrderBy = null;

        public GetOrderListQuery()
        {
            
        }

        public GetOrderListQuery(Guid userId, Expression<Func<CustomerOrder, bool>>? predicate = null, 
                                            Func<IQueryable<CustomerOrder>, IOrderedQueryable<CustomerOrder>>? orderBy = null, 
                                            string includeProperties = "")
        {
            UserId = userId;
            Predicate = predicate;
            IncludesProperties = includeProperties;
        }
    }
}