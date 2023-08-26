using Catalog.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.API.Data.Config
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.Property(o => o.ProductId).IsRequired();

            builder.Property(o => o.ImageUrl).IsRequired();

            builder.Property(o => o.ImageLocalPath).IsRequired();
        }
    }
}