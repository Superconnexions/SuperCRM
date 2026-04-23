using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    /// <summary>
    /// EF Core repository for Sales Unit master setup.
    /// </summary>
    public class SalesUnitRepository : ISalesUnitRepository
    {
        private readonly SuperCrmDbContext _dbContext;

        public SalesUnitRepository(SuperCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SalesUnit>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SalesUnits
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<SalesUnit?> GetByIdAsync(int salesUnitId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.SalesUnits
                .FirstOrDefaultAsync(x => x.SalesUnitId == salesUnitId, cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(string unitCode, int? excludeSalesUnitId = null, CancellationToken cancellationToken = default)
        {
            return await _dbContext.SalesUnits
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UnitCode == unitCode &&
                    (!excludeSalesUnitId.HasValue || x.SalesUnitId != excludeSalesUnitId.Value),
                    cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string unitName, int? excludeSalesUnitId = null, CancellationToken cancellationToken = default)
        {
            return await _dbContext.SalesUnits
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UnitName == unitName &&
                    (!excludeSalesUnitId.HasValue || x.SalesUnitId != excludeSalesUnitId.Value),
                    cancellationToken);
        }

        public async Task AddAsync(SalesUnit entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.SalesUnits.AddAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(SalesUnit entity, CancellationToken cancellationToken = default)
        {
            _dbContext.SalesUnits.Update(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
