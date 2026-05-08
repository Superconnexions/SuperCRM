using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    public interface IEmailLogRepository
    {
        Task<List<EmailLog>> GetRecentAsync(int take = 100, CancellationToken cancellationToken = default);
        Task AddAsync(EmailLog entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
