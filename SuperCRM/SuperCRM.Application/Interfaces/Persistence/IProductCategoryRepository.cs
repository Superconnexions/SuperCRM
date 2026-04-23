using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    /// <summary>
    /// Persistence contract for Product Category master setup.
    /// Keeps EF/DbContext out of Application and Web layers.
    /// </summary>
    public interface IProductCategoryRepository
    {
        Task<List<ProductCategory>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductCategory?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
        Task<bool> ExistsByCodeAsync(string categoryCode, Guid? excludeCategoryId = null, CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string categoryName, Guid? excludeCategoryId = null, CancellationToken cancellationToken = default);
        Task AddAsync(ProductCategory entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(ProductCategory entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
