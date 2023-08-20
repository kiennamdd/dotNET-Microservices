using Discount.API.Domain.Entities;
using Discount.API.Interfaces;
using MongoDB.Driver;

namespace Discount.API.Data
{
    public class ApplicationDbContext: IApplicationDbContext
    {
        public ApplicationDbContext(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("MongoDb");
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(configuration.GetValue<string>("MongoDbSettings:DiscountDb"));

            Coupons = database.GetCollection<Coupon>(configuration.GetValue<string>("MongoDbSettings:CouponCollection"));
        }

        public IMongoCollection<Coupon> Coupons { get; set; }

        public IMongoCollection<T> GetCollection<T>()
        {
            IMongoCollection<T>? collection = null;

            foreach (var propertyInfo in GetType().GetProperties())
            {
                if(propertyInfo.PropertyType == typeof(IMongoCollection<T>))
                {
                    collection = propertyInfo.GetValue(this) as IMongoCollection<T>;
                    break;
                }
            }

            if(collection is null)
                throw new KeyNotFoundException($"Can not find any collection has entity with type: {typeof(T)}");

            return collection;
        }
    }
}
