using Catalog.API.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Configuration
            .AddJsonFile($"ocelot.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
            
builder.Services.AddOcelot();

var app = builder.Build();

app.UseRouting();
app.UseEndpoints(endpoints => 
{
    endpoints.MapControllers();

    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("DotNet microservices - API Gateway with Ocelot!");
    });
});

await app.UseOcelot();

app.Run();
