using SuperCRM.Application.DTOs.ProviderProducts;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Services
{
    /// <summary>
    /// Standard application service for ProviderProducts mapping.
    /// Supports list search, validation, duplicate prevention, and delete.
    /// </summary>
    public class ProviderProductService : IProviderProductService
    {
        private readonly IProviderProductRepository _repository;

        public ProviderProductService(IProviderProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProviderProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAllAsync(cancellationToken);
            return entities.OrderBy(x => x.Provider!.ProviderName).ThenBy(x => x.ProductName).Select(MapToDto).ToList();
        }

        public async Task<List<ProviderProductDto>> SearchAsync(ProviderProductSearchDto search, CancellationToken cancellationToken = default)
        {
            var entities = await _repository.SearchAsync(search.ProviderName, search.ProductName, cancellationToken);
            return entities.OrderBy(x => x.Provider!.ProviderName).ThenBy(x => x.ProductName).Select(MapToDto).ToList();
        }

        public async Task<ProviderProductDto?> GetByIdAsync(Guid providerProductId, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(providerProductId, cancellationToken);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<ProviderProductFormLookupDto> GetFormLookupAsync(CancellationToken cancellationToken = default)
        {
            var providers = await _repository.GetActiveProvidersAsync(cancellationToken);
            var products = await _repository.GetActiveProductsAsync(cancellationToken);

            return new ProviderProductFormLookupDto
            {
                Providers = providers.OrderBy(x => x.ProviderName)
                    .Select(x => new ProviderProductLookupItemDto
                    {
                        Value = x.ProviderId.ToString(),
                        Text = x.ProviderName
                    }).ToList(),
                Products = products.OrderBy(x => x.ProductName).ThenBy(x => x.ProductCode)
                    .Select(x => new ProviderProductLookupItemDto
                    {
                        Value = x.ProductId.ToString(),
                        Text = $"{x.ProductName} ({x.ProductCode})",
                        ProductCode = x.ProductCode,
                        ProductName = x.ProductName
                    }).ToList()
            };
        }

        public async Task<(bool Success, string ErrorMessage)> CreateAsync(CreateProviderProductDto request, CancellationToken cancellationToken = default)
        {
            var validation = await ValidateAsync(request.ProviderId, request.ProductId, null, cancellationToken);
            if (!validation.Success)
                return (validation.Success, validation.ErrorMessage);

            var provider = validation.Provider!;
            var product = validation.Product!;

            var entity = new ProviderProduct
            {
                ProviderProductId = Guid.NewGuid(),
                ProviderId = provider.ProviderId,
                ProductId = product.ProductId,
                ProductCode = product.ProductCode.Trim(),
                ProductName = product.ProductName.Trim(),
                CreatedAt = DateTime.UtcNow,
                UpdatedByUserId = request.UpdatedByUserId
            };

            await _repository.AddAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return (true, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateAsync(UpdateProviderProductDto request, CancellationToken cancellationToken = default)
        {
            if (request.ProviderProductId == Guid.Empty)
                return (false, "Invalid Provider Product Id.");

            var entity = await _repository.GetByIdAsync(request.ProviderProductId, cancellationToken);
            if (entity == null)
                return (false, "Provider Product not found.");

            var validation = await ValidateAsync(request.ProviderId, request.ProductId, request.ProviderProductId, cancellationToken);
            if (!validation.Success)
                return (validation.Success, validation.ErrorMessage);

            var provider = validation.Provider!;
            var product = validation.Product!;

            entity.ProviderId = provider.ProviderId;
            entity.ProductId = product.ProductId;
            entity.ProductCode = product.ProductCode.Trim();
            entity.ProductName = product.ProductName.Trim();
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedByUserId = request.UpdatedByUserId;

            await _repository.UpdateAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return (true, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage)> DeleteAsync(Guid providerProductId, CancellationToken cancellationToken = default)
        {
            if (providerProductId == Guid.Empty)
                return (false, "Invalid Provider Product Id.");

            var entity = await _repository.GetByIdAsync(providerProductId, cancellationToken);
            if (entity == null)
                return (false, "Provider Product not found.");

            await _repository.DeleteAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return (true, string.Empty);
        }

        private async Task<(bool Success, string ErrorMessage, Provider? Provider, Product? Product)> ValidateAsync(
            Guid providerId,
            Guid productId,
            Guid? excludeProviderProductId,
            CancellationToken cancellationToken)
        {
            if (providerId == Guid.Empty)
                return (false, "Provider is required.", null, null);

            if (productId == Guid.Empty)
                return (false, "Product is required.", null, null);

            var provider = await _repository.GetProviderByIdAsync(providerId, cancellationToken);
            if (provider == null)
                return (false, "Selected provider was not found.", null, null);

            var product = await _repository.GetProductByIdAsync(productId, cancellationToken);
            if (product == null)
                return (false, "Selected product was not found.", null, null);

            if (await _repository.ExistsAsync(providerId, productId, excludeProviderProductId, cancellationToken))
                return (false, "This provider and product mapping already exists.", provider, product);

            return (true, string.Empty, provider, product);
        }

        private static ProviderProductDto MapToDto(ProviderProduct entity)
        {
            return new ProviderProductDto
            {
                ProviderProductId = entity.ProviderProductId,
                ProviderId = entity.ProviderId,
                ProviderName = entity.Provider?.ProviderName ?? string.Empty,
                ProductId = entity.ProductId,
                ProductCode = entity.ProductCode,
                ProductName = entity.ProductName
            };
        }
    }
}
