using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    public interface IEmailSettingRepository
    {
        Task<List<EmailSetting>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<EmailSetting?> GetByIdAsync(Guid emailSettingId, CancellationToken cancellationToken = default);
        Task<EmailSetting?> GetDefaultActiveAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsBySettingNameAsync(string settingName, Guid? excludeId = null, CancellationToken cancellationToken = default);
        Task AddAsync(EmailSetting entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(EmailSetting entity, CancellationToken cancellationToken = default);
        Task SetAllDefaultFalseAsync(CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
