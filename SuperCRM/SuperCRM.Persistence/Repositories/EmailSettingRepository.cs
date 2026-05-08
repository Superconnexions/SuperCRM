using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    public class EmailSettingRepository : IEmailSettingRepository
    {
        private readonly SuperCrmDbContext _context;

        public EmailSettingRepository(SuperCrmDbContext context)
        {
            _context = context;
        }

        public Task<List<EmailSetting>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return _context.EmailSettings
                .AsNoTracking()
                .OrderByDescending(x => x.IsDefault)
                .ThenBy(x => x.SettingName)
                .ToListAsync(cancellationToken);
        }

        public Task<EmailSetting?> GetByIdAsync(Guid emailSettingId, CancellationToken cancellationToken = default)
        {
            return _context.EmailSettings.FirstOrDefaultAsync(x => x.EmailSettingId == emailSettingId, cancellationToken);
        }

        public Task<EmailSetting?> GetDefaultActiveAsync(CancellationToken cancellationToken = default)
        {
            return _context.EmailSettings
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<bool> ExistsBySettingNameAsync(string settingName, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            return _context.EmailSettings.AnyAsync(x =>
                x.SettingName == settingName &&
                (excludeId == null || x.EmailSettingId != excludeId.Value), cancellationToken);
        }

        public async Task AddAsync(EmailSetting entity, CancellationToken cancellationToken = default)
        {
            await _context.EmailSettings.AddAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(EmailSetting entity, CancellationToken cancellationToken = default)
        {
            _context.EmailSettings.Update(entity);
            return Task.CompletedTask;
        }

        public async Task SetAllDefaultFalseAsync(CancellationToken cancellationToken = default)
        {
            var items = await _context.EmailSettings.Where(x => x.IsDefault).ToListAsync(cancellationToken);
            foreach (var item in items)
                item.IsDefault = false;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
