using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    /// <summary>
    /// Persistence abstraction for Product and ProductImages.
    /// </summary>
    public interface IProductRepository
    {
        Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Product?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(string productCode, Guid? excludeProductId = null, CancellationToken cancellationToken = default);
        Task<ProductCategory?> GetCategoryByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
        Task<SalesUnit?> GetSalesUnitByIdAsync(int salesUnitId, CancellationToken cancellationToken = default);
        Task<List<ProductCategory>> GetActiveCategoriesAsync(CancellationToken cancellationToken = default);
        Task<List<SalesUnit>> GetActiveSalesUnitsAsync(CancellationToken cancellationToken = default);
        Task AddAsync(Product entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(Product entity, CancellationToken cancellationToken = default);
        Task ReplaceImagesAsync(Guid productId, List<ProductImage> images, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
