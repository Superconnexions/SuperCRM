using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    /// <summary>
    /// EF Core repository for Product Variant master setup.
    /// </summary>
    public class ProductVariantRepository : IProductVariantRepository
    {
        private readonly SuperCrmDbContext _dbContext;

        public ProductVariantRepository(SuperCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductVariant>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants
                .AsNoTracking()
                .Include(x => x.Product)
                .Include(x => x.VariantType)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProductVariant?> GetByIdAsync(Guid productVariantId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants
                .Include(x => x.Product)
                .Include(x => x.VariantType)
                .FirstOrDefaultAsync(x => x.ProductVariantId == productVariantId, cancellationToken);
        }

        public async Task<bool> ExistsByProductAndCodeAsync(Guid productId, string variantCode, Guid? excludeProductVariantId = null, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariants.AnyAsync(x =>
                x.ProductId == productId
                && x.VariantCode == variantCode
                && (!excludeProductVariantId.HasValue || x.ProductVariantId != excludeProductVariantId.Value),
                cancellationToken);
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.ProductId == productId, cancellationToken);
        }

        public async Task<ProductVariantType?> GetVariantTypeByCodeAsync(string typeCode, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariantTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.TypeCode == typeCode, cancellationToken);
        }

        public async Task<List<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .AsNoTracking()
                .Where(x => x.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ProductVariantType>> GetActiveVariantTypesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductVariantTypes
                .AsNoTracking()
                .Where(x => x.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(ProductVariant entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.ProductVariants.AddAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(ProductVariant entity, CancellationToken cancellationToken = default)
        {
            _dbContext.ProductVariants.Update(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
