using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    /// <summary>
    /// EF Core repository for Product and ProductImages.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly SuperCrmDbContext _dbContext;

        public ProductRepository(SuperCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .AsNoTracking()
                .Include(x => x.Category)
                .Include(x => x.SalesUnit)
                .Include(x => x.ProductImages)
                .ToListAsync(cancellationToken);
        }

        public async Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .Include(x => x.Category)
                .Include(x => x.SalesUnit)
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.ProductId == productId, cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(string productCode, Guid? excludeProductId = null, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Products
                .AsNoTracking()
                .AnyAsync(x =>
                    x.ProductCode == productCode &&
                    (!excludeProductId.HasValue || x.ProductId != excludeProductId.Value),
                    cancellationToken);
        }

        public async Task<ProductCategory?> GetCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductCategories
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CategoryId == categoryId, cancellationToken);
        }

        public async Task<SalesUnit?> GetSalesUnitByIdAsync(int salesUnitId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.SalesUnits
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SalesUnitId == salesUnitId, cancellationToken);
        }

        public async Task<List<ProductCategory>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductCategories
                .AsNoTracking()
                .Where(x => x.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<SalesUnit>> GetActiveSalesUnitsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SalesUnits
                .AsNoTracking()
                .Where(x => x.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Product entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.Products.AddAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(Product entity, CancellationToken cancellationToken = default)
        {
            _dbContext.Products.Update(entity);
            return Task.CompletedTask;
        }

        public async Task ReplaceImagesAsync(Guid productId, List<ProductImage> images, CancellationToken cancellationToken = default)
        {
            var existing = await _dbContext.ProductImages
                .Where(x => x.ProductId == productId)
                .ToListAsync(cancellationToken);

            if (existing.Count > 0)
                _dbContext.ProductImages.RemoveRange(existing);

            if (images.Count > 0)
                await _dbContext.ProductImages.AddRangeAsync(images, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
