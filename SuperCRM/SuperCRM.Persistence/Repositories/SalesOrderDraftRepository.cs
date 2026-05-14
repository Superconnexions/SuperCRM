using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    public class SalesOrderDraftRepository : ISalesOrderDraftRepository
    {
        private readonly SuperCrmDbContext _dbContext;

        public SalesOrderDraftRepository(SuperCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<SalesOrderDraft?> GetByIdWithLinesAsync(Guid salesOrderDraftId, CancellationToken cancellationToken = default)
        {
            return _dbContext.SalesOrderDrafts
                .Include(x => x.DraftLines)
                .FirstOrDefaultAsync(x => x.SalesOrderDraftId == salesOrderDraftId, cancellationToken);
        }

        public async Task<string> GenerateNextDraftNoAsync(CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.ToString("yyyyMMdd");
            var prefix = $"DRAFT-{today}-";

            var countToday = await _dbContext.SalesOrderDrafts
                .CountAsync(x => x.DraftNo.StartsWith(prefix), cancellationToken);

            return prefix + (countToday + 1).ToString("0000");
        }

        public async Task AddAsync(SalesOrderDraft entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.SalesOrderDrafts.AddAsync(entity, cancellationToken);
        }

        public void RemoveLines(IEnumerable<SalesOrderDraftLine> lines)
        {
            _dbContext.SalesOrderDraftLines.RemoveRange(lines);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
