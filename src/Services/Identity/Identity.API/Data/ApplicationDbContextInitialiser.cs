
using Identity.API.Domain.Constants;
using Identity.API.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Identity.API.Data
{
    public static class InitialiserExtensions
    {
        public static async Task<WebApplication> InitialiseDatabaseAsync(this WebApplication app)
        {
            using(var scope = app.Services.CreateScope())
            {
                var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

                await initialiser.MigrationAsync();
                await initialiser.SeedAsync();
            }

            return app;
        }
    }

    public class ApplicationDbContextInitialiser
    {
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger,
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task MigrationAsync()
        {
            try
            {
                await _db.Database.MigrateAsync();
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Can not migrating database");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                if(!_db.Roles.Any(o => o.Name == Roles.ADMIN))
                {
                    await _roleManager.CreateAsync(new ApplicationRole { Name = Roles.ADMIN });
                }

                var adminUser = new ApplicationUser
                {
                    Email = "namkienlua@gmail.com",
                    UserName = "namkienlua@gmail.com",
                    FullName = "Kien Nam",
                    PhoneNumber = "0901111111"
                };
                string adminPassword = "Admin1234@";
                if(!_db.Users.Any(o => o.Email == adminUser.Email))
                {
                    var result = await _userManager.CreateAsync(adminUser, adminPassword);
                    await _userManager.AddToRoleAsync(adminUser, Roles.ADMIN);
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Can not seeding database");
                throw;
            }
        }
    }
}