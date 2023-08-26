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
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.InitialiseDatabase();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseStaticFiles();

app.UseAuthorization();

app.UseRouting();
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
