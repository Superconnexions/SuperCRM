using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    /// <summary>
    /// Persistence contract for Sales Unit master setup.
    /// </summary>
    public interface ISalesUnitRepository
    {
        Task<List<SalesUnit>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<SalesUnit?> GetByIdAsync(int salesUnitId, CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(string unitCode, int? excludeSalesUnitId = null, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string unitName, int? excludeSalesUnitId = null, CancellationToken cancellationToken = default);
        Task AddAsync(SalesUnit entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(SalesUnit entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
