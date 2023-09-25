using Order.Application.Common.Interfaces;
using Order.Domain.Entities;
using Order.Infrastructure.Data;

namespace Order.Infrastructure.Repositories
{
    public class OrderItemRepository : RepositoryBase<OrderItem, int>, IOrderItemRepository
    {
        public OrderItemRepository(ApplicationDbContext db) : base(db)
        {
        }
    }
}