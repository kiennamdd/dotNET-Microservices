
using MimeKit;

namespace Email.API.Models
{
    public class MailData
    {
        public List<string> To { get; }

        public string Subject { get; }

        public string? Body { get; }

        public string? From { get; }

        public string? DisplayName { get; }

        public MailData(List<string> to, string subject, string? body = null, string? from = null, string? displayName = null)
        {
            To = to;

            Subject = subject;
            Body = body;

            From = from;
            DisplayName = displayName;
        }
    }
}