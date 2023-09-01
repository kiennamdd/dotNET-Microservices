
using Email.API.Interfaces;
using Email.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace Email.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController: ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;    
        }

        // For testing purpose
        [NonAction]
        [HttpPost]
        [Route("SendEmail")]
        public async Task<IActionResult> SendEmail(MailData mailData)
        {
            bool succeed = await _emailService.SendAsync(mailData);

            return Ok(new { Success = succeed });
        }
    }
}