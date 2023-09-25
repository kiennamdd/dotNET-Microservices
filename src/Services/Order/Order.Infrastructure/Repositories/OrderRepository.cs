
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBase<CustomerOrder, Guid>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext db) : base(db)
        {
            //
        }

        
    }
}