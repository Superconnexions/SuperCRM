using SuperCRM.Application.DTOs.ProductBaseCommissions;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface IProductBaseCommissionService
    {
        Task<List<ProductBaseCommissionDto>> SearchAsync(ProductBaseCommissionSearchDto search, CancellationToken cancellationToken = default);
        Task<ProductBaseCommissionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<ProductBaseCommissionHistoryDto>> GetHistoryAsync(Guid productBaseCommissionId, CancellationToken cancellationToken = default);
        Task<List<ProductLookupDto>> GetActiveProductsAsync(CancellationToken cancellationToken = default);

        Task<(bool Success, string ErrorMessage)> CreateAsync(CreateProductBaseCommissionDto request, CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> UpdateAsync(UpdateProductBaseCommissionDto request, CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> SoftDeleteAsync(Guid id, Guid changedByUserId, string? note, CancellationToken cancellationToken = default);

        Task<ProductBaseCommissionDto?> GetSmartCommissionAsync(Guid productId, DateTime orderDate, CancellationToken cancellationToken = default);
        Task<CommissionCalculationResultDto> CalculateCommissionAsync(Guid productId, DateTime orderDate, decimal orderAmount, CancellationToken cancellationToken = default);
    }
}
