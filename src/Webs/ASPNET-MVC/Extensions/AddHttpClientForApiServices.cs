using ASPNET_MVC.Constants;

namespace ASPNET_MVC.Extensions
{
    public static class AddHttpClientForApiServices
    {
        public static IServiceCollection AddApiServiceHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
            var apiServiceBaseUrls = configuration.GetSection("ApiServiceBaseUrls");

            services.AddHttpClient(ApiServiceNames.IdentityApi, config => 
            {
                config.BaseAddress = new Uri(apiServiceBaseUrls.GetValue<string>("IdentityApi"));
            });

            services.AddHttpClient(ApiServiceNames.DiscountApi, config => 
            {
                config.BaseAddress = new Uri(apiServiceBaseUrls.GetValue<string>("DiscountApi"));
            });

            services.AddHttpClient(ApiServiceNames.CatalogApi, config => 
            {
                config.BaseAddress = new Uri(apiServiceBaseUrls.GetValue<string>("CatalogApi"));
            });

            services.AddHttpClient(ApiServiceNames.CartApi, config => 
            {
                config.BaseAddress = new Uri(apiServiceBaseUrls.GetValue<string>("CartApi"));
            });

            services.AddHttpClient(ApiServiceNames.OrderApi, config => 
            {
                config.BaseAddress = new Uri(apiServiceBaseUrls.GetValue<string>("OrderApi"));
            });
            
            return services;
        }   
    }
}