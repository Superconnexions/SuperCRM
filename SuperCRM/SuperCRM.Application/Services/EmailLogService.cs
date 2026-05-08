using SuperCRM.Application.DTOs.Common;
using SuperCRM.Application.DTOs.EmailSettings;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;

namespace SuperCRM.Application.Services
{
    public class EmailLogService : IEmailLogService
    {
        private readonly IEmailLogRepository _emailLogRepository;
        private readonly IEmailSenderService _emailSenderService;

        public EmailLogService(IEmailLogRepository emailLogRepository, IEmailSenderService emailSenderService)
        {
            _emailLogRepository = emailLogRepository;
            _emailSenderService = emailSenderService;
        }

        public Task<PagedResultDto<EmailLogListDto>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            bool? isSent,
            string? searchText,
            CancellationToken cancellationToken = default)
        {
            return _emailLogRepository.GetPagedAsync(pageNumber, pageSize, isSent, searchText, cancellationToken);
        }

        public async Task<EmailLogDetailsDto?> GetDetailsAsync(Guid emailLogId, CancellationToken cancellationToken = default)
        {
            var log = await _emailLogRepository.GetByIdAsync(emailLogId, cancellationToken);
            if (log == null) return null;

            return new EmailLogDetailsDto
            {
                EmailLogId = log.EmailLogId,
                EmailSettingId = log.EmailSettingId,
                ToEmail = log.ToEmail,
                CcEmail = log.CcEmail,
                BccEmail = log.BccEmail,
                Subject = log.Subject,
                BodyPreview = log.BodyPreview,
                Body = log.Body,
                IsHtml = log.IsHtml,
                IsSent = log.IsSent,
                SentAt = log.SentAt,
                ErrorMessage = log.ErrorMessage,
                SourceModule = log.SourceModule,
                CreatedAt = log.CreatedAt
            };
        }

        public async Task<(bool Success, string Message)> ResendFailedAsync(Guid emailLogId, Guid? requestedByUserId, CancellationToken cancellationToken = default)
        {
            var log = await _emailLogRepository.GetByIdAsync(emailLogId, cancellationToken);
            if (log == null)
                return (false, "Email log not found.");

            if (log.IsSent)
                return (false, "Only failed emails can be resent.");

            var body = log.Body ?? log.BodyPreview ?? string.Empty;
            if (string.IsNullOrWhiteSpace(body))
                return (false, "Email body is empty. Resend cannot continue.");

            var result = await _emailSenderService.SendAsync(new SendEmailRequestDto
            {
                ToEmail = log.ToEmail,
                CcEmail = log.CcEmail,
                BccEmail = log.BccEmail,
                Subject = log.Subject,
                Body = body,
                IsHtml = log.IsHtml,
                SourceModule = "EmailLogs-Resend",
                CreatedByUserId = requestedByUserId
            }, cancellationToken);

            return (result.Success, result.Message);
        }
    }
}
