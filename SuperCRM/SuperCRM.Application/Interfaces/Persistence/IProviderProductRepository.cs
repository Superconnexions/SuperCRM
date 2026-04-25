using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    public interface IProviderProductRepository
    {
        Task<List<ProviderProduct>> SearchAsync(string? providerName, string? productName, CancellationToken cancellationToken = default);
        Task<List<ProviderProduct>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<ProviderProduct?> GetByIdAsync(Guid providerProductId, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(Guid providerId, Guid productId, Guid? excludeProviderProductId = null, CancellationToken cancellationToken = default);
        Task<Provider?> GetProviderByIdAsync(Guid providerId, CancellationToken cancellationToken = default);
        Task<Product?> GetProductByIdAsync(Guid productId, CancellationToken cancellationToken = default);
        Task<List<Provider>> GetActiveProvidersAsync(CancellationToken cancellationToken = default);
        Task<List<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
        Task AddAsync(ProviderProduct entity, CancellationToken cancellationToken = default);
        Task UpdateAsync(ProviderProduct entity, CancellationToken cancellationToken = default);
        Task DeleteAsync(ProviderProduct entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
