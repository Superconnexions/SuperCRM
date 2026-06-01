using SuperCRM.Application.DTOs.SalesOrders;
using SuperCRM.Domain.Entities;

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

        Task<bool> CanCreateSalesOrderAsync(
        Guid userId,
        bool isAgent,
        CancellationToken cancellationToken = default);

        Task<Agent?> GetAgentByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

        Task<List<SalesOrderHistoryDto>> GetSalesOrderHistoryAsync(
        Guid? soldByUserId,
        CancellationToken cancellationToken = default);
    }
}
