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

        Task<(List<SalesOrderHistoryDto> Items, int TotalRecords)> GetSalesOrderHistoryAsync(
        Guid? soldByUserId,
        DateTime? orderDateFrom,
        DateTime? orderDateTo,
        byte? salesOrderStatus,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

        Task<SalesOrderManagementDetailDto?> GetSalesOrderManagementDetailAsync(
        Guid saleId,
        CancellationToken cancellationToken = default);

            Task<bool> UpdateSalesInformationAsync(
                UpdateSalesInformationDto request,
                CancellationToken cancellationToken = default);

            Task<bool> UpdateSalesCommissionAsync(
                UpdateSalesCommissionDto request,
                CancellationToken cancellationToken = default);

            Task<bool> UpdateSalesOrderStatusAsync(
                UpdateSalesOrderStatusDto request,
                CancellationToken cancellationToken = default);

            Task<bool> UpdateSuperCRMCommissionAsync(
                UpdateSuperCRMCommissionDto request,
                CancellationToken cancellationToken = default);
            Task MarkCustomerEmailSentAsync(
            List<Guid> saleIds,
            CancellationToken cancellationToken = default);

        // END

    }

}
