
using Order.Domain.Entities;

namespace Order.Application.Common.Interfaces
{
    public interface IOrderItemRepository: IRepositoryBase<OrderItem, int>
    {
        
    }
}