using SuperCRM.Application.DTOs.ProductBaseCommissions;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    public interface IProductBaseCommissionRepository
    {
        Task<List<ProductBaseCommission>> SearchAsync(string? productKeyword, DateTime? effectiveFrom, DateTime? effectiveTo, bool includeInactive, CancellationToken cancellationToken = default);
        Task<ProductBaseCommission?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<ProductBaseCommissionHistory>> GetHistoryByCommissionIdAsync(Guid productBaseCommissionId, CancellationToken cancellationToken = default);
        Task<List<ProductLookupDto>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
        Task<List<ProductLookupDto>> GetProductsAsync(CancellationToken cancellationToken = default);
        Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<ProductBaseCommission?> GetSmartCommissionAsync(Guid productId, DateTime orderDate, CancellationToken cancellationToken = default);
        Task<bool> ExistsOverlappingActiveCommissionAsync(Guid productId, DateTime? effectiveFrom, DateTime? effectiveTo, Guid? excludeCommissionId = null, CancellationToken cancellationToken = default);
        Task AddAsync(ProductBaseCommission entity, CancellationToken cancellationToken = default);
        Task AddHistoryAsync(ProductBaseCommissionHistory entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(ProductBaseCommission entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<List<ProductVariantCommissionOverrideDto>> GetProductVariantCommissionOverridesAsync(
        string? productKeyword,
        CancellationToken cancellationToken = default);

        Task<ProductVariantCommissionOverrideDto?> GetProductVariantCommissionOverrideByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task CreateProductVariantCommissionOverrideAsync(
            SaveProductVariantCommissionOverrideDto dto,
            CancellationToken cancellationToken = default);

        Task UpdateProductVariantCommissionOverrideAsync(
            SaveProductVariantCommissionOverrideDto dto,
            CancellationToken cancellationToken = default);

        Task<List<ProductOptionDto>> GetProductOptionsAsync(
            CancellationToken cancellationToken = default);

        Task<List<ProductOptionDto>> GetVariantOptionsByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default);
    }
}
