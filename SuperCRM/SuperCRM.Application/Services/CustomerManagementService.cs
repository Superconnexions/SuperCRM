using SuperCRM.Application.DTOs.SalesOrders;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Entities;
using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.Services
{
    public class CustomerManagementService : ICustomerManagementService
    {
        private readonly ICustomerManagementRepository _repository;

        public CustomerManagementService(ICustomerManagementRepository repository)
        {
            _repository = repository;
        }

        public async Task<CustomerEditPageDto?> GetCustomerEditPageAsync(
            Guid customerId,
            Guid currentUserId,
            bool canManageAllCustomers,
            CancellationToken cancellationToken = default)
        {
            if (customerId == Guid.Empty || currentUserId == Guid.Empty)
                return null;

            if (!await _repository.CanEditCustomerAsync(customerId, currentUserId, canManageAllCustomers, cancellationToken))
                return null;

            var customer = await _repository.GetCustomerForEditAsync(customerId, cancellationToken);
            if (customer == null) return null;

            var business = await _repository.GetBusinessForEditAsync(customerId, cancellationToken);
            var bank = await _repository.GetBankAccountForEditAsync(customerId, cancellationToken);

            return new CustomerEditPageDto
            {
                CustomerId = customer.CustomerId,
                CustomerCode = customer.CustomerCode ?? string.Empty,
                RegistrationSource = customer.RegistrationSource,
                RegistrationSourceText = Enum.IsDefined(typeof(RegistrationSource), customer.RegistrationSource)
                    ? ((RegistrationSource)customer.RegistrationSource).ToString()
                    : customer.RegistrationSource.ToString(),
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                DisplayName = customer.DisplayName,
                Email = customer.Email,
                AlternativeEmail = customer.AlternativeEmail,
                Phone = customer.Phone,
                Mobile = customer.Mobile,
                IsCompanyDirector = customer.IsCompanyDirector ?? false,
                Addresses = await _repository.GetAddressesForEditAsync(customerId, cancellationToken),
                Countries = await _repository.GetCountryOptionsAsync(cancellationToken),
                Regions = await _repository.GetRegionOptionsAsync(cancellationToken),
                Business = new CustomerEditBusinessDto
                {
                    CustomerId = customerId,
                    CustomerBusinessId = business?.CustomerBusinessId,
                    BusinessType = business?.BusinessType ?? (byte)CustomerBusinessType.Solo,
                    BusinessName = business?.BusinessName,
                    BusinessEmail = business?.BusinessEmail,
                    TradingName = business?.TradingName,
                    RegistrationNo = business?.RegistrationNo,
                    ContactPersonName = business?.ContactPersonName,
                    ContactPersonPhone = business?.ContactPersonPhone
                },
                BankAccount = new CustomerEditBankAccountDto
                {
                    CustomerId = customerId,
                    CustomerBankAccountId = bank?.CustomerBankAccountId,
                    BankName = bank?.BankName,
                    AccountName = bank?.AccountName,
                    AccountNumber = bank?.AccountNumber,
                    SortCode = bank?.SortCode
                }
            };
        }

        public async Task<CustomerEditResultDto> UpdateCustomerBasicInfoAsync(UpdateCustomerBasicInfoDto request, CancellationToken cancellationToken = default)
        {
            var access = await ValidateAccessAsync(request.CustomerId, request.CurrentUserId, request.CanManageAllCustomers, cancellationToken);
            if (access != null) return access;

            var errors = new Dictionary<string, string[]>();
            if (string.IsNullOrWhiteSpace(request.FirstName)) errors[nameof(request.FirstName)] = new[] { "First name is required." };
            if (string.IsNullOrWhiteSpace(request.LastName)) errors[nameof(request.LastName)] = new[] { "Last name is required." };
            if (string.IsNullOrWhiteSpace(request.Email)) errors[nameof(request.Email)] = new[] { "Email is required." };
            if (string.IsNullOrWhiteSpace(request.Mobile)) errors[nameof(request.Mobile)] = new[] { "Mobile is required." };

            if (!string.IsNullOrWhiteSpace(request.Email) &&
                await _repository.CustomerEmailExistsAsync(request.Email, request.CustomerId, cancellationToken))
                errors[nameof(request.Email)] = new[] { "Email already exists." };

            if (!string.IsNullOrWhiteSpace(request.Mobile) &&
                await _repository.CustomerMobileExistsAsync(request.Mobile, request.CustomerId, cancellationToken))
                errors[nameof(request.Mobile)] = new[] { "Mobile number already exists." };

            if (errors.Count > 0) return Fail(request.CustomerId, "Please correct the validation errors.", errors);

            var customer = await _repository.GetCustomerForEditAsync(request.CustomerId, cancellationToken);
            if (customer == null) return Fail(request.CustomerId, "Customer was not found.");

            customer.FirstName = request.FirstName.Trim();
            customer.LastName = request.LastName.Trim();
            customer.DisplayName = string.IsNullOrWhiteSpace(request.DisplayName)
                ? $"{customer.FirstName} {customer.LastName}".Trim()
                : request.DisplayName.Trim();
            customer.Email = Clean(request.Email);
            customer.AlternativeEmail = Clean(request.AlternativeEmail);
            customer.Phone = Clean(request.Phone);
            customer.Mobile = Clean(request.Mobile);
            customer.IsCompanyDirector = request.IsCompanyDirector;
            customer.UpdatedAt = DateTime.UtcNow;
            customer.UpdatedByUserId = request.CurrentUserId;

            await _repository.SaveChangesAsync(cancellationToken);
            return Success(request.CustomerId, "Customer information updated successfully.");
        }

        public async Task<CustomerEditResultDto> SaveCustomerAddressAsync(SaveCustomerAddressDto request, CancellationToken cancellationToken = default)
        {
            var access = await ValidateAccessAsync(request.CustomerId, request.CurrentUserId, request.CanManageAllCustomers, cancellationToken);
            if (access != null) return access;

            var errors = new Dictionary<string, string[]>();
            if (!Enum.IsDefined(typeof(AddressType), request.AddressType)) errors[nameof(request.AddressType)] = new[] { "Please select a valid address type." };
            if (string.IsNullOrWhiteSpace(request.HouseNo)) errors[nameof(request.HouseNo)] = new[] { "House No is required." };
            if (string.IsNullOrWhiteSpace(request.PostCode)) errors[nameof(request.PostCode)] = new[] { "Post Code is required." };
            if (!request.CountryId.HasValue) errors[nameof(request.CountryId)] = new[] { "Country is required." };
            if (!request.CityId.HasValue) errors[nameof(request.CityId)] = new[] { "City is required." };
            if (errors.Count > 0) return Fail(request.CustomerId, "Please correct the validation errors.", errors);

            CustomerAddress address;
            var isNew = !request.CustomerAddressId.HasValue || request.CustomerAddressId == Guid.Empty;

            if (isNew)
            {
                address = new CustomerAddress
                {
                    CustomerAddressId = Guid.NewGuid(),
                    CustomerId = request.CustomerId,
                    CreatedAt = DateTime.UtcNow
                };
                await _repository.AddAddressAsync(address, cancellationToken);
            }
            else
            {
                var existingAddress = await _repository.GetAddressForUpdateAsync(request.CustomerId, request.CustomerAddressId!.Value, cancellationToken);
                if (existingAddress == null)
                    return Fail(request.CustomerId, "Customer address was not found.");

                address = existingAddress;
                address.UpdatedAt = DateTime.UtcNow;
                address.UpdatedByUserId = request.CurrentUserId;
            }

            if (request.IsDefault)
                await _repository.ResetDefaultAddressAsync(request.CustomerId, request.AddressType, address.CustomerAddressId, cancellationToken);

            address.AddressType = request.AddressType;
            address.AddressLine = Clean(request.AddressLine);
            address.HouseNo = Clean(request.HouseNo);
            address.RoadName = Clean(request.RoadName);
            address.PostCode = Clean(request.PostCode);
            address.City = Clean(request.City);
            address.CountryId = request.CountryId;
            address.RegionId = request.RegionId;
            address.CityId = request.CityId;
            address.IsDefault = request.IsDefault;
            address.IsBusinessAddressSame = false;

            var business = request.AddressType == (byte)AddressType.Business
                ? await _repository.GetBusinessForEditAsync(request.CustomerId, cancellationToken)
                : null;
            address.CustomerBusinessId = business?.CustomerBusinessId;

            await _repository.SaveChangesAsync(cancellationToken);
            return Success(request.CustomerId, isNew ? "Address added successfully." : "Address updated successfully.", address.CustomerAddressId);
        }

        public async Task<CustomerEditResultDto> UpdateCustomerBusinessAsync(UpdateCustomerBusinessDto request, CancellationToken cancellationToken = default)
        {
            var access = await ValidateAccessAsync(request.CustomerId, request.CurrentUserId, request.CanManageAllCustomers, cancellationToken);
            if (access != null) return access;

            var errors = new Dictionary<string, string[]>();
            if (string.IsNullOrWhiteSpace(request.BusinessName)) errors[nameof(request.BusinessName)] = new[] { "Business name is required." };
            if (string.IsNullOrWhiteSpace(request.BusinessEmail)) errors[nameof(request.BusinessEmail)] = new[] { "Business email is required." };
            if (!Enum.IsDefined(typeof(CustomerBusinessType), request.BusinessType)) errors[nameof(request.BusinessType)] = new[] { "Please select a valid business type." };
            if (request.BusinessType == (byte)CustomerBusinessType.Limited)
            {
                if (string.IsNullOrWhiteSpace(request.RegistrationNo)) errors[nameof(request.RegistrationNo)] = new[] { "Registration number is required for LTD company." };
                if (string.IsNullOrWhiteSpace(request.ContactPersonName)) errors[nameof(request.ContactPersonName)] = new[] { "Contact person is required for LTD company." };
                if (string.IsNullOrWhiteSpace(request.ContactPersonPhone)) errors[nameof(request.ContactPersonPhone)] = new[] { "Contact person phone number is required for LTD company." };
            }
            if (errors.Count > 0) return Fail(request.CustomerId, "Please correct the validation errors.", errors);

            var business = await _repository.GetBusinessForEditAsync(request.CustomerId, cancellationToken);
            var isNew = business == null;
            if (business == null)
            {
                business = new CustomerBusiness
                {
                    CustomerBusinessId = Guid.NewGuid(),
                    CustomerId = request.CustomerId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                await _repository.AddBusinessAsync(business, cancellationToken);
            }
            else
            {
                business.UpdatedAt = DateTime.UtcNow;
                business.UpdatedByUserId = request.CurrentUserId;
            }

            business.BusinessType = request.BusinessType;
            business.BusinessName = Clean(request.BusinessName);
            business.BusinessEmail = Clean(request.BusinessEmail);
            business.TradingName = Clean(request.TradingName);
            business.RegistrationNo = Clean(request.RegistrationNo);
            business.ContactPersonName = Clean(request.ContactPersonName);
            business.ContactPersonPhone = Clean(request.ContactPersonPhone);
            business.IsActive = true;

            await _repository.SaveChangesAsync(cancellationToken);
            return Success(request.CustomerId, isNew ? "Business information added successfully." : "Business information updated successfully.", business.CustomerBusinessId);
        }

        public async Task<CustomerEditResultDto> UpdateCustomerBankAccountAsync(UpdateCustomerBankAccountDto request, CancellationToken cancellationToken = default)
        {
            var access = await ValidateAccessAsync(request.CustomerId, request.CurrentUserId, request.CanManageAllCustomers, cancellationToken);
            if (access != null) return access;

            var bank = await _repository.GetBankAccountForEditAsync(request.CustomerId, cancellationToken);
            var existingId = bank?.CustomerBankAccountId;

            if (!string.IsNullOrWhiteSpace(request.AccountNumber) &&
                await _repository.BankAccountExistsAsync(request.AccountNumber, existingId, cancellationToken))
                return Fail(request.CustomerId, "Please correct the validation errors.", new Dictionary<string, string[]>
                {
                    [nameof(request.AccountNumber)] = new[] { "Bank account already exists for this Account Number." }
                });

            var isNew = bank == null;
            if (bank == null)
            {
                bank = new CustomerBankAccount
                {
                    CustomerBankAccountId = Guid.NewGuid(),
                    CustomerId = request.CustomerId,
                    CreatedAt = DateTime.UtcNow
                };
                await _repository.AddBankAccountAsync(bank, cancellationToken);
            }
            else
            {
                bank.UpdatedAt = DateTime.UtcNow;
                bank.UpdatedByUserId = request.CurrentUserId;
            }

            bank.BankName = Clean(request.BankName);
            bank.AccountName = Clean(request.AccountName);
            bank.AccountNumber = Clean(request.AccountNumber);
            bank.SortCode = Clean(request.SortCode);

            await _repository.SaveChangesAsync(cancellationToken);
            return Success(request.CustomerId, isNew ? "Bank information added successfully." : "Bank information updated successfully.", bank.CustomerBankAccountId);
        }

        private async Task<CustomerEditResultDto?> ValidateAccessAsync(Guid customerId, Guid currentUserId, bool canManageAllCustomers, CancellationToken cancellationToken)
        {
            if (customerId == Guid.Empty || currentUserId == Guid.Empty)
                return Fail(customerId, "Invalid request or login session.");

            if (!await _repository.CanEditCustomerAsync(customerId, currentUserId, canManageAllCustomers, cancellationToken))
                return Fail(customerId, "You are not authorized to edit this customer.");

            return null;
        }

        private static string? Clean(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static CustomerEditResultDto Success(Guid customerId, string message, Guid? recordId = null) => new()
        {
            Success = true,
            Message = message,
            CustomerId = customerId,
            RecordId = recordId
        };

        private static CustomerEditResultDto Fail(Guid customerId, string message, Dictionary<string, string[]>? errors = null) => new()
        {
            Success = false,
            Message = message,
            CustomerId = customerId,
            Errors = errors ?? new Dictionary<string, string[]>()
        };
    }
}
