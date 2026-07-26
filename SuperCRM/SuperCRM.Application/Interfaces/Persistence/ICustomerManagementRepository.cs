using SuperCRM.Application.DTOs.SalesOrders;
using SuperCRM.Domain.Entities;

namespace SuperCRM.Application.Interfaces.Persistence
{
    public interface ICustomerManagementRepository
    {
        Task<Customer?> GetCustomerForEditAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<bool> CanEditCustomerAsync(Guid customerId, Guid currentUserId, bool canManageAllCustomers, CancellationToken cancellationToken = default);
        Task<List<CustomerEditAddressDto>> GetAddressesForEditAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<CustomerBusiness?> GetBusinessForEditAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<CustomerBankAccount?> GetBankAccountForEditAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<CustomerAddress?> GetAddressForUpdateAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken = default);
        Task ResetDefaultAddressAsync(Guid customerId, byte addressType, Guid? exceptAddressId, CancellationToken cancellationToken = default);
        Task<bool> CustomerEmailExistsAsync(string email, Guid excludeCustomerId, CancellationToken cancellationToken = default);
        Task<bool> CustomerMobileExistsAsync(string mobile, Guid excludeCustomerId, CancellationToken cancellationToken = default);
        Task<bool> BankAccountExistsAsync(string accountNumber, Guid? excludeBankAccountId, CancellationToken cancellationToken = default);
        Task<List<SalesOrderLookupOptionDto>> GetCountryOptionsAsync(CancellationToken cancellationToken = default);
        Task<List<SalesOrderLookupOptionDto>> GetRegionOptionsAsync(CancellationToken cancellationToken = default);
        Task AddAddressAsync(CustomerAddress address, CancellationToken cancellationToken = default);
        Task AddBusinessAsync(CustomerBusiness business, CancellationToken cancellationToken = default);
        Task AddBankAccountAsync(CustomerBankAccount bankAccount, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
