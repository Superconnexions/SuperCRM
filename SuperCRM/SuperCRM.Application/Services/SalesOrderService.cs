using SuperCRM.Application.DTOs.SalesOrders;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;

namespace SuperCRM.Application.Services
{
    public class SalesOrderService : ISalesOrderService
    {
        private readonly ISalesOrderProductRepository _salesOrderProductRepository;

        public SalesOrderService(ISalesOrderProductRepository salesOrderProductRepository)
        {
            _salesOrderProductRepository = salesOrderProductRepository;
        }

        public Task<SalesOrderProductListDto> GetProductListForOrderAsync(CancellationToken cancellationToken = default)
        {
            return _salesOrderProductRepository.GetProductListForOrderAsync(cancellationToken);
        }
    }
}
