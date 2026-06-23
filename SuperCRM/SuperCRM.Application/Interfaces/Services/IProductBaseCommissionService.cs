using SuperCRM.Application.DTOs.ProductBaseCommissions;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface IProductBaseCommissionService
    {
        Task<List<ProductBaseCommissionDto>> SearchAsync(ProductBaseCommissionSearchDto search, CancellationToken cancellationToken = default);
        Task<ProductBaseCommissionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<ProductBaseCommissionHistoryDto>> GetHistoryAsync(Guid productBaseCommissionId, CancellationToken cancellationToken = default);
        Task<List<ProductLookupDto>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
        Task<List<ProductLookupDto>> GetProductsAsync(CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> CreateAsync(CreateProductBaseCommissionDto request, CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> UpdateAsync(UpdateProductBaseCommissionDto request, CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> SoftDeleteAsync(Guid id, Guid changedByUserId, string? note, CancellationToken cancellationToken = default);

        // Product Variant Override

        Task<List<ProductVariantCommissionOverrideDto>>
        GetProductVariantCommissionOverridesAsync(
        string? productKeyword,
        CancellationToken cancellationToken = default);

        Task<ProductVariantCommissionOverrideDto?>
        GetProductVariantCommissionOverrideByIdAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task CreateProductVariantCommissionOverrideAsync(
            SaveProductVariantCommissionOverrideDto dto,
            CancellationToken cancellationToken = default);

        Task UpdateProductVariantCommissionOverrideAsync(
            SaveProductVariantCommissionOverrideDto dto,
            CancellationToken cancellationToken = default);

        Task<List<ProductOptionDto>>
        GetProductOptionsAsync(
            CancellationToken cancellationToken = default);

        Task<List<ProductOptionDto>>
        GetVariantOptionsByProductIdAsync(
            Guid productId,
            CancellationToken cancellationToken = default);

        Task<ProductBaseCommissionDto?> GetSmartCommissionAsync(Guid productId, DateTime orderDate, CancellationToken cancellationToken = default);
        Task<CommissionCalculationResultDto> CalculateCommissionAsync(Guid productId, DateTime orderDate, decimal orderAmount, CancellationToken cancellationToken = default);
    }
}
