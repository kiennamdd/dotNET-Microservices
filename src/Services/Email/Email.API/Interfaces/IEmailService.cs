
using Email.API.Models;

namespace Email.API.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendAsync(MailData mailData);
    }
}