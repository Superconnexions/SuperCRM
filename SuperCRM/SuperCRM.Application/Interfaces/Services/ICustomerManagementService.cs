using SuperCRM.Application.DTOs.SalesOrders;

namespace SuperCRM.Application.Interfaces.Services
{
    public interface ICustomerManagementService
    {
        Task<CustomerEditPageDto?> GetCustomerEditPageAsync(Guid customerId, Guid currentUserId, bool canManageAllCustomers, CancellationToken cancellationToken = default);
        Task<CustomerEditResultDto> UpdateCustomerBasicInfoAsync(UpdateCustomerBasicInfoDto request, CancellationToken cancellationToken = default);
        Task<CustomerEditResultDto> SaveCustomerAddressAsync(SaveCustomerAddressDto request, CancellationToken cancellationToken = default);
        Task<CustomerEditResultDto> UpdateCustomerBusinessAsync(UpdateCustomerBusinessDto request, CancellationToken cancellationToken = default);
        Task<CustomerEditResultDto> UpdateCustomerBankAccountAsync(UpdateCustomerBankAccountDto request, CancellationToken cancellationToken = default);
    }
}
