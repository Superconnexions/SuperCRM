using SuperCRM.Application.DTOs.SalesOrders;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface ISalesOrderDraftService
    {
        Task<SalesOrderDraftSaveResultDto> SaveProductSelectionAsync(SaveSalesOrderProductSelectionDto request, CancellationToken cancellationToken = default);
        Task<SalesOrderDraftDto?> GetDraftAsync(Guid salesOrderDraftId, CancellationToken cancellationToken = default);
    }
}
