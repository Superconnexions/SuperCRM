using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.DTOs.Common;
using SuperCRM.Application.DTOs.EmailSettings;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    public class EmailLogRepository : IEmailLogRepository
    {
        private readonly SuperCrmDbContext _dbContext;
        
        public EmailLogRepository(SuperCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(EmailLog entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.EmailLogs.AddAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(EmailLog entity, CancellationToken cancellationToken = default)
        {
            _dbContext.EmailLogs.Update(entity);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<EmailLog?> GetByIdAsync(Guid emailLogId, CancellationToken cancellationToken = default)
        {
            return _dbContext.EmailLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.EmailLogId == emailLogId, cancellationToken);
        }

        public async Task<PagedResultDto<EmailLogListDto>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            bool? isSent,
            string? searchText,
            CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = _dbContext.EmailLogs.AsNoTracking().AsQueryable();

            if (isSent.HasValue)
                query = query.Where(x => x.IsSent == isSent.Value);

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                var text = searchText.Trim();
                query = query.Where(x =>
                    x.ToEmail.Contains(text) ||
                    (x.CcEmail != null && x.CcEmail.Contains(text)) ||
                    (x.BccEmail != null && x.BccEmail.Contains(text)) ||
                    x.Subject.Contains(text) ||
                    (x.SourceModule != null && x.SourceModule.Contains(text)) ||
                    (x.ErrorMessage != null && x.ErrorMessage.Contains(text)));
            }

            var totalRecords = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new EmailLogListDto
                {
                    EmailLogId = x.EmailLogId,
                    ToEmail = x.ToEmail,
                    CcEmail = x.CcEmail,
                    BccEmail = x.BccEmail,
                    Subject = x.Subject,
                    IsHtml = x.IsHtml,
                    IsSent = x.IsSent,
                    SentAt = x.SentAt,
                    ErrorMessage = x.ErrorMessage,
                    SourceModule = x.SourceModule,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync(cancellationToken);

            return new PagedResultDto<EmailLogListDto>
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }
    }
}
