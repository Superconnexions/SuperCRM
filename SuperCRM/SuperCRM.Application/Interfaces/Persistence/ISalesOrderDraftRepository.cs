using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    public interface ISalesOrderDraftRepository
    {
        Task<SalesOrderDraft?> GetByIdWithLinesAsync(Guid salesOrderDraftId, CancellationToken cancellationToken = default);
        Task<string> GenerateNextDraftNoAsync(CancellationToken cancellationToken = default);
        Task AddAsync(SalesOrderDraft entity, CancellationToken cancellationToken = default);
        void RemoveLines(IEnumerable<SalesOrderDraftLine> lines);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
