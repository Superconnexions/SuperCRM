using SuperCRM.Application.DTOs.SalesOrders;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    public interface ISalesOrderCreationRepository
    {
        Task<SalesOrderDraft?> GetDraftWithLinesAsync(Guid draftId, CancellationToken cancellationToken = default);
        Task<Customer?> GetCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<CustomerBusiness?> GetCustomerBusinessAsync(Guid customerBusinessId, CancellationToken cancellationToken = default);
        Task<CustomerAddress?> GetHomeAddressAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<CustomerAddress?> GetBusinessAddressAsync(Guid customerBusinessId, CancellationToken cancellationToken = default);
        Task<CustomerBankAccount?> GetCustomerBankAccountAsync(Guid customerBankAccountId, CancellationToken cancellationToken = default);
        Task<List<Product>> GetProductsByIdsAsync(List<Guid> productIds, CancellationToken cancellationToken = default);
        Task<List<ProductBaseCommission>> GetActiveProductBaseCommissionsAsync(List<Guid> productIds, DateTime orderDate, CancellationToken cancellationToken = default);
        Task<List<Provider>> GetProvidersByIdsAsync(List<Guid> providerIds, CancellationToken cancellationToken = default);

        Task AddSaleAsync(Sale sale, CancellationToken cancellationToken = default);
        Task AddSaleLineAsync(SaleLine saleLine, CancellationToken cancellationToken = default);
        Task AddInstallmentScheduleAsync(InstallmentSchedule schedule, CancellationToken cancellationToken = default);

        Task<List<Sale>> GetSalesByIdsAsync(List<Guid> saleIds, CancellationToken cancellationToken = default);
        Task<List<SaleLine>> GetSaleLinesBySaleIdsAsync(List<Guid> saleIds, CancellationToken cancellationToken = default);
        Task<List<InstallmentSchedule>> GetInstallmentSchedulesBySaleLineIdsAsync(List<Guid> saleLineIds, CancellationToken cancellationToken = default);
        Task<CustomerAddress?> GetAddressByIdAsync(Guid addressId, CancellationToken cancellationToken = default);
        Task<string?> GetCountryNameAsync(int? countryId, CancellationToken cancellationToken = default);
        Task<string?> GetRegionNameAsync(int? regionId, CancellationToken cancellationToken = default);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        Task<Agent?> GetAgentByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

        Task<bool> IsApprovedAgentAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

        Task<List<SalesOrderHistoryDto>> GetSalesOrderHistoryAsync(
        Guid? soldByUserId,
        CancellationToken cancellationToken = default);
    }
}
