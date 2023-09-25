
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Order.Domain.Enums;

namespace Order.Infrastructure.Data
{
    public static class InitialiserExtensions
    {
        public static async Task InitialiseDatabase(this IServiceProvider services)
        {
            using(var scope = services.CreateScope())
            {
                var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

                await initialiser.MigrateAsync();
                await initialiser.SeedAsync();
            }
        }
    }

    public class ApplicationDbContextInitialiser
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;

        public ApplicationDbContextInitialiser(ApplicationDbContext db, ILogger<ApplicationDbContextInitialiser> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task MigrateAsync()
        {
            try
            {
                await _db.Database.MigrateAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while migrating database.");
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await Task.CompletedTask;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding database.");
            }
        }
    }
}