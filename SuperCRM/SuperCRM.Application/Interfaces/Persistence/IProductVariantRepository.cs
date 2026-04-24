using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    /// <summary>
    /// Persistence abstraction for Product Variant master setup.
    /// </summary>
    public interface IProductVariantRepository
    {
        Task<List<ProductVariant>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductVariant?> GetByIdAsync(Guid productVariantId, CancellationToken cancellationToken = default);
        Task<bool> ExistsByProductAndCodeAsync(Guid productId, string variantCode, Guid? excludeProductVariantId = null, CancellationToken cancellationToken = default);
        Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<ProductVariantType?> GetVariantTypeByCodeAsync(string typeCode, CancellationToken cancellationToken = default);
        Task<List<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
        Task<List<ProductVariantType>> GetActiveVariantTypesAsync(CancellationToken cancellationToken = default);
        Task AddAsync(ProductVariant entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(ProductVariant entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
