
using Order.Domain.Entities;

namespace Order.Application.Common.Interfaces
{
    public interface IOrderRepository: IRepositoryBase<CustomerOrder, Guid>
    {
        
    }
}