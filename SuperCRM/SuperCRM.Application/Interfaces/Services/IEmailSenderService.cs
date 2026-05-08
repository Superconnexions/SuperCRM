using SuperCRM.Application.DTOs.EmailSettings;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface IEmailSenderService
    {
        Task<SendEmailResultDto> SendAsync(SendEmailRequestDto request, CancellationToken cancellationToken = default);
        Task<SendEmailResultDto> SendTestEmailAsync(string toEmail, Guid? requestedByUserId, CancellationToken cancellationToken = default);
    }
}
