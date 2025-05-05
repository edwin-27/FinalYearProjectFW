using Microsoft.AspNetCore.Identity.UI.Services;

namespace FinalYearProject.Support
{
    public class DummyEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // this Does nothing, just simulate email sending
            return Task.CompletedTask;
        }

    }
}
