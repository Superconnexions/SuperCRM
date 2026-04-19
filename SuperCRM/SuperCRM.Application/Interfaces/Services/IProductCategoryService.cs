using SuperCRM.Application.DTOs.ProductCategories;

namespace SuperCRM.Application.Interfaces.Services
{
    /// <summary>
    /// Application service contract for Product Category master setup.
    /// Business rules and validation live in this layer.
    /// </summary>
    public interface IProductCategoryService
    {
        Task<List<ProductCategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductCategoryDto?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> CreateAsync(CreateProductCategoryDto request, CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> UpdateAsync(UpdateProductCategoryDto request, CancellationToken cancellationToken = default);
    }
}
