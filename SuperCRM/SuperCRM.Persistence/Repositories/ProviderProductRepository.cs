using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    /// <summary>
    /// EF Core repository for ProviderProducts mapping.
    /// </summary>
    public class ProviderProductRepository : IProviderProductRepository
    {
        private readonly SuperCrmDbContext _dbContext;

        public ProviderProductRepository(SuperCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProviderProduct>> SearchAsync(string? providerName, string? productName, CancellationToken cancellationToken = default)
        {
            var query = _dbContext.ProviderProducts
                .AsNoTracking()
                .Include(x => x.Provider)
                .Include(x => x.Product)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(providerName))
            {
                var providerFilter = providerName.Trim();
                query = query.Where(x => x.Provider != null && x.Provider.ProviderName.Contains(providerFilter));
            }

            if (!string.IsNullOrWhiteSpace(productName))
            {
                var productFilter = productName.Trim();
                query = query.Where(x =>
                    x.ProductName.Contains(productFilter) ||
                    x.ProductCode.Contains(productFilter) ||
                    (x.Product != null && x.Product.ProductName.Contains(productFilter)) ||
                    (x.Product != null && x.Product.ProductCode.Contains(productFilter)));
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<List<ProviderProduct>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProviderProducts
                .AsNoTracking()
                .Include(x => x.Provider)
                .Include(x => x.Product)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProviderProduct?> GetByIdAsync(Guid providerProductId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProviderProducts
                .Include(x => x.Provider)
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.ProviderProductId == providerProductId, cancellationToken);
        }

        public async Task<bool> ExistsAsync(Guid providerId, Guid productId, Guid? excludeProviderProductId = null, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProviderProducts
                .AsNoTracking()
                .AnyAsync(x => x.ProviderId == providerId && x.ProductId == productId &&
                               (!excludeProviderProductId.HasValue || x.ProviderProductId != excludeProviderProductId.Value),
                    cancellationToken);
        }

        public async Task<Provider?> GetProviderByIdAsync(Guid providerId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Providers.AsNoTracking().FirstOrDefaultAsync(x => x.ProviderId == providerId, cancellationToken);
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products.AsNoTracking().FirstOrDefaultAsync(x => x.ProductId == productId, cancellationToken);
        }

        public async Task<List<Provider>> GetActiveProvidersAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Providers.AsNoTracking().Where(x => x.IsActive).ToListAsync(cancellationToken);
        }

        public async Task<List<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products.AsNoTracking().Where(x => x.IsActive).ToListAsync(cancellationToken);
        }

        public async Task AddAsync(ProviderProduct entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.ProviderProducts.AddAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(ProviderProduct entity, CancellationToken cancellationToken = default)
        {
            _dbContext.ProviderProducts.Update(entity);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(ProviderProduct entity, CancellationToken cancellationToken = default)
        {
            _dbContext.ProviderProducts.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
