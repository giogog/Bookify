using Contracts;
using Domain.Exception;
using FluentEmail.Core;

namespace Infrastructure.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly IFluentEmail _fluentEmail;

        public EmailSender(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public async Task<EmailResult> SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var response = await _fluentEmail
                    .To(email)
                    .Subject(subject)
                    .Body(htmlMessage, isHtml: true)
                    .SendAsync();

                return new EmailResult { IsSuccess = response.Successful, ErrorMessage = response.ErrorMessages?.FirstOrDefault() };
            }
            catch (Exception ex)
            {
                return new EmailResult { IsSuccess = false, ErrorMessage = ex.Message };
            }
        }
    }
}
