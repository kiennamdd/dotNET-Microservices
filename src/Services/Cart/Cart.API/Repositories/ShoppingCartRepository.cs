
using Cart.API.Data;
using Cart.API.Domain.Entities;
using Cart.API.Interfaces;

namespace Cart.API.Repositories
{
    public class ShoppingCartRepository: RepositoryBase<ShoppingCart, Guid>, IShoppingCartRepository
    {
        public ShoppingCartRepository(ApplicationDbContext db)
            : base(db)
        {
            //
        }
    }
}