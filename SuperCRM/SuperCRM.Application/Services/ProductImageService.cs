
using SuperCRM.Application.DTOs.Products;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IProductImageRepository _repository;

        public ProductImageService(IProductImageRepository repository)
        {
            _repository = repository;
        }

        public async Task<(bool Success, string ErrorMessage)> AddAsync(CreateProductImageDto request, CancellationToken cancellationToken = default)
        {
            if (request.ProductId == Guid.Empty) return (false, "Invalid Product Id.");
            if (string.IsNullOrWhiteSpace(request.ImageUrl)) return (false, "Image URL is required.");

            var entity = new ProductImage
            {
                ProductImageId = Guid.NewGuid(),
                ProductId = request.ProductId,
                ImageUrl = request.ImageUrl.Trim(),
                //ImageName = string.IsNullOrWhiteSpace(request.ImageName) ? null : request.ImageName.Trim(),
                DisplayOrder = request.DisplayOrder,
                IsPrimary = request.IsPrimary,
                CreatedAt = DateTime.UtcNow
                //,
                //UpdatedAt = null,
                //UpdatedByUserId = request.UpdatedByUserId
            };

            await _repository.AddAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return (true, string.Empty);
        }
    }
}
