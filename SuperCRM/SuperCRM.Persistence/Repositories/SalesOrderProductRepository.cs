using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.DTOs.SalesOrders;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    public class SalesOrderProductRepository : ISalesOrderProductRepository
    {
        private readonly SuperCrmDbContext _dbContext;

        public SalesOrderProductRepository(SuperCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SalesOrderProductListDto> GetProductListForOrderAsync(CancellationToken cancellationToken = default)
        {
            var categories = await _dbContext.ProductCategories
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.CategoryName)
                .Select(x => new
                {
                    x.CategoryId,
                    x.CategoryCode,
                    x.CategoryName,
                    x.CategoryImageUrl,
                    x.DisplayOrder
                })
                .ToListAsync(cancellationToken);

            var products = await _dbContext.Products
                .AsNoTracking()
                //.Where(x => x.IsActive && (x.CustomerType == 1 || x.CustomerType == 2 || x.CustomerType == 3))
                .Where(x => x.IsActive )
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.ProductName)
                .Select(x => new
                {
                    x.ProductId,
                    x.CategoryId,
                    x.ProductCode,
                    x.ProductName,
                    x.ProductDisplayName,
                    ProductType = (byte)x.ProductType,
                    CustomerType = (byte?)x.CustomerType,
                    BasePriceType = (byte)x.BasePriceType,
                    x.BasePrice,
                    x.IsPriceEditable,
                    x.InstallmentApplicable,
                    x.DownPaymentAmount,
                    x.NoOfInstallment,
                    x.MonthlyInstallmentAmount,
                    x.CurrencyCode,
                    x.ProductDescription,
                    x.ProductDisplayNotes,
                    x.PaymentNotes,
                    x.Remarks,
                    x.DisplayOrder
                })
                .ToListAsync(cancellationToken);

            var productIds = products.Select(x => x.ProductId).ToList();

            var primaryImages = await _dbContext.ProductImages
                .AsNoTracking()
                .Where(x => productIds.Contains(x.ProductId) && x.IsPrimary)
                .GroupBy(x => x.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    ImageUrl = g.OrderBy(x => x.DisplayOrder).Select(x => x.ImageUrl).FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            var providerProducts = await _dbContext.ProviderProducts
                .AsNoTracking()
                .Include(x => x.Provider)
                .Where(x => productIds.Contains(x.ProductId))
                .OrderBy(x => x.Provider!.ProviderName)
                .ThenBy(x => x.ProductName)
                .Select(x => new
                {
                    x.ProductId,
                    x.ProviderProductId,
                    x.ProviderId,
                    ProviderName = x.Provider != null ? x.Provider.ProviderName : string.Empty,
                    x.ProductCode,
                    x.ProductName
                })
                .ToListAsync(cancellationToken);

            var variants = await _dbContext.ProductVariants
                .AsNoTracking()
                .Include(x => x.VariantType)
                .Where(x => productIds.Contains(x.ProductId))
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.VariantName)
                .Select(x => new
                {
                    x.ProductId,
                    x.ProductVariantId,
                    x.VariantCode,
                    x.VariantTypeCode,
                    VariantTypeName = x.VariantType != null ? x.VariantType.TypeValue : x.VariantTypeCode,
                    x.VariantName,
                    DisplayStyle = (byte)x.DisplayStyle,
                    x.BasePrice,
                    x.DisplayOrder
                })
                .ToListAsync(cancellationToken);

            List<SalesOrderProductCategoryDto> BuildSection(byte sectionCustomerType)
            {
                return categories
                    .Select(c => new SalesOrderProductCategoryDto
                    {
                        CategoryId = c.CategoryId,
                        CategoryCode = c.CategoryCode ?? string.Empty,
                        CategoryName = c.CategoryName,
                        CategoryImageUrl = c.CategoryImageUrl ?? string.Empty,
                        DisplayOrder = c.DisplayOrder,
                        Products = products
                            .Where(p => p.CategoryId == c.CategoryId && (p.CustomerType == sectionCustomerType || p.CustomerType == 3))
                            .OrderBy(p => p.DisplayOrder)
                            .ThenBy(p => p.ProductName)
                            .Select(p => new SalesOrderProductItemDto
                            {
                                ProductId = p.ProductId,
                                CategoryId = p.CategoryId,
                                ProductCode = p.ProductCode,
                                ProductName = p.ProductName,
                                ProductDisplayName = p.ProductDisplayName,
                                ProductType = p.ProductType,
                                CustomerType = p.CustomerType,
                                BasePriceType = p.BasePriceType,
                                BasePrice = p.BasePrice,
                                IsPriceEditable = p.IsPriceEditable,
                                InstallmentApplicable = p.InstallmentApplicable,
                                DownPaymentAmount = p.DownPaymentAmount,
                                NoOfInstallment = p.NoOfInstallment,
                                MonthlyInstallmentAmount = p.MonthlyInstallmentAmount,
                                CurrencyCode = p.CurrencyCode,
                                ProductDescription = p.ProductDescription ?? string.Empty,
                                ProductDisplayNotes = p.ProductDisplayNotes ?? string.Empty,
                                PaymentNotes = p.PaymentNotes ?? string.Empty,
                                Remarks = p.Remarks ?? string.Empty,
                                DisplayOrder = p.DisplayOrder,
                                PrimaryImageUrl = primaryImages.FirstOrDefault(i => i.ProductId == p.ProductId)?.ImageUrl ?? string.Empty,
                                Providers = providerProducts
                                    .Where(pp => pp.ProductId == p.ProductId)
                                    .Select(pp => new SalesOrderProviderOptionDto
                                    {
                                        ProviderProductId = pp.ProviderProductId,
                                        ProviderId = pp.ProviderId,
                                        ProviderName = pp.ProviderName,
                                        ProductCode = pp.ProductCode,
                                        ProductName = pp.ProductName
                                    }).ToList(),
                                Variants = variants
                                    .Where(v => v.ProductId == p.ProductId)
                                    .Select(v => new SalesOrderProductVariantOptionDto
                                    {
                                        ProductVariantId = v.ProductVariantId,
                                        VariantCode = v.VariantCode,
                                        VariantTypeCode = v.VariantTypeCode,
                                        VariantTypeName = v.VariantTypeName,
                                        VariantName = v.VariantName,
                                        DisplayStyle = v.DisplayStyle,
                                        BasePrice = v.BasePrice,
                                        DisplayOrder = v.DisplayOrder
                                    }).ToList()
                            })
                            .ToList()
                    })
                    .Where(c => c.Products.Any())
                    .OrderBy(c => c.DisplayOrder)
                    .ThenBy(c => c.CategoryName)
                    .ToList();
            }

            return new SalesOrderProductListDto
            {
                BusinessCategories = BuildSection(1),
                ResidentialCategories = BuildSection(2)
            };
        }
    }
}
