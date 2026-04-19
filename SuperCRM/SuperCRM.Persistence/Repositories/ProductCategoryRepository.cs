using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    /// <summary>
    /// EF Core repository for Product Category master setup.
    /// </summary>
    public class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly SuperCrmDbContext _dbContext;

        public ProductCategoryRepository(SuperCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductCategory>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductCategories
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<ProductCategory?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductCategories
                .FirstOrDefaultAsync(x => x.CategoryId == categoryId, cancellationToken);
        }

        public async Task<bool> ExistsByCodeAsync(string categoryCode, Guid? excludeCategoryId = null, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductCategories
                .AsNoTracking()
                .AnyAsync(x =>
                    x.CategoryCode == categoryCode &&
                    (!excludeCategoryId.HasValue || x.CategoryId != excludeCategoryId.Value),
                    cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string categoryName, Guid? excludeCategoryId = null, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductCategories
                .AsNoTracking()
                .AnyAsync(x =>
                    x.CategoryName == categoryName &&
                    (!excludeCategoryId.HasValue || x.CategoryId != excludeCategoryId.Value),
                    cancellationToken);
        }

        public async Task AddAsync(ProductCategory entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.ProductCategories.AddAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(ProductCategory entity, CancellationToken cancellationToken = default)
        {
            _dbContext.ProductCategories.Update(entity);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
