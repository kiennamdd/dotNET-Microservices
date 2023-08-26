
using System.Reflection;
using Catalog.API.Domain.Entities;
using Catalog.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Catalog.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {
            //
        }

        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Brand> Brands => Set<Brand>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                        .HasMany(o => o.Products)
                        .WithOne(o => o.Category)
                        .HasForeignKey(o => o.CategoryId);

            modelBuilder.Entity<Brand>()
                        .HasMany(o => o.Products)
                        .WithOne(o => o.Brand)
                        .HasForeignKey(o => o.BrandId);

            modelBuilder.Entity<Product>()
                        .HasMany(o => o.ProductImages)
                        .WithOne(o => o.Product)
                        .HasForeignKey(o => o.ProductId);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
    }
}