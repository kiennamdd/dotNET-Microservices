
using Cart.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cart.API.Data.Configs
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.Property(o => o.Id).IsRequired();
            builder.Property(o => o.ShoppingCartId).IsRequired();
            builder.Property(o => o.ProductId).IsRequired();
            builder.Property(o => o.ProductName).IsRequired();
        }
    }
}