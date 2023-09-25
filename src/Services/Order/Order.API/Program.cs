using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Order.API.Extensions;
using Order.API.Middlewares;
using Order.API.Utils;
using Order.Application;
using Order.Application.Common.Interfaces;
using Order.Domain.Constants;
using Order.Infrastructure;
using Order.Infrastructure.Data;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddTokenAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.ConfigureApplicationServices(builder.Configuration);
builder.Services.ConfigureInfrastructureServices(builder.Configuration);

builder.Services.AddSingleton<ExceptionHandlingMiddleware>();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<HttpClientHandlingAuthentication>();
builder.Services.AddHttpClient(ApiServiceNames.CartApi, config => 
{
    config.BaseAddress = new Uri(builder.Configuration.GetValue<string>("ServiceApiUrls:CartApi"));
}).AddHttpMessageHandler(s => s.GetRequiredService<HttpClientHandlingAuthentication>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.Services.InitialiseDatabase();
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

app.Run();
