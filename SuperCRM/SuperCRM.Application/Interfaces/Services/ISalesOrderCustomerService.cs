using SuperCRM.Application.DTOs.SalesOrders;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface ISalesOrderCustomerService
    {
        Task<SalesOrderCustomerCreationPageDto?> GetCustomerCreationPageAsync(Guid draftId, CancellationToken cancellationToken = default);
        Task<List<CustomerSearchResultDto>> SearchCustomersAsync(string keyword, CancellationToken cancellationToken = default);
        Task<SalesOrderCustomerSaveResultDto> SaveCustomerAsync(SaveSalesOrderCustomerDto request, CancellationToken cancellationToken = default);
        Task<SalesOrderCustomerLoadDto?> GetCustomerForSalesOrderAsync( Guid customerId, CancellationToken cancellationToken = default);
        Task<int?> GetAnyRegionIdByCountryIdAsync(int countryId, CancellationToken cancellationToken = default);
        Task<List<SalesOrderLookupOptionDto>> GetCityOptionsByRegionIdAsync(int regionId, CancellationToken cancellationToken = default);

        Task<string?> GetCityNameByCountryIdAsync(int? cityId,CancellationToken cancellationToken = default);

        Task<List<CustomerSearchResultDto>> GetCustomersCreatedByUserAsync( Guid currentUserId, CancellationToken cancellationToken = default);

        Task<(bool EmailExists, bool MobileExists)> CheckCustomerDuplicateAsync( string? email, string? mobile, Guid? excludeCustomerId, CancellationToken cancellationToken = default);
        Task<(bool EmailExists, bool MobileExists, bool BankAccountExists)> CheckCustomerDuplicateForOrderAsync(
        string? email,
        string? mobile,
        string? sortCode,
        string? accountNumber,
        Guid? excludeCustomerId,
        Guid? excludeBankAccountId,
        CancellationToken cancellationToken = default);

        Task<SalesOrderCustomerSaveResultDto> UpdateCustomerAsync(
        SaveSalesOrderCustomerDto request,
        CancellationToken cancellationToken = default);

    }
}
