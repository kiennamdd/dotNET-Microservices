using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Infrastructure.Data.Configs
{
    public class OrderConfiguration : IEntityTypeConfiguration<CustomerOrder>
    {
        public void Configure(EntityTypeBuilder<CustomerOrder> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Status).HasConversion(orderStatus => orderStatus.Name, 
                                                        statusName => Enumeration.FromDisplayName<OrderStatus>(statusName));
        }
    }
}