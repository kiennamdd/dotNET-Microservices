using Cart.API.Data;
using Cart.API.Domain.Entities;
using Cart.API.Interfaces;

namespace Cart.API.Repositories
{
    public class CartItemRepository: RepositoryBase<CartItem, int>, ICartItemRepository
    {
        public CartItemRepository(ApplicationDbContext db)
            : base(db)
        {
            //
        }
    }
}