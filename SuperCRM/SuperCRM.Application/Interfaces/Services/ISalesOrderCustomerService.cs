using SuperCRM.Application.DTOs.SalesOrders;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface ISalesOrderCustomerService
    {
        Task<SalesOrderCustomerCreationPageDto?> GetCustomerCreationPageAsync(Guid draftId, CancellationToken cancellationToken = default);
        Task<List<CustomerSearchResultDto>> SearchCustomersAsync(string keyword, CancellationToken cancellationToken = default);
        Task<SalesOrderCustomerSaveResultDto> SaveCustomerAsync(SaveSalesOrderCustomerDto request, CancellationToken cancellationToken = default);
        Task<SalesOrderCustomerLoadDto?> GetCustomerForSalesOrderAsync( Guid customerId, CancellationToken cancellationToken = default);
    }
}
