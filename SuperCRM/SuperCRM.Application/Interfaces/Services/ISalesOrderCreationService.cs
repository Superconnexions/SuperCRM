using SuperCRM.Application.DTOs.SalesOrders;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface ISalesOrderCreationService
    {
        Task<CreateSalesOrderResultDto> CreateSalesOrderFromDraftAsync(
            CreateSalesOrderFromDraftRequestDto request,
            CancellationToken cancellationToken = default);

        Task<SalesOrderCreatedSummaryDto?> GetCreatedSalesOrderSummaryAsync(
            List<Guid> saleIds,
            CancellationToken cancellationToken = default);
    }
}
