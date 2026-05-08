using SuperCRM.Application.DTOs.Common;
using SuperCRM.Application.DTOs.EmailSettings;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    public interface IEmailLogRepository
    {
        Task AddAsync(EmailLog entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<EmailLog?> GetByIdAsync(Guid emailLogId, CancellationToken cancellationToken = default);
        Task<PagedResultDto<EmailLogListDto>> GetPagedAsync(int pageNumber, int pageSize, bool? isSent, string? searchText, CancellationToken cancellationToken = default);
    }
}
