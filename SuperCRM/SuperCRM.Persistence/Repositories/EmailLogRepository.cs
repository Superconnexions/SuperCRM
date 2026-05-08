using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    public class EmailLogRepository : IEmailLogRepository
    {
        private readonly SuperCrmDbContext _context;

        public EmailLogRepository(SuperCrmDbContext context)
        {
            _context = context;
        }

        public Task<List<EmailLog>> GetRecentAsync(int take = 100, CancellationToken cancellationToken = default)
        {
            return _context.EmailLogs
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAt)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(EmailLog entity, CancellationToken cancellationToken = default)
        {
            await _context.EmailLogs.AddAsync(entity, cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
