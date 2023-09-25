
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Common.Interfaces;
using Order.Application.Common.Interfaces.Infrastructure;
using Order.Infrastructure.Data;
using Order.Infrastructure.Data.Interceptors;
using Order.Infrastructure.Repositories;
using Order.Infrastructure.Services;
using Stripe;

namespace Order.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection ConfigureInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            StripeConfiguration.ApiKey = configuration.GetValue<string>("Stripe:ApiKey");

            services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
            services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventInterceptor>();
            services.AddDbContext<ApplicationDbContext>((s, options) => 
            {
                options.AddInterceptors(s.GetServices<ISaveChangesInterceptor>());
                options.UseNpgsql(configuration.GetConnectionString("Postgres"), npgsqlOptions => npgsqlOptions.MigrationsAssembly("Order.API"));
            });

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            services.AddScoped<ApplicationDbContextInitialiser>();

            services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IBuyerRepository, BuyerRepository>();
            services.AddScoped<IOrderItemRepository, OrderItemRepository>();

            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IStripeService, StripeService>();

            return services;
        }
    }
}