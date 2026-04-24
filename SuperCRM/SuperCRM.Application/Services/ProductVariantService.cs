using SuperCRM.Application.DTOs.ProductVariants;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Entities;
using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.Services
{
    /// <summary>
    /// Standard application service for Product Variant master setup.
    /// This service:
    /// 1. validates required data,
    /// 2. checks uniqueness within the selected product,
    /// 3. validates reference data,
    /// 4. maps DTOs to domain entities,
    /// 5. delegates persistence to repository.
    /// </summary>
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductVariantRepository _repository;

        public ProductVariantService(IProductVariantRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductVariantDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAllAsync(cancellationToken);

            return entities
                .OrderBy(x => x.Product!.ProductName)
                .ThenBy(x => x.VariantTypeCode)
                .ThenBy(x => x.VariantName)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<ProductVariantDto?> GetByIdAsync(Guid productVariantId, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(productVariantId, cancellationToken);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<ProductVariantFormLookupDto> GetFormLookupAsync(CancellationToken cancellationToken = default)
        {
            var products = await _repository.GetActiveProductsAsync(cancellationToken);
            var variantTypes = await _repository.GetActiveVariantTypesAsync(cancellationToken);

            return new ProductVariantFormLookupDto
            {
                ProductOptions = products
                    .OrderBy(x => x.ProductName)
                    .Select(x => new ProductVariantLookupDto
                    {
                        Value = x.ProductId.ToString(),
                        Text = $"{x.ProductCode} - {x.ProductName}"
                    }).ToList(),
                VariantTypeOptions = variantTypes
                    .OrderBy(x => x.DisplayOrder)
                    .ThenBy(x => x.TypeValue)
                    .Select(x => new ProductVariantLookupDto
                    {
                        Value = x.TypeCode,
                        Text = $"{x.TypeCode} - {x.TypeValue}"
                    }).ToList(),
                DisplayStyleOptions = GetDisplayStyleOptions()
            };
        }

        public async Task<(bool Success, string ErrorMessage)> CreateAsync(CreateProductVariantDto request, CancellationToken cancellationToken = default)
        {
            var validation = await ValidateAsync(request.ProductId, request.VariantCode, request.VariantTypeCode, request.VariantName, request.DisplayStyle, request.DisplayOrder , null, cancellationToken);
            if (!validation.Success)
                return validation;

            var entity = new ProductVariant
            {
                ProductVariantId = Guid.NewGuid(),
                ProductId = request.ProductId,
                VariantCode = NormalizeCode(request.VariantCode)!,
                VariantTypeCode = NormalizeCode(request.VariantTypeCode)!,
                VariantName = request.VariantName.Trim(),
                DisplayStyle = request.DisplayStyle,
                DisplayOrder = request.DisplayOrder,
                BasePrice = request.BasePrice,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                UpdatedByUserId = request.UpdatedByUserId
            };

            await _repository.AddAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return (true, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateAsync(UpdateProductVariantDto request, CancellationToken cancellationToken = default)
        {
            if (request.ProductVariantId == Guid.Empty)
                return (false, "Invalid Product Variant Id.");

            var entity = await _repository.GetByIdAsync(request.ProductVariantId, cancellationToken);
            if (entity == null)
                return (false, "Product Variant not found.");

            var validation = await ValidateAsync(request.ProductId, request.VariantCode, request.VariantTypeCode, request.VariantName, request.DisplayStyle, request.DisplayOrder, request.ProductVariantId, cancellationToken);
            if (!validation.Success)
                return validation;

            entity.ProductId = request.ProductId;
            entity.VariantCode = NormalizeCode(request.VariantCode)!;
            entity.VariantTypeCode = NormalizeCode(request.VariantTypeCode)!;
            entity.VariantName = request.VariantName.Trim();
            entity.DisplayStyle = request.DisplayStyle;
            entity.DisplayOrder = request.DisplayOrder;
            entity.BasePrice = request.BasePrice;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedByUserId = request.UpdatedByUserId;

            await _repository.UpdateAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return (true, string.Empty);
        }

        private async Task<(bool Success, string ErrorMessage)> ValidateAsync(
            Guid productId,
            string variantCode,
            string variantTypeCode,
            string variantName,
            //byte displayStyle,
            DisplayStyle displayStyle,
            int displayOrder,
            Guid? excludeProductVariantId,
            CancellationToken cancellationToken)
        {
            if (productId == Guid.Empty)
                return (false, "Product is required.");

            if (string.IsNullOrWhiteSpace(variantCode))
                return (false, "Variant Code is required.");

            if (string.IsNullOrWhiteSpace(variantTypeCode))
                return (false, "Variant Type is required.");

            if (string.IsNullOrWhiteSpace(variantName))
                return (false, "Variant Name is required.");

            //if (displayStyle == 0)
            //    return (false, "Display Style is required.");

            if (!Enum.IsDefined(typeof(DisplayStyle), displayStyle))
                return (false, "Display Style is invalid.");

            if (displayOrder == 0)
                return (false, "Display order is required.");

            var normalizedVariantCode = NormalizeCode(variantCode)!;
            var normalizedVariantTypeCode = NormalizeCode(variantTypeCode)!;

            var product = await _repository.GetProductByIdAsync(productId, cancellationToken);
            if (product == null)
                return (false, "Selected Product not found.");

            var type = await _repository.GetVariantTypeByCodeAsync(normalizedVariantTypeCode, cancellationToken);
            if (type == null)
                return (false, "Selected Variant Type not found.");

            if (await _repository.ExistsByProductAndCodeAsync(productId, normalizedVariantCode, excludeProductVariantId, cancellationToken))
                return (false, "Variant Code already exists for the selected product.");

            return (true, string.Empty);
        }

        private static string? NormalizeCode(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim().ToUpperInvariant();
        }

        private static List<ProductVariantLookupDto> GetDisplayStyleOptions()
        {
            //return new List<ProductVariantLookupDto>
            //{
            //    new() { Value = "0", Text = "0 - Select one" },
            //    new() { Value = "1", Text = "1 - Dropdown" },
            //    new() { Value = "2", Text = "2 - Checkbox" }
            //};

            return Enum.GetValues<DisplayStyle>()
            .Select(x => new ProductVariantLookupDto
            {
                Value = ((byte)x).ToString(),
                Text = x.ToString()
            })
            .ToList();
        }

        private static ProductVariantDto MapToDto(ProductVariant entity)
        {
            return new ProductVariantDto
            {
                ProductVariantId = entity.ProductVariantId,
                ProductId = entity.ProductId,
                ProductCode = entity.Product?.ProductCode ?? string.Empty,
                ProductName = entity.Product?.ProductName ?? string.Empty,
                VariantCode = entity.VariantCode,
                VariantTypeCode = entity.VariantTypeCode,
                VariantTypeName = entity.VariantType?.TypeValue ?? string.Empty,
                VariantName = entity.VariantName,
                DisplayStyle = entity.DisplayStyle,
                DisplayOrder = entity.DisplayOrder,
                BasePrice = entity.BasePrice
            };
        }
    }
}
