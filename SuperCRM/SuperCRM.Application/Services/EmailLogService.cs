using SuperCRM.Application.DTOs.EmailSettings;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;

namespace SuperCRM.Application.Services
{
    public class EmailLogService : IEmailLogService
    {
        private readonly IEmailLogRepository _repository;

        public EmailLogService(IEmailLogRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<EmailLogDto>> GetRecentAsync(int take = 100, CancellationToken cancellationToken = default)
        {
            var logs = await _repository.GetRecentAsync(take, cancellationToken);
            return logs.Select(x => new EmailLogDto
            {
                EmailLogId = x.EmailLogId,
                EmailSettingId = x.EmailSettingId,
                ToEmail = x.ToEmail,
                CcEmail = x.CcEmail,
                BccEmail = x.BccEmail,
                Subject = x.Subject,
                IsSent = x.IsSent,
                SentAt = x.SentAt,
                ErrorMessage = x.ErrorMessage,
                SourceModule = x.SourceModule,
                CreatedAt = x.CreatedAt
            }).ToList();
        }
    }
}
