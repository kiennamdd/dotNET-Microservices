using System.Reflection;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Common.Behaviors;

namespace Order.Application
{
    public static class ConfigureServices
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddMediatR(config => 
            {
                config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            });

            services.AddMassTransit(massTransitConfig => 
            {
                massTransitConfig.UsingRabbitMq((context, rabbitmqConfig) => 
                {
                    var rabbitMqSettings = configuration.GetSection("RabbitMqSettings");
                    string host = rabbitMqSettings.GetValue<string>("Host") ?? "";
                    ushort port = rabbitMqSettings.GetValue<ushort>("Port");
                    string user = rabbitMqSettings.GetValue<string>("User") ?? "";
                    string password = rabbitMqSettings.GetValue<string>("Password") ?? "";

                    rabbitmqConfig.Host(host: host, port: port, virtualHost: "/", hostConfig => 
                    {
                        hostConfig.Username(user);
                        hostConfig.Password(password);
                    });
                });
            });

            return services;
        }
    }
}