using Discount.API.Domain.Entities;
using Discount.API.Interfaces;
using MongoDB.Driver;

namespace Discount.API.Data
{
    public static class InitialiserExtensions
    {
        public static async Task<WebApplication> InitialiseDatabaseAsync(this WebApplication app)
        {
            using(var scope = app.Services.CreateScope())
            {
                var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

                await initialiser.SeedAsync();
            }

            return app;
        }
    }

    public class ApplicationDbContextInitialiser
    {
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;
        private readonly IApplicationDbContext _db;
        private readonly IStripeService _stripeService;

        public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger,
            IApplicationDbContext db,
            IStripeService stripeService)
        {
            _logger = logger;
            _db = db;
            _stripeService = stripeService;
        }

        public async Task SeedAsync()
        {
            try
            {
                if (!_db.Coupons.Find(Builders<Coupon>.Filter.Empty).Any())
                {
                    var list = GetCouponSeedingList();

                    await _db.Coupons.InsertManyAsync(list);

                    foreach(var coupon in list)
                    {
                        var stripeCoupon = await _stripeService.FindCouponAsync(coupon.CouponCode);

                        if(stripeCoupon is null)
                        {
                            await _stripeService.CreateCouponAsync(coupon);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e, "Can not seeding database");
                throw;
            }
        }

        public IEnumerable<Coupon> GetCouponSeedingList()
        {
            IEnumerable<Coupon> list = new List<Coupon>
            {
                new Coupon
                {
                    CreatedBy = "seeder",
                    CreatedAt = DateTime.Now,
                    ModifiedBy = "seeder",
                    ModifiedAt = DateTime.Now,
                    CouponCode = "OFFA15",
                    MinOrderTotal = 100,
                    DiscountAmount = 15,
                    DiscountPercent = 0,
                    MaxDiscountAmount = 0,
                    ExpiredDate = DateTime.Now.AddDays(7)
                },
                new Coupon
                {
                    CreatedBy = "seeder",
                    CreatedAt = DateTime.Now,
                    ModifiedBy = "seeder",
                    ModifiedAt = DateTime.Now,
                    CouponCode = "OFFP20",
                    MinOrderTotal = 110,
                    DiscountAmount = 0,
                    DiscountPercent = 20,
                    MaxDiscountAmount = 15,
                    ExpiredDate = DateTime.Now.AddDays(3)
                },
                new Coupon
                {
                    CreatedBy = "seeder",
                    CreatedAt = DateTime.Now,
                    ModifiedBy = "seeder",
                    ModifiedAt = DateTime.Now,
                    CouponCode = "OFFA10",
                    MinOrderTotal = 70,
                    DiscountAmount = 10,
                    DiscountPercent = 0,
                    MaxDiscountAmount = 0,
                    ExpiredDate = DateTime.Now.AddDays(3)
                }
            };

            return list;
        }
    }
}
