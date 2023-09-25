using System.Text;
using Email.API.Interfaces;
using Email.API.Models;
using EventBus.Events;
using MassTransit;

namespace Email.API.IntegrationEventConsumers
{
    public class OrderRefundedIntegrationEventConsumer : IConsumer<OrderRefundedIntegrationEvent>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<OrderCancelledIntegrationEventConsumer> _logger;

        public OrderRefundedIntegrationEventConsumer(IEmailService emailService, ILogger<OrderCancelledIntegrationEventConsumer> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderRefundedIntegrationEvent> context)
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
            body.AppendLine($"<h3>Your order has been cancelled and refunded to your bank account. Order ID: {message.OrderId}</h3>");
            body.AppendLine("</body>");
            body.AppendLine("</html>");

            var to = new List<string>
            {
                message.UserEmail
            };

            var mailData = new MailData(to, "Order Refunded Successfully!", body.ToString(), true);
            await _emailService.SendAsync(mailData);
        }
    }
}