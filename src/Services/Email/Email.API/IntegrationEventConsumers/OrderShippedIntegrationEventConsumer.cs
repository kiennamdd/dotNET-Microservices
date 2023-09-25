using System.Text;
using Email.API.Interfaces;
using Email.API.Models;
using EventBus.Events;
using MassTransit;

namespace Email.API.IntegrationEventConsumers
{
    public class OrderShippedIntegrationEventConsumer : IConsumer<OrderShippedIntegrationEvent>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<OrderCancelledIntegrationEventConsumer> _logger;

        public OrderShippedIntegrationEventConsumer(IEmailService emailService, ILogger<OrderCancelledIntegrationEventConsumer> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderShippedIntegrationEvent> context)
        {
            var message = context.Message;

            if(string.IsNullOrEmpty(message.UserEmail))
            {
                _logger.LogError($"User's email is null or empty. Integration event ID: {message.Id}. Order ID: {message.OrderId}.");
                return;
            }

            var body = new StringBuilder();
            body.AppendLine("<html>");
            body.AppendLine("<body>");
            body.AppendLine($"<h3>Your order has been shipped. Order ID: {message.OrderId}</h3>");
            // todo: Give customer a link to rate the order
            body.AppendLine("</body>");
            body.AppendLine("</html>");

            var to = new List<string>
            {
                message.UserEmail
            };

            var mailData = new MailData(to, "Order Shipped Successfully!", body.ToString(), true);
            await _emailService.SendAsync(mailData);
        }
    }
}