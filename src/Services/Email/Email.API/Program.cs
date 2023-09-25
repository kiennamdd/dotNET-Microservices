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
    massTransitConfig.AddConsumer<UserCreatedIntegrationEventConsumer>();
    massTransitConfig.AddConsumer<OrderCancelledIntegrationEventConsumer>();
    massTransitConfig.AddConsumer<OrderPaidIntegrationEventConsumer>();
    massTransitConfig.AddConsumer<OrderShippedIntegrationEventConsumer>();
    massTransitConfig.AddConsumer<OrderRefundedIntegrationEventConsumer>();

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

        rabbitmqConfig.ReceiveEndpoint("EmailAPI_UserCreatedIntegrationEvent_Queue", endpoint => 
        {
            endpoint.ConfigureConsumer<UserCreatedIntegrationEventConsumer>(context);
        });

        rabbitmqConfig.ReceiveEndpoint("EmailAPI_OrderCancelledIntegrationEvent_Queue", endpoint => 
        {
            endpoint.ConfigureConsumer<OrderCancelledIntegrationEventConsumer>(context);
        });

        rabbitmqConfig.ReceiveEndpoint("EmailAPI_OrderPaidIntegrationEvent_Queue", endpoint => 
        {
            endpoint.ConfigureConsumer<OrderPaidIntegrationEventConsumer>(context);
        });

        rabbitmqConfig.ReceiveEndpoint("EmailAPI_OrderShippedIntegrationEvent_Queue", endpoint => 
        {
            endpoint.ConfigureConsumer<OrderShippedIntegrationEventConsumer>(context);
        });

        rabbitmqConfig.ReceiveEndpoint("EmailAPI_OrderRefundedIntegrationEvent_Queue", endpoint => 
        {
            endpoint.ConfigureConsumer<OrderRefundedIntegrationEventConsumer>(context);
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
