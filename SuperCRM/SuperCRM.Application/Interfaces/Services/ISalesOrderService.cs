using SuperCRM.Application.DTOs.SalesOrders;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface ISalesOrderService
    {
        Task<SalesOrderProductListDto> GetProductListForOrderAsync(CancellationToken cancellationToken = default);
    }
}
