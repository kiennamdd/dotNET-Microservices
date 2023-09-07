
using Microsoft.EntityFrameworkCore;

namespace Cart.API.Data
{
    public static class InitialiserExtensions
    {
        public static async Task<WebApplication> InitialiseDatabaseAsync(this WebApplication app)
        {
            using(var scope = app.Services.CreateScope())
            {
                var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

                await initialiser.MigrateDatabaseAsync();
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
    }
}