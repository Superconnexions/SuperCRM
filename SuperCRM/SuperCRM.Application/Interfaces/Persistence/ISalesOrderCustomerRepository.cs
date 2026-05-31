using SuperCRM.Application.DTOs.SalesOrders;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    public interface ISalesOrderCustomerRepository
    {
        Task<SalesOrderDraft?> GetDraftWithLinesAsync(Guid draftId, CancellationToken cancellationToken = default);
        Task<List<Product>> GetProductsByIdsAsync(List<Guid> productIds, CancellationToken cancellationToken = default);
        Task<List<CustomerSearchResultDto>> SearchCustomersAsync(string keyword, CancellationToken cancellationToken = default);
        Task<List<SalesOrderLookupOptionDto>> GetCountryOptionsAsync(CancellationToken cancellationToken = default);
        Task<List<SalesOrderLookupOptionDto>> GetRegionOptionsAsync(CancellationToken cancellationToken = default);
        Task<string> GenerateNextCustomerCodeAsync(CancellationToken cancellationToken = default);
        Task<Customer?> GetCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task AddCustomerAsync(Customer customer, CancellationToken cancellationToken = default);
        Task AddCustomerAddressAsync(CustomerAddress address, CancellationToken cancellationToken = default);
        Task AddCustomerBusinessAsync(CustomerBusiness business, CancellationToken cancellationToken = default);
        Task AddCustomerBankAccountAsync(CustomerBankAccount bankAccount, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<Customer?> GetCustomerWithDetailsAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<CustomerBusiness?> GetCustomerBusinessAsync(Guid customerBusinessId, CancellationToken cancellationToken = default);
        Task<CustomerAddress?> GetDefaultCustomerAddressAsync( Guid customerId, CancellationToken cancellationToken = default);
        Task<CustomerBusiness?> GetCustomerBusinessByCustomerIdAsync( Guid customerId, CancellationToken cancellationToken = default);
        Task<CustomerBankAccount?> GetCustomerBankAccountAsync( Guid customerId, CancellationToken cancellationToken = default);
        Task<CustomerAddress?> GetBusinessAddressAsync(Guid customerBusinessId, CancellationToken cancellationToken = default);

        // Search Customer
        Task<CustomerAddress?> GetPersonalAddressForUpdateAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);

        Task<CustomerBusiness?> GetCustomerBusinessForUpdateAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);

        Task<CustomerAddress?> GetBusinessAddressForUpdateAsync(
            Guid customerBusinessId,
            CancellationToken cancellationToken = default);

        Task<CustomerBankAccount?> GetCustomerBankAccountForUpdateAsync(
            Guid customerId,
            CancellationToken cancellationToken = default);

        Task<int?> GetAnyRegionIdByCountryIdAsync( int countryId, CancellationToken cancellationToken = default);

        Task<List<SalesOrderLookupOptionDto>> GetCityOptionsByRegionIdAsync( int regionId, CancellationToken cancellationToken = default);

        Task<string?> GetCityNameAsync(
            int? cityId,
            CancellationToken cancellationToken = default);

        Task<List<CustomerSearchResultDto>> GetCustomersCreatedByUserAsync( Guid currentUserId, CancellationToken cancellationToken = default);
        Task<bool> CustomerEmailExistsAsync(string email, Guid? excludeCustomerId, CancellationToken cancellationToken = default);

        Task<bool> CustomerMobileExistsAsync( string mobile, Guid? excludeCustomerId, CancellationToken cancellationToken = default);

        Task<bool> BankAccountExistsAsync(
        string accountNumber,
        Guid? excludeBankAccountId,
        CancellationToken cancellationToken = default);

    }
}
