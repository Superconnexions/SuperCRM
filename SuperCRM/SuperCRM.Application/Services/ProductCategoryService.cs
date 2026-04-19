using SuperCRM.Application.DTOs.ProductCategories;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Services
{
    /// <summary>
    /// Standard application service for Product Category master setup.
    /// This service:
    /// 1. validates required data,
    /// 2. checks uniqueness,
    /// 3. maps DTOs to domain entities,
    /// 4. delegates persistence to repository.
    /// </summary>
    public class ProductCategoryService : IProductCategoryService
    {
        private readonly IProductCategoryRepository _repository;

        public ProductCategoryService(IProductCategoryRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Returns all categories for list page.
        /// Ordered by DisplayOrder then CategoryName for stable UI ordering.
        /// </summary>
        public async Task<List<ProductCategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAllAsync(cancellationToken);

            return entities
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.CategoryName)
                .Select(MapToDto)
                .ToList();
        }

        /// <summary>
        /// Loads one category by primary key.
        /// </summary>
        public async Task<ProductCategoryDto?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(categoryId, cancellationToken);
            return entity == null ? null : MapToDto(entity);
        }

        /// <summary>
        /// Creates a new category after business validation.
        /// </summary>
        public async Task<(bool Success, string ErrorMessage)> CreateAsync(CreateProductCategoryDto request, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(request.CategoryName))
                return (false, "Category Name is required.");

            var normalizedCode = NormalizeCode(request.CategoryCode);
            var normalizedName = request.CategoryName.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedCode))
            {
                if (await _repository.ExistsByCodeAsync(normalizedCode, null, cancellationToken))
                    return (false, "Category Code already exists.");
            }

            if (await _repository.ExistsByNameAsync(normalizedName, null, cancellationToken))
                return (false, "Category Name already exists.");

            var entity = new ProductCategory
            {
                CategoryId = Guid.NewGuid(),
                CategoryCode = normalizedCode,
                CategoryName = normalizedName,
                CategoryImageUrl = CleanOptional(request.CategoryImageUrl),
                DisplayNotes = CleanOptional(request.DisplayNotes),
                DisplayOrder = request.DisplayOrder,
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                UpdatedByUserId = request.UpdatedByUserId
            };

            await _repository.AddAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return (true, string.Empty);
        }

        /// <summary>
        /// Updates an existing category after validation and uniqueness checks.
        /// </summary>
        public async Task<(bool Success, string ErrorMessage)> UpdateAsync(UpdateProductCategoryDto request, CancellationToken cancellationToken = default)
        {
            if (request.CategoryId == Guid.Empty)
                return (false, "Invalid Category Id.");

            if (string.IsNullOrWhiteSpace(request.CategoryName))
                return (false, "Category Name is required.");

            var entity = await _repository.GetByIdAsync(request.CategoryId, cancellationToken);
            if (entity == null)
                return (false, "Product Category not found.");

            var normalizedCode = NormalizeCode(request.CategoryCode);
            var normalizedName = request.CategoryName.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedCode))
            {
                if (await _repository.ExistsByCodeAsync(normalizedCode, request.CategoryId, cancellationToken))
                    return (false, "Category Code already exists.");
            }

            if (await _repository.ExistsByNameAsync(normalizedName, request.CategoryId, cancellationToken))
                return (false, "Category Name already exists.");

            entity.CategoryCode = normalizedCode;
            entity.CategoryName = normalizedName;
            entity.CategoryImageUrl = CleanOptional(request.CategoryImageUrl);
            entity.DisplayNotes = CleanOptional(request.DisplayNotes);
            entity.DisplayOrder = request.DisplayOrder;
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedByUserId = request.UpdatedByUserId;

            await _repository.UpdateAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return (true, string.Empty);
        }

        private static string? NormalizeCode(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim().ToUpperInvariant();
        }

        private static string? CleanOptional(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static ProductCategoryDto MapToDto(ProductCategory entity)
        {
            return new ProductCategoryDto
            {
                CategoryId = entity.CategoryId,
                CategoryCode = entity.CategoryCode,
                CategoryName = entity.CategoryName,
                CategoryImageUrl = entity.CategoryImageUrl,
                DisplayNotes = entity.DisplayNotes,
                DisplayOrder = entity.DisplayOrder,
                IsActive = entity.IsActive
            };
        }
    }
}
