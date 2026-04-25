using SuperCRM.Application.DTOs.ProductVariants;

namespace SuperCRM.Application.Interfaces.Services
{
    /// <summary>
    /// Service contract for Product Variant setup.
    /// </summary>
    public interface IProductVariantService
    {
        Task<List<ProductVariantDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProductVariantDto?> GetByIdAsync(Guid productVariantId, CancellationToken cancellationToken = default);
        Task<ProductVariantFormLookupDto> GetFormLookupAsync(CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> CreateAsync(CreateProductVariantDto request, CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> UpdateAsync(UpdateProductVariantDto request, CancellationToken cancellationToken = default);
    }
}
