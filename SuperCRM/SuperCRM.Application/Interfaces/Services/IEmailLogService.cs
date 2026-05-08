using SuperCRM.Application.DTOs.Common;
using SuperCRM.Application.DTOs.EmailSettings;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface IEmailLogService
    {
        Task<PagedResultDto<EmailLogListDto>> GetPagedAsync(int pageNumber, int pageSize, bool? isSent, string? searchText, CancellationToken cancellationToken = default);
        Task<EmailLogDetailsDto?> GetDetailsAsync(Guid emailLogId, CancellationToken cancellationToken = default);
        Task<(bool Success, string Message)> ResendFailedAsync(Guid emailLogId, Guid? requestedByUserId, CancellationToken cancellationToken = default);
    }
}
