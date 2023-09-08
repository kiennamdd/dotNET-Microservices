
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

        public bool IsHtmlBody { get; }

        public MailData(List<string> to, 
                        string subject, 
                        string? body = null, 
                        bool isHtmlBody = false,
                        string? from = null, 
                        string? displayName = null)
        {
            To = to;

            Subject = subject;
            IsHtmlBody = isHtmlBody;
            Body = body;

            From = from;
            DisplayName = displayName;
        }
    }
}