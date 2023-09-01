using Email.API.IntegrationEventConsumers;
using Email.API.Interfaces;
using Email.API.Models;
using Email.API.Services;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(massTransitConfig => 
{
    massTransitConfig.AddConsumer<UserCreatedEventConsumer>();

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

        rabbitmqConfig.ReceiveEndpoint("user-created-event-queue", endpoint => 
        {
            endpoint.ConfigureConsumer<UserCreatedEventConsumer>(context);
        });
    });
});

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

//app.UseAuthorization();

app.UseEndpoints(endpoints => 
{
    endpoints.MapGet("/", (context) => 
    {
        context.Response.Redirect("/swagger/index.html");
        return Task.CompletedTask;
    });
    endpoints.MapControllers();
});

app.MapControllers();

app.Run();
