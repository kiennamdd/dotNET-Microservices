using Catalog.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Data
{
    public static class InitialiserExtensions
    {
        public static async Task<WebApplication> InitialiseDatabaseAsync(this WebApplication app)
        {
            using(var scope = app.Services.CreateScope())
            {
                var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

                await initialiser.MigrateDatabaseAsync();
                await initialiser.SeedDatabaseAsync();
            }

            return app;
        }
    }

    public class ApplicationDbContextInitialiser
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;

        public ApplicationDbContextInitialiser(ApplicationDbContext db, 
            ILogger<ApplicationDbContextInitialiser> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task MigrateDatabaseAsync()
        {
            try
            {
                await _db.Database.MigrateAsync();
            }
            catch(Exception e)
            {
                _logger.LogError(e, "An error occurred while migrating database.");
                throw;
            }
        }

        public async Task SeedDatabaseAsync()
        {
            try
            {
                if(!_db.Categories.Any())
                {
                    await _db.Categories.AddRangeAsync(GetSeedingCategories());
                }

                if(!_db.Brands.Any())
                {
                    await _db.Brands.AddRangeAsync(GetSeedingBrands());
                }

                await _db.SaveChangesAsync();
            }
            catch(Exception e)
            {
                _logger.LogError(e, "An error occurred while seeding database.");
                throw;
            }
        }

        public IEnumerable<Product> GetSeedingProducts()
        {
            return new List<Product>
            {
            };
        }

        public IEnumerable<Brand> GetSeedingBrands()
        {
            return new List<Brand>
            {
                new Brand { Name = "VIt" },
                new Brand { Name = "Vincom" },
                new Brand { Name = "Vinamilk" },
            };
        }

        public IEnumerable<Category> GetSeedingCategories()
        {
            return new List<Category>
            {
                new Category { Name = "Ear phone" },
                new Category { Name = "Smart phone" },
                new Category { Name = "Milk" },
            };
        }
    }
}