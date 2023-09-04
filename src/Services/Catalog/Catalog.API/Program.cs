using System.Reflection;
using Catalog.API.Data;
using Catalog.API.Data.Interceptors;
using Catalog.API.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using FluentValidation;
using Catalog.API.Middlewares;
using Catalog.API.Repositories;
using Catalog.API.Services;
using Catalog.API.Interfaces.Infrastructure;
using Catalog.API.Services.Infrastructure;
using Catalog.API.Handlers;
using Catalog.API.Domain.Constants;
using Catalog.API.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using MassTransit;
using Catalog.API.EventConsumers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<ExceptionHandlingMiddleware>();
builder.Services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();

builder.Services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddScoped<IDiscountService, DiscountService>();

builder.Services.AddDbContext<ApplicationDbContext>((s, options)=>
{
    options.AddInterceptors(s.GetServices<ISaveChangesInterceptor>());
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddScoped<ApplicationDbContextInitialiser>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthenticationHttpClientHandler>();

builder.Services.AddHttpClient(ApiServices.DiscountApi, config => 
{
    config.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ServiceApiUrls:DiscountApi"));
}).AddHttpMessageHandler(s => s.GetRequiredService<AuthenticationHttpClientHandler>());


builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

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

builder.Services.AddMassTransit(massTransitConfig => 
{
    massTransitConfig.AddConsumer<CouponDeletedEventConsumer>();

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

        rabbitmqConfig.ReceiveEndpoint("coupon-deleted-event-queue", endpoint => 
        {
            endpoint.ConfigureConsumer<CouponDeletedEventConsumer>(context);
        });
    });
});

builder.Services.AddAppAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.InitialiseDatabaseAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseStaticFiles();

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
