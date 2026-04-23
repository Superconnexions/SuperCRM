using SuperCRM.Application.DTOs.Products;

namespace SuperCRM.Application.Interfaces.Services
{
    /// <summary>
    /// Application service contract for Product master setup.
    /// </summary>
    public interface IProductService
    {
        Task<List<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductDto?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<ProductFormLookupDto> GetFormLookupAsync(CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> CreateAsync(CreateProductDto request, CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> UpdateAsync(UpdateProductDto request, CancellationToken cancellationToken = default);
    }
}
