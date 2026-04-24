using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.DTOs.ProductBaseCommissions;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    public class ProductBaseCommissionRepository : IProductBaseCommissionRepository
    {
        private readonly SuperCrmDbContext _context;

        public ProductBaseCommissionRepository(SuperCrmDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductBaseCommission>> SearchAsync(string? productKeyword, DateTime? effectiveFrom, DateTime? effectiveTo, bool includeInactive, CancellationToken cancellationToken = default)
        {
            var query = _context.ProductBaseCommissions
                .Include(x => x.Product)
                .AsQueryable();

            if (!includeInactive)
                query = query.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(productKeyword))
            {
                var keyword = productKeyword.Trim();
                query = query.Where(x => x.Product != null &&
                    (x.Product.ProductCode.Contains(keyword) || x.Product.ProductName.Contains(keyword)));
            }

            if (effectiveFrom.HasValue || effectiveTo.HasValue)
            {
                var from = effectiveFrom;
                var to = effectiveTo;
                query = query.Where(x =>
                    (from == null || x.EffectiveTo == null || x.EffectiveTo >= from) &&
                    (to == null || x.EffectiveFrom == null || x.EffectiveFrom <= to));
            }

            return await query
                .OrderBy(x => x.Product!.ProductName)
                .ThenByDescending(x => x.EffectiveFrom)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProductBaseCommission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.ProductBaseCommissions
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.ProductBaseCommissionId == id, cancellationToken);
        }

        public async Task<List<ProductBaseCommissionHistory>> GetHistoryByCommissionIdAsync(Guid productBaseCommissionId, CancellationToken cancellationToken = default)
        {
            return await _context.ProductBaseCommissionHistories
                .Include(x => x.Product)
                .Where(x => x.ProductBaseCommissionId == productBaseCommissionId)
                .OrderByDescending(x => x.ChangedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ProductLookupDto>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Products
                .Where(x => x.IsActive)
                .OrderBy(x => x.ProductName)
                .ThenBy(x => x.ProductCode)
                .Select(x => new ProductLookupDto
                {
                    ProductId = x.ProductId,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductName
                })
                .ToListAsync(cancellationToken);
        }

        public Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            return _context.Products.FirstOrDefaultAsync(x => x.ProductId == productId && x.IsActive, cancellationToken);
        }

        public async Task<ProductBaseCommission?> GetSmartCommissionAsync(Guid productId, DateTime orderDate, CancellationToken cancellationToken = default)
        {
            return await _context.ProductBaseCommissions
                .Include(x => x.Product)
                .Where(x => x.ProductId == productId
                            && x.IsActive
                            && (x.EffectiveFrom == null || x.EffectiveFrom <= orderDate)
                            && (x.EffectiveTo == null || x.EffectiveTo >= orderDate))
                .OrderByDescending(x => x.EffectiveFrom ?? DateTime.MinValue)
                .ThenByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> ExistsOverlappingActiveCommissionAsync(Guid productId, DateTime? effectiveFrom, DateTime? effectiveTo, Guid? excludeCommissionId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.ProductBaseCommissions.Where(x =>
                x.ProductId == productId &&
                x.IsActive &&
                (excludeCommissionId == null || x.ProductBaseCommissionId != excludeCommissionId.Value));

            return await query.AnyAsync(x =>
                (effectiveFrom == null || x.EffectiveTo == null || x.EffectiveTo >= effectiveFrom) &&
                (effectiveTo == null || x.EffectiveFrom == null || x.EffectiveFrom <= effectiveTo), cancellationToken);
        }

        public async Task AddAsync(ProductBaseCommission entity, CancellationToken cancellationToken = default)
        {
            await _context.ProductBaseCommissions.AddAsync(entity, cancellationToken);
        }

        public async Task AddHistoryAsync(ProductBaseCommissionHistory entity, CancellationToken cancellationToken = default)
        {
            await _context.ProductBaseCommissionHistories.AddAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(ProductBaseCommission entity, CancellationToken cancellationToken = default)
        {
            _context.ProductBaseCommissions.Update(entity);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}
