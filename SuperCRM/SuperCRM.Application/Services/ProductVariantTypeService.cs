using SuperCRM.Application.DTOs.ProductVariantTypes;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Services
{
    public class ProductVariantTypeService : IProductVariantTypeService
    {
        private readonly IProductVariantTypeRepository _repository;

        public ProductVariantTypeService(IProductVariantTypeRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductVariantTypeDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAllAsync(cancellationToken);

            return entities
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.TypeValue)
                .Select(x => new ProductVariantTypeDto
                {
                    TypeCode = x.TypeCode,
                    TypeValue = x.TypeValue,
                    DisplayOrder = x.DisplayOrder,
                    IsActive = x.IsActive
                })
                .ToList();
        }

        public async Task<ProductVariantTypeDto?> GetByTypeCodeAsync(string typeCode, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByTypeCodeAsync(typeCode, cancellationToken);
            if (entity == null)
                return null;

            return new ProductVariantTypeDto
            {
                TypeCode = entity.TypeCode,
                TypeValue = entity.TypeValue,
                DisplayOrder = entity.DisplayOrder,
                IsActive = entity.IsActive
            };
        }

        public async Task<(bool Success, string ErrorMessage)> CreateAsync(CreateProductVariantTypeDto dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.TypeCode))
                return (false, "Type Code is required.");

            if (string.IsNullOrWhiteSpace(dto.TypeValue))
                return (false, "Type Value is required.");

            var normalizedCode = dto.TypeCode.Trim().ToUpperInvariant();

            var exists = await _repository.ExistsByTypeCodeAsync(normalizedCode, cancellationToken);
            if (exists)
                return (false, "Type Code already exists.");

            var entity = new ProductVariantType
            {
                TypeCode = normalizedCode,
                TypeValue = dto.TypeValue.Trim(),
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                UpdatedByUserId = dto.UpdatedByUserId
            };

            await _repository.AddAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return (true, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateAsync(UpdateProductVariantTypeDto dto, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(dto.TypeCode))
                return (false, "Type Code is required.");

            if (string.IsNullOrWhiteSpace(dto.TypeValue))
                return (false, "Type Value is required.");

            var normalizedCode = dto.TypeCode.Trim().ToUpperInvariant();

            var entity = await _repository.GetByTypeCodeAsync(normalizedCode, cancellationToken);
            if (entity == null)
                return (false, "Product Variant Type not found.");

            entity.TypeValue = dto.TypeValue.Trim();
            entity.DisplayOrder = dto.DisplayOrder;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedByUserId = dto.UpdatedByUserId;

            await _repository.UpdateAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return (true, string.Empty);
        }
    }
}