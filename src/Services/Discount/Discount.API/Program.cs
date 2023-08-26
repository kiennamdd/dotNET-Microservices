using Discount.API.Data;
using Discount.API.Interfaces;
using Discount.API.Middlewares;
using Discount.API.Repositories;
using Discount.API.Services;
using Discount.API.Validators;
using FluentValidation;
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
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    await app.SeedDatabase();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

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
