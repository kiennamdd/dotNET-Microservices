
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Infrastructure.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {
            //
        }

        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Buyer> Buyers => Set<Buyer>();
        public DbSet<CustomerOrder> Orders => Set<CustomerOrder>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Buyer>()
                        .HasMany(o => o.Addresses)
                        .WithOne()
                        .HasForeignKey(o => o.BuyerId);

            modelBuilder.Entity<CustomerOrder>()
                        .HasOne(o => o.Buyer)
                        .WithMany()
                        .HasForeignKey(o => o.BuyerId);  

            modelBuilder.Entity<CustomerOrder>()
                        .HasOne(o => o.Address)
                        .WithMany()
                        .HasForeignKey(o => o.AddressId);   

            modelBuilder.Entity<CustomerOrder>()
                        .HasMany(o => o.Items)
                        .WithOne()
                        .HasForeignKey(o => o.OrderId);

            base.OnModelCreating(modelBuilder);
        }
    }
}