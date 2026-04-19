
using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    public class ProductImageRepository : IProductImageRepository
    {
        private readonly SuperCrmDbContext _dbContext;

        public ProductImageRepository(SuperCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ProductImage>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.ProductImages.AsNoTracking().Where(x => x.ProductId == productId).ToListAsync(cancellationToken);
        }

        public async Task AddAsync(ProductImage entity, CancellationToken cancellationToken = default)
        {
            await _dbContext.ProductImages.AddAsync(entity, cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
