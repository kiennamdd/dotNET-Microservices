using Catalog.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.API.Data.Config
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.Property(o => o.Id)
                    .UseIdentityColumn(1, 1);

            builder.Property(o => o.Name)
                    .IsRequired()
                    .HasMaxLength(50);
        }
    }
}