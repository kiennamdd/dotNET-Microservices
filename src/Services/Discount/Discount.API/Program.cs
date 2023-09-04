using Discount.API.Data;
using Discount.API.Extensions;
using Discount.API.Interfaces;
using Discount.API.Middlewares;
using Discount.API.Repositories;
using Discount.API.Services;
using Discount.API.Validators;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Stripe;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
StripeConfiguration.ApiKey = builder.Configuration.GetValue<string>("Stripe:ApiKey");


builder.Services.AddSingleton<IApplicationDbContext, ApplicationDbContext>();
builder.Services.AddSingleton<IStripeService, StripeService>();
builder.Services.AddSingleton<ExceptionHandlingMiddleware>();

builder.Services.AddScoped(typeof(IRepositoryBase<,>), typeof(RepositoryBase<,>));
builder.Services.AddScoped<ICouponRepository, CouponRepository>();

builder.Services.AddScoped<ApplicationDbContextInitialiser>();

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

builder.Services.AddAppAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddMassTransit(massTransitConfig => 
{
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
