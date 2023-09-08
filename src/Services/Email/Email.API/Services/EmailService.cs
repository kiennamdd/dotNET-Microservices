

using Email.API.Interfaces;
using Email.API.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Email.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<MailSettings> options, ILogger<EmailService> logger)
        {
            _mailSettings = options.Value;    
            _logger = logger;
        }

        public async Task<bool> SendAsync(MailData mailData)
        {
            try
            {
                var emailMessage = new MimeMessage();

                var emailFrom = new MailboxAddress(mailData.DisplayName ?? _mailSettings.SenderName, mailData.From ?? _mailSettings.SenderEmail);
                emailMessage.From.Add(emailFrom);

                foreach(string toAddress in mailData.To)
                {
                    emailMessage.To.Add(MailboxAddress.Parse(toAddress));
                }

                emailMessage.Subject = mailData.Subject;

                var bodyBuilder = new BodyBuilder();

                if(mailData.IsHtmlBody)
                    bodyBuilder.HtmlBody = mailData.Body;
                else
                    bodyBuilder.TextBody = mailData.Body;

                emailMessage.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                await client.ConnectAsync(_mailSettings.Server, _mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_mailSettings.Username, _mailSettings.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);

                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending email.");

                return false;
            }
        }
    }
}