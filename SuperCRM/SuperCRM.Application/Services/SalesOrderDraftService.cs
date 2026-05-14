using SuperCRM.Application.DTOs.SalesOrders;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Entities;
using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.Services
{
    public class SalesOrderDraftService : ISalesOrderDraftService
    {
        private readonly ISalesOrderDraftRepository _draftRepository;
        private readonly ISalesOrderProductRepository _productRepository;

        public SalesOrderDraftService(
            ISalesOrderDraftRepository draftRepository,
            ISalesOrderProductRepository productRepository)
        {
            _draftRepository = draftRepository;
            _productRepository = productRepository;
        }

        public async Task<SalesOrderDraftSaveResultDto> SaveProductSelectionAsync(
            SaveSalesOrderProductSelectionDto request,
            CancellationToken cancellationToken = default)
        {
            if (request.CurrentUserId == Guid.Empty)
                return Fail("Invalid current user.");

            if (request.Lines == null || !request.Lines.Any())
                return Fail("Please select at least one product.");

            var productList = await _productRepository.GetProductListForOrderAsync(cancellationToken);
            var allProducts = productList.BusinessCategories.SelectMany(x => x.Products)
                .Concat(productList.ResidentialCategories.SelectMany(x => x.Products))
                .GroupBy(x => x.ProductId)
                .Select(g => g.First())
                .ToList();

            SalesOrderDraft draft;
            if (request.SalesOrderDraftId.HasValue && request.SalesOrderDraftId.Value != Guid.Empty)
            {
                draft = await _draftRepository.GetByIdWithLinesAsync(request.SalesOrderDraftId.Value, cancellationToken)
                    ?? throw new InvalidOperationException("Draft not found.");

                _draftRepository.RemoveLines(draft.DraftLines.ToList());
                draft.DraftLines.Clear();
                draft.UpdatedAt = DateTime.UtcNow;
                draft.UpdatedByUserId = request.CurrentUserId;
                draft.DraftStatus = (byte)SalesOrderDraftStatus.ProductSelected;
            }
            else
            {
                draft = new SalesOrderDraft
                {
                    SalesOrderDraftId = Guid.NewGuid(),
                    DraftNo = await _draftRepository.GenerateNextDraftNoAsync(cancellationToken),
                    DraftStatus = (byte)SalesOrderDraftStatus.ProductSelected,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = request.CurrentUserId
                };

                await _draftRepository.AddAsync(draft, cancellationToken);
            }

            foreach (var line in request.Lines)
            {
                var product = allProducts.FirstOrDefault(x => x.ProductId == line.ProductId);
                if (product == null)
                    return Fail("Selected product was not found or inactive.");

                var variant = line.ProductVariantId.HasValue
                    ? product.Variants.FirstOrDefault(x => x.ProductVariantId == line.ProductVariantId.Value)
                    : null;

                var provider = line.ProviderProductId.HasValue
                    ? product.Providers.FirstOrDefault(x => x.ProviderProductId == line.ProviderProductId.Value)
                    : null;

                var quantity = line.Quantity <= 0 ? 1 : line.Quantity;
                var basePrice = ResolveBasePrice(product, variant);
                var salePrice = line.SalePrice > 0 ? line.SalePrice : basePrice;

                draft.DraftLines.Add(new SalesOrderDraftLine
                {
                    SalesOrderDraftLineId = Guid.NewGuid(),
                    SalesOrderDraftId = draft.SalesOrderDraftId,
                    ProductId = product.ProductId,
                    ProductCode = product.ProductCode,
                    ProductName = product.ProductName,
                    ProductVariantId = variant?.ProductVariantId,
                    VariantCode = variant?.VariantCode,
                    VariantName = variant?.VariantName,
                    ProviderProductId = provider?.ProviderProductId,
                    ProviderId = provider?.ProviderId,
                    ProviderName = provider?.ProviderName,
                    Quantity = quantity,
                    BasePriceType = product.BasePriceType,
                    BasePrice = basePrice,
                    SalePrice = salePrice,
                    LineTotalAmount = quantity * salePrice,
                    CurrencyCode = product.CurrencyCode,
                    IsPriceEditable = product.IsPriceEditable,
                    InstallmentApplicable = product.InstallmentApplicable,
                    IsInstallmentSelected = line.IsInstallmentSelected,
                    DownPaymentAmount = line.DownPaymentAmount,
                    NoOfInstallment = line.NoOfInstallment,
                    MonthlyInstallmentAmount = line.MonthlyInstallmentAmount,
                    FirstInstallmentDate = line.FirstInstallmentDate,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _draftRepository.SaveChangesAsync(cancellationToken);

            return new SalesOrderDraftSaveResultDto
            {
                Success = true,
                Message = "Product selection saved successfully.",
                SalesOrderDraftId = draft.SalesOrderDraftId,
                DraftNo = draft.DraftNo
            };
        }

        public async Task<SalesOrderDraftDto?> GetDraftAsync(Guid salesOrderDraftId, CancellationToken cancellationToken = default)
        {
            var draft = await _draftRepository.GetByIdWithLinesAsync(salesOrderDraftId, cancellationToken);
            if (draft == null) return null;

            return new SalesOrderDraftDto
            {
                SalesOrderDraftId = draft.SalesOrderDraftId,
                DraftNo = draft.DraftNo,
                DraftStatus = draft.DraftStatus,
                CreatedByUserId = draft.CreatedByUserId,
                CreatedAt = draft.CreatedAt,
                UpdatedAt = draft.UpdatedAt,
                Lines = draft.DraftLines.Select(x => new SalesOrderDraftLineDto
                {
                    SalesOrderDraftLineId = x.SalesOrderDraftLineId,
                    ProductId = x.ProductId,
                    ProductCode = x.ProductCode ?? string.Empty,
                    ProductName = x.ProductName ?? string.Empty,
                    ProductVariantId = x.ProductVariantId,
                    VariantCode = x.VariantCode,
                    VariantName = x.VariantName,
                    ProviderProductId = x.ProviderProductId,
                    ProviderId = x.ProviderId,
                    ProviderName = x.ProviderName,
                    Quantity = x.Quantity,
                    BasePriceType = x.BasePriceType,
                    BasePrice = x.BasePrice,
                    SalePrice = x.SalePrice,
                    LineTotalAmount = x.LineTotalAmount,
                    CurrencyCode = x.CurrencyCode,
                    IsInstallmentSelected = x.IsInstallmentSelected,
                    NoOfInstallment = x.NoOfInstallment,
                    MonthlyInstallmentAmount = x.MonthlyInstallmentAmount,
                    InstallmentApplicable = x.InstallmentApplicable,
                    DownPaymentAmount = x.DownPaymentAmount,
                    FirstInstallmentDate = x.FirstInstallmentDate
                }).ToList()
            };
        }

        private static decimal ResolveBasePrice(SalesOrderProductItemDto product, SalesOrderProductVariantOptionDto? variant)
        {
            // BasePriceType: 1=SimplePrice, 2=OpenPrice, 3=VariantPrice
            if (product.BasePriceType == 2)
                return 0;

            if (product.BasePriceType == 3 && variant != null)
                return variant.BasePrice ?? 0;

            return product.BasePrice;
        }

        private static SalesOrderDraftSaveResultDto Fail(string message)
        {
            return new SalesOrderDraftSaveResultDto
            {
                Success = false,
                Message = message
            };
        }
    }
}
