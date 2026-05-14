using SuperCRM.Application.DTOs.SalesOrders;

namespace SuperCRM.Application.Interfaces.Persistence
{
    public interface ISalesOrderProductRepository
    {
        Task<SalesOrderProductListDto> GetProductListForOrderAsync(CancellationToken cancellationToken = default);
    }
}
