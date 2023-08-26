using Catalog.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.API.Data.Config
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(o => o.Name)

                    .IsRequired()
                    .HasMaxLength(150);

            builder.Property(o => o.Description)
                    .HasMaxLength(500);

            builder.Property(o => o.Origin)
                    .HasMaxLength(10);
        }
    }
}