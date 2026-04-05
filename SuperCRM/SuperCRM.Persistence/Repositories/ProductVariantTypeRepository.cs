using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    public class ProductVariantTypeRepository : IProductVariantTypeRepository
    {
        private readonly SuperCrmDbContext _dbContext;

        public ProductVariantTypeRepository(SuperCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductVariantType>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariantTypes
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<ProductVariantType?> GetByTypeCodeAsync(string typeCode, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariantTypes
                .FirstOrDefaultAsync(x => x.TypeCode == typeCode, cancellationToken);
        }

        public async Task<bool> ExistsByTypeCodeAsync(string typeCode, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariantTypes
                .AnyAsync(x => x.TypeCode == typeCode, cancellationToken);
        }

        public async Task AddAsync(ProductVariantType entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.ProductVariantTypes.AddAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(ProductVariantType entity, CancellationToken cancellationToken = default)
        {
            _dbContext.ProductVariantTypes.Update(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}