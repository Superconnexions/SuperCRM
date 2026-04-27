using SuperCRM.Application.DTOs.Products;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Entities;
using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.Services
{
    /// <summary>
    /// Standard application service for Product setup.
    /// Handles validation, normalization, lookup binding, and image mapping.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var entities = await _repository.GetAllAsync(cancellationToken);

            return entities
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.ProductName)
                .Select(MapToDto)
                .ToList();
        }

        public async Task<ProductDto?> GetByIdAsync(Guid productId, CancellationToken cancellationToken = default)
        {
            var entity = await _repository.GetByIdAsync(productId, cancellationToken);
            return entity == null ? null : MapToDto(entity);
        }

        public async Task<ProductFormLookupDto> GetFormLookupAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _repository.GetActiveCategoriesAsync(cancellationToken);
            var salesUnits = await _repository.GetActiveSalesUnitsAsync(cancellationToken);

            return new ProductFormLookupDto
            {
                Categories = categories
                    .OrderBy(x => x.DisplayOrder)
                    .ThenBy(x => x.CategoryName)
                    .Select(x => new ProductLookupItemDto
                    {
                        Value = x.CategoryId.ToString(),
                        Text = $"{x.CategoryName}{(string.IsNullOrWhiteSpace(x.CategoryCode) ? string.Empty : $" ({x.CategoryCode})")}"
                    })
                    .ToList(),

                SalesUnits = salesUnits
                    .OrderBy(x => x.UnitName)
                    .Select(x => new ProductLookupItemDto
                    {
                        Value = x.SalesUnitId.ToString(),
                        Text = $"{x.UnitName} ({x.UnitCode})"
                    })
                    .ToList()
            };
        }

        public async Task<(bool Success, string ErrorMessage)> CreateAsync(CreateProductDto request, CancellationToken cancellationToken = default)
        {
            var validation = await ValidateAsync(
                request.CategoryId,
                request.SalesUnitId,
                request.ProductCode,
                request.ProductName,
                request.ProductDisplayName,
                request.ProductType,
                request.BasePriceType,
                request.InstallmentApplicable,
                request.DownPaymentAmount,
                request.CurrencyCode,
                request.Images,
                null,
                request.NoOfInstallment,
                request.MonthlyInstallmentAmount,
                request.IsThirdPartyProduct,
                cancellationToken);

            if (!validation.Success)
                return (validation.Success, validation.ErrorMessage);

            var category = validation.Category!;
            var salesUnit = validation.SalesUnit!;

            var newProductId = Guid.NewGuid();

            var entity = new Product
            {
                ProductId = newProductId,
                CategoryId = category.CategoryId,
                SalesUnitId = salesUnit.SalesUnitId,
                SalesUnitCode = salesUnit.UnitCode,
                ProductCode = NormalizeCode(request.ProductCode)!,
                ProductName = request.ProductName.Trim(),
                ProductDisplayName = request.ProductDisplayName.Trim(),
                ProductType = request.ProductType,
                CustomerType = request.CustomerType,
                ProductDescription = CleanOptional(request.ProductDescription),
                IsThirdPartyProduct = request.IsThirdPartyProduct,
                InstallmentApplicable = request.InstallmentApplicable,
                DownPaymentAmount = request.InstallmentApplicable ? request.DownPaymentAmount : null,
                NoOfInstallment = request.NoOfInstallment ?? 0,
                MonthlyInstallmentAmount = request.MonthlyInstallmentAmount,

                CurrencyCode = NormalizeCode(request.CurrencyCode) ?? "£",
                IsRequiredBankInformation = request.IsRequiredBankInformation,
                IsProviderDeliveryProduct = request.IsProviderDeliveryProduct,
                BasePriceType = request.BasePriceType,
                BasePrice = NormalizePrice(request.BasePriceType, request.BasePrice),
                IsPriceEditable = request.IsPriceEditable,
                IsPortalVisible = request.IsPortalVisible,
                IsPortalOrderEnabled = request.IsPortalOrderEnabled,
                DisplayOrder = request.DisplayOrder,
                ProductDisplayNotes = CleanOptional(request.ProductDisplayNotes),
                PaymentNotes = CleanOptional(request.PaymentNotes),
                Remarks = CleanOptional(request.Remarks),
                IsActive = request.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,
                UpdatedByUserId = request.UpdatedByUserId,
                ProductImages = BuildImages(() => Guid.NewGuid(), newProductId, request.Images)
            };


            await _repository.AddAsync(entity, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return (true, string.Empty);
        }

        public async Task<(bool Success, string ErrorMessage)> UpdateAsync(UpdateProductDto request, CancellationToken cancellationToken = default)
        {
            if (request.ProductId == Guid.Empty)
                return (false, "Invalid Product Id.");

            var entity = await _repository.GetByIdAsync(request.ProductId, cancellationToken);
            if (entity == null)
                return (false, "Product not found.");

            var validation = await ValidateAsync(
                request.CategoryId,
                request.SalesUnitId,
                request.ProductCode,
                request.ProductName,
                request.ProductDisplayName,
                request.ProductType,
                request.BasePriceType,
                request.InstallmentApplicable,
                request.DownPaymentAmount,
                request.CurrencyCode,
                request.Images,
                request.ProductId,
                request.NoOfInstallment,
                request.MonthlyInstallmentAmount,
                request.IsThirdPartyProduct,
                cancellationToken);

            if (!validation.Success)
                return (validation.Success, validation.ErrorMessage);

            var category = validation.Category!;
            var salesUnit = validation.SalesUnit!;

            entity.CategoryId = category.CategoryId;
            entity.SalesUnitId = salesUnit.SalesUnitId;
            entity.SalesUnitCode = salesUnit.UnitCode;
            entity.ProductCode = NormalizeCode(request.ProductCode)!;
            entity.ProductName = request.ProductName.Trim();
            entity.ProductDisplayName = request.ProductDisplayName.Trim();
            entity.ProductType = request.ProductType;
            entity.CustomerType = request.CustomerType;
            entity.ProductDescription = CleanOptional(request.ProductDescription);
            entity.IsThirdPartyProduct = request.IsThirdPartyProduct;
            entity.InstallmentApplicable = request.InstallmentApplicable;
            entity.DownPaymentAmount = request.InstallmentApplicable ? request.DownPaymentAmount : null;
            entity.CurrencyCode = NormalizeCode(request.CurrencyCode) ?? "£";
            entity.IsRequiredBankInformation = request.IsRequiredBankInformation;
            entity.IsProviderDeliveryProduct = request.IsProviderDeliveryProduct;
            entity.BasePriceType = request.BasePriceType;
            entity.BasePrice = NormalizePrice(request.BasePriceType, request.BasePrice);
            entity.IsPriceEditable = request.IsPriceEditable;
            entity.IsPortalVisible = request.IsPortalVisible;
            entity.IsPortalOrderEnabled = request.IsPortalOrderEnabled;
            entity.DisplayOrder = request.DisplayOrder;
            entity.MonthlyInstallmentAmount = request.MonthlyInstallmentAmount;
            entity.NoOfInstallment = request.NoOfInstallment ?? 0;
            entity.ProductDisplayNotes = CleanOptional(request.ProductDisplayNotes);
            entity.PaymentNotes = CleanOptional(request.PaymentNotes);
            entity.Remarks = CleanOptional(request.Remarks);
            entity.IsActive = request.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedByUserId = request.UpdatedByUserId;

            await _repository.UpdateAsync(entity, cancellationToken);

            var images = BuildImages(Guid.NewGuid, entity.ProductId, request.Images);
            await _repository.ReplaceImagesAsync(entity.ProductId, images, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);

            return (true, string.Empty);
        }

        private async Task<(bool Success, string ErrorMessage, ProductCategory? Category, SalesUnit? SalesUnit)> ValidateAsync(
            Guid categoryId,
            int salesUnitId,
            string productCode,
            string productName,
            string productDisplayName,
            ProductType productType,
            ProductBasePriceType basePriceType,
            bool installmentApplicable,
            decimal? downPaymentAmount,
            string currencyCode,
            List<ProductImageDto> images,
            Guid? excludeProductId,
            int? noOfInstallment,
            decimal? monthlyInstallmentAmount,
            bool isThirdPartyProduct,
            CancellationToken cancellationToken)
        {
            if (categoryId == Guid.Empty)
                return (false, "Category is required.", null, null);

            if (salesUnitId <= 0)
                return (false, "Sales Unit is required.", null, null);

            if (string.IsNullOrWhiteSpace(productCode))
                return (false, "Product Code is required.", null, null);

            if (string.IsNullOrWhiteSpace(productName))
                return (false, "Product Name is required.", null, null);

            if (string.IsNullOrWhiteSpace(productDisplayName))
                return (false, "Product Display Name is required.", null, null);

            if (string.IsNullOrWhiteSpace(currencyCode))
                return (false, "Currency Code is required.", null, null);

            var normalizedCode = NormalizeCode(productCode)!;
            if (await _repository.ExistsByCodeAsync(normalizedCode, excludeProductId, cancellationToken))
                return (false, "Product Code already exists.", null, null);

            if (productType == ProductType.SimpleProduct && basePriceType == ProductBasePriceType.VariantPrice)
                return (false, "Variant Price can be used only for Variant Product.", null, null);

            if (installmentApplicable && downPaymentAmount.HasValue && downPaymentAmount.Value < 0)
                return (false, "Down Payment Amount cannot be negative.", null, null);

            if (!installmentApplicable && downPaymentAmount.HasValue && downPaymentAmount.Value != 0)
            {
                // Not a hard error. Value will be null during save.
                return (false, "InstallmentApplicable shuld be enable for Down Payment Amount.", null, null);
            }
            
            //if(!isThirdPartyProduct && installmentApplicable && (!noOfInstallment.HasValue || noOfInstallment.Value <= 0))
            //    return (false, "Number of Installment should be greater than zero when Installment Applicable is enabled.", null, null);


            var category = await _repository.GetCategoryByIdAsync(categoryId, cancellationToken);
            if (category == null)
                return (false, "Selected Category was not found.", null, null);

            var salesUnit = await _repository.GetSalesUnitByIdAsync(salesUnitId, cancellationToken);
            if (salesUnit == null)
                return (false, "Selected Sales Unit was not found.", null, null);

            var cleanedImages = images
                .Where(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
                .ToList();

            if (cleanedImages.Count > 0 && cleanedImages.Count(x => x.IsPrimary) > 1)
                return (false, "Only one image can be marked as Primary.", null, null);

            return (true, string.Empty, category, salesUnit);
        }

        private static List<ProductImage> BuildImages(Func<Guid> guidFactory, Guid productId, List<ProductImageDto> images)
        {
            var cleaned = images
                .Where(x => !string.IsNullOrWhiteSpace(x.ImageUrl))
                .OrderBy(x => x.DisplayOrder)
                .ToList();

            if (cleaned.Count > 0 && cleaned.All(x => !x.IsPrimary))
                cleaned[0].IsPrimary = true;

            return cleaned.Select(x => new ProductImage
            {
                ProductImageId = x.ProductImageId ?? guidFactory(),
                ProductId = productId,
                ImageUrl = x.ImageUrl?.Trim(),
                DisplayOrder = x.DisplayOrder,
                IsPrimary = x.IsPrimary,
                CreatedAt = DateTime.UtcNow
            }).ToList();
        }

        private static decimal NormalizePrice(ProductBasePriceType priceType, decimal basePrice)
        {
            return priceType == ProductBasePriceType.OpenPrice ? 0 : basePrice;
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

        private static ProductDto MapToDto(Product entity)
        {
            return new ProductDto
            {
                ProductId = entity.ProductId,
                CategoryId = entity.CategoryId,
                CategoryName = entity.Category?.CategoryName ?? string.Empty,
                SalesUnitId = entity.SalesUnitId,
                SalesUnitCode = entity.SalesUnitCode,
                ProductCode = entity.ProductCode,
                ProductName = entity.ProductName,
                ProductDisplayName = entity.ProductDisplayName,
                ProductType = entity.ProductType,
                CustomerType = entity.CustomerType,
                BasePriceType = entity.BasePriceType,
                BasePrice = entity.BasePrice,
                CurrencyCode = entity.CurrencyCode,
                IsActive = entity.IsActive,
                IsPortalVisible = entity.IsPortalVisible,
                IsPortalOrderEnabled = entity.IsPortalOrderEnabled,
                DisplayOrder = entity.DisplayOrder,
                ProductDescription = entity.ProductDescription,
                IsThirdPartyProduct = entity.IsThirdPartyProduct,
                InstallmentApplicable = entity.InstallmentApplicable,
                DownPaymentAmount = entity.DownPaymentAmount,
                NoOfInstallment = entity.NoOfInstallment,
                MonthlyInstallmentAmount = entity.MonthlyInstallmentAmount,

                IsRequiredBankInformation = entity.IsRequiredBankInformation,
                IsProviderDeliveryProduct = entity.IsProviderDeliveryProduct,
                IsPriceEditable = entity.IsPriceEditable,
                ProductDisplayNotes = entity.ProductDisplayNotes,
                PaymentNotes = entity.PaymentNotes,
                Remarks = entity.Remarks,
                Images = entity.ProductImages
                    .OrderBy(x => x.DisplayOrder)
                    .Select(x => new ProductImageDto
                    {
                        ProductImageId = x.ProductImageId,
                        ImageUrl = x.ImageUrl,
                        DisplayOrder = x.DisplayOrder,
                        IsPrimary = x.IsPrimary
                    })
                    .ToList()
            };
        }
    }
}
