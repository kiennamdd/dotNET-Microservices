using Order.Domain.Entities;

namespace Order.Application.Common.Interfaces
{
    public interface IBuyerRepository: IRepositoryBase<Buyer, int>
    {
        Task<Buyer?> GetByUserIdAsync(Guid userId);
    }
}