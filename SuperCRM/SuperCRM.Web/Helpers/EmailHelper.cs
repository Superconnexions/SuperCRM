using SuperCRM.Application.DTOs.EmailSettings;
using SuperCRM.Application.Interfaces.Services;

namespace SuperCRM.Web.Helpers
{
    /// <summary>
    /// Reusable Web-layer helper. Inject IEmailHelper into controllers/services in the Web project.
    /// </summary>
    public interface IEmailHelper
    {
        Task<SendEmailResultDto> SendAsync(string toEmail, string subject, string body, bool isHtml = true, string? sourceModule = null, Guid? userId = null, CancellationToken cancellationToken = default);
        Task<SendEmailResultDto> SendTestEmailAsync(string toEmail, Guid? userId = null, CancellationToken cancellationToken = default);
    }

    public class EmailHelper : IEmailHelper
    {
        private readonly IEmailSenderService _emailSenderService;

        public EmailHelper(IEmailSenderService emailSenderService)
        {
            _emailSenderService = emailSenderService;
        }

        public Task<SendEmailResultDto> SendAsync(string toEmail, string subject, string body, bool isHtml = true, string? sourceModule = null, Guid? userId = null, CancellationToken cancellationToken = default)
        {
            return _emailSenderService.SendAsync(new SendEmailRequestDto
            {
                ToEmail = toEmail,
                Subject = subject,
                Body = body,
                IsHtml = isHtml,
                SourceModule = sourceModule,
                CreatedByUserId = userId
            }, cancellationToken);
        }

        public Task<SendEmailResultDto> SendTestEmailAsync(string toEmail, Guid? userId = null, CancellationToken cancellationToken = default)
        {
            return _emailSenderService.SendTestEmailAsync(toEmail, userId, cancellationToken);
        }
    }
}
