using System.Text;
using Email.API.Interfaces;
using Email.API.Models;
using EventBus.Events;
using MassTransit;

namespace Email.API.IntegrationEventConsumers
{
    public class UserCreatedIntegrationEventConsumer : IConsumer<UserCreatedIntegrationEvent>
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<UserCreatedIntegrationEventConsumer> _logger;

        public UserCreatedIntegrationEventConsumer(IEmailService emailService, 
            ILogger<UserCreatedIntegrationEventConsumer> logger)
        {
            _emailService = emailService;   
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<UserCreatedIntegrationEvent> context)
        {
            UserCreatedIntegrationEvent userInfo = context.Message;

            if(string.IsNullOrEmpty(userInfo.Email))
            {
                _logger.LogError($"User's email is null or empty. User ID: {userInfo.Id}");
                return;
            }

            var body = new StringBuilder();
            body.AppendLine("<html>");
            body.AppendLine("<body>");
            body.AppendLine($"<h3>Hello {userInfo.FullName}, you are our new customer!</h3>");
            body.AppendLine("<br/>");
            body.AppendLine("<p>You can now place order on website.</p>");
            body.AppendLine("<br/>");
            body.AppendLine($"<button style=\"background-color: red;\"><a style=\"color: white;\" href=\"http://localhost:8005\">Shop NOW!</a></button>");
            body.AppendLine("</body>");
            body.AppendLine("</html>");

            var to = new List<string>
            {
                userInfo.Email
            };

            var mailData = new MailData(to, "User Registration Successfully!", body.ToString(), true);
            await _emailService.SendAsync(mailData);
        }
    }
}