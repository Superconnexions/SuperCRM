using SuperCRM.Application.DTOs.ProviderProducts;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface IProviderProductService
    {
        Task<List<ProviderProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<ProviderProductDto>> SearchAsync(ProviderProductSearchDto search, CancellationToken cancellationToken = default);
        Task<ProviderProductDto?> GetByIdAsync(Guid providerProductId, CancellationToken cancellationToken = default);
        Task<ProviderProductFormLookupDto> GetFormLookupAsync(CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> CreateAsync(CreateProviderProductDto request, CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> UpdateAsync(UpdateProviderProductDto request, CancellationToken cancellationToken = default);
        Task<(bool Success, string ErrorMessage)> DeleteAsync(Guid providerProductId, CancellationToken cancellationToken = default);
    }
}
