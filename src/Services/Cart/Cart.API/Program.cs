using System.Reflection;
using Cart.API.Data;
using Cart.API.Domain.Constants;
using Cart.API.Interfaces;
using Cart.API.Interfaces.Infrastructure;
using Cart.API.Middlewares;
using Cart.API.Repositories;
using Cart.API.Services;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Cart.API.Extensions;
using Cart.API.IntegrationEventConsumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTokenAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddDbContext<ApplicationDbContext>(options => 
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => 
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            }, new string[]{}
        }
    });
});

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthenticationHttpClientHandler>();

builder.Services.AddHttpClient(ApiServiceNames.DiscountApi, config => 
{
    config.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiServiceBaseUrls:DiscountApi"));
}).AddHttpMessageHandler(s => s.GetRequiredService<AuthenticationHttpClientHandler>());

builder.Services.AddHttpClient(ApiServiceNames.CatalogApi, config => 
{
    config.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ApiServiceBaseUrls:CatalogApi"));
}).AddHttpMessageHandler(s => s.GetRequiredService<AuthenticationHttpClientHandler>());

builder.Services.AddSingleton<ExceptionHandlingMiddleware>();
builder.Services.AddScoped<ApplicationDbContextInitialiser>();

builder.Services.AddScoped<ICurrentUser, CurrentUser>();
builder.Services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<IDiscountService, DiscountService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();

builder.Services.AddMassTransit(massTransitConfig => 
{
    massTransitConfig.AddConsumer<CouponDeletedIntegrationEventConsumer>();
    massTransitConfig.AddConsumer<ProductCouponCodeChangedIntegrationEventConsumer>();
    massTransitConfig.AddConsumer<ProductPriceChangedIntegrationEventConsumer>();
    massTransitConfig.AddConsumer<ProductDeletedIntegrationEventConsumer>();
    massTransitConfig.AddConsumer<OrderStartedIntegrationEventConsumer>();

    massTransitConfig.UsingRabbitMq((context, rabbitmqConfig) => 
    {
        var rabbitMqSettings = builder.Configuration.GetSection("RabbitMqSettings");
        string host = rabbitMqSettings.GetValue<string>("Host") ?? "";
        ushort port = rabbitMqSettings.GetValue<ushort>("Port");
        string user = rabbitMqSettings.GetValue<string>("User") ?? "";
        string password = rabbitMqSettings.GetValue<string>("Password") ?? "";

        rabbitmqConfig.Host(host: host, port: port, virtualHost: "/", hostConfig => 
        {
            hostConfig.Username(user);
            hostConfig.Password(password);
        });

        rabbitmqConfig.ReceiveEndpoint("CartAPI_CouponDeletedIntegrationEvent_Queue", endpoint => 
        {
            endpoint.ConfigureConsumer<CouponDeletedIntegrationEventConsumer>(context);
        });

        rabbitmqConfig.ReceiveEndpoint("CartAPI_ProductCouponCodeChangedIntegrationEvent_Queue", endpoint => 
        {
            endpoint.ConfigureConsumer<ProductCouponCodeChangedIntegrationEventConsumer>(context);
        });

        rabbitmqConfig.ReceiveEndpoint("CartAPI_ProductPriceChangedIntegrationEvent_Queue", endpoint => 
        {
            endpoint.ConfigureConsumer<ProductPriceChangedIntegrationEventConsumer>(context);
        });

        rabbitmqConfig.ReceiveEndpoint("CartAPI_ProductDeletedIntegrationEvent_Queue", endpoint => 
        {
            endpoint.ConfigureConsumer<ProductDeletedIntegrationEventConsumer>(context);
        });

        rabbitmqConfig.ReceiveEndpoint("CartAPI_OrderStartedIntegrationEvent_Queue", endpoint => 
        {
            endpoint.ConfigureConsumer<OrderStartedIntegrationEventConsumer>(context);
        });
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.InitialiseDatabaseAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints => 
{
    endpoints.MapGet("/", (context) => 
    {
        context.Response.Redirect("/swagger/index.html");
        return Task.CompletedTask;
    });

    endpoints.MapControllers();
});

//app.MapControllers();

app.Run();
