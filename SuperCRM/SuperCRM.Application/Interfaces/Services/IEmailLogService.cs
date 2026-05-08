using SuperCRM.Application.DTOs.EmailSettings;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface IEmailLogService
    {
        Task<List<EmailLogDto>> GetRecentAsync(int take = 100, CancellationToken cancellationToken = default);
    }
}
