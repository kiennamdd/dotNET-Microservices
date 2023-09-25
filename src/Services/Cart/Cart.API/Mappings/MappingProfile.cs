using AutoMapper;
using Cart.API.Domain.Entities;
using Cart.API.Models;

namespace Cart.API.Mappings
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<ShoppingCart, ShoppingCartDto>();

            CreateMap<CartItem, CartItemDto>();
        }
    }
}