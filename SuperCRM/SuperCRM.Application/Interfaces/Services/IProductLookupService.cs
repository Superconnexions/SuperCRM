
using SuperCRM.Application.DTOs.Lookups;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface IProductLookupService
    {
        Task<List<ProductCategoryLookupDto>> GetCategoriesAsync(CancellationToken cancellationToken = default);
        Task<List<SalesUnitLookupDto>> GetSalesUnitsAsync(CancellationToken cancellationToken = default);
    }
}
