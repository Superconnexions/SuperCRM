using SuperCRM.Application.DTOs.SalesOrders;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Entities;
using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.Services
{
    public class SalesOrderCustomerService : ISalesOrderCustomerService
    {
        private readonly ISalesOrderCustomerRepository _repository;

        public SalesOrderCustomerService(ISalesOrderCustomerRepository repository)
        {
            _repository = repository;
        }

        public async Task<SalesOrderCustomerCreationPageDto?> GetCustomerCreationPageAsync(Guid draftId, CancellationToken cancellationToken = default)
        {
            var draft = await _repository.GetDraftWithLinesAsync(draftId, cancellationToken);
            if (draft == null) return null;

            var productIds = draft.DraftLines.Select(x => x.ProductId).Distinct().ToList();
            var products = await _repository.GetProductsByIdsAsync(productIds, cancellationToken);
            var requirement = BuildRequirement(products);

            // Start
            Customer? savedCustomer = null;
            CustomerAddress? personalAddress = null;
            CustomerBusiness? savedBusiness = null;
            CustomerAddress? businessAddress = null;
            CustomerBankAccount? savedBankAccount = null;

            if (draft.CustomerId.HasValue)
            {
                savedCustomer = await _repository.GetCustomerWithDetailsAsync(draft.CustomerId.Value, cancellationToken);
                personalAddress = await _repository.GetDefaultCustomerAddressAsync(draft.CustomerId.Value, cancellationToken);
                savedBankAccount = await _repository.GetCustomerBankAccountAsync(draft.CustomerId.Value, cancellationToken);
            }

            if (draft.CustomerBusinessId.HasValue)
            {
                savedBusiness = await _repository.GetCustomerBusinessAsync(draft.CustomerBusinessId.Value, cancellationToken);

                businessAddress = await _repository.GetBusinessAddressAsync(
                    draft.CustomerBusinessId.Value,
                    cancellationToken);
            }
            else if (draft.CustomerId.HasValue)
            {
                savedBusiness = await _repository.GetCustomerBusinessByCustomerIdAsync(draft.CustomerId.Value, cancellationToken);

                if (savedBusiness != null)
                {
                    businessAddress = await _repository.GetBusinessAddressAsync(
                        savedBusiness.CustomerBusinessId,
                        cancellationToken);
                }
            }

            // END

            return new SalesOrderCustomerCreationPageDto
            {
                SalesOrderDraftId = draft.SalesOrderDraftId,
                DraftNo = draft.DraftNo,
                Requirement = requirement,
                Products = draft.DraftLines
                    .OrderBy(x => x.ProviderName ?? "SuperCRM")
                    .ThenBy(x => x.ProductName)
                    .Select(x => new SalesOrderSelectedProductSummaryDto
                    {
                        ProductId = x.ProductId,
                        ProductCode = x.ProductCode ?? string.Empty,
                        ProductName = x.ProductName ?? string.Empty,
                        ProductVariantId = x.ProductVariantId,
                        VariantName = x.VariantName,
                        ProviderId = x.ProviderId,
                        ProviderName = string.IsNullOrWhiteSpace(x.ProviderName) ? "SuperCRM" : x.ProviderName!,
                        Quantity = x.Quantity,
                        SalePrice = x.SalePrice,
                        LineTotalAmount = x.LineTotalAmount,
                        SalesUnitCode = products.FirstOrDefault(p => p.ProductId == x.ProductId)?.SalesUnitCode ?? string.Empty,
                        CurrencyCode = x.CurrencyCode,
                        InstallmentApplicable = x.InstallmentApplicable,
                        IsInstallmentSelected = x.IsInstallmentSelected,
                        DownPaymentAmount = x.DownPaymentAmount,
                        NoOfInstallment = x.NoOfInstallment,
                        MonthlyInstallmentAmount = x.MonthlyInstallmentAmount,
                        FirstInstallmentDate = x.FirstInstallmentDate
                    }).ToList(),
                Countries = await _repository.GetCountryOptionsAsync(cancellationToken),
                Regions = await _repository.GetRegionOptionsAsync(cancellationToken),
                SelectedCustomerId = draft.CustomerId,
                SelectedCustomerBusinessId = draft.CustomerBusinessId,
                SelectedCustomerAddressId = draft.CustomerAddressId,
                SelectedCustomerBankAccountId = draft.CustomerBankAccountId,
                
                // Added to load Customer information
                Customer = savedCustomer == null ? null : new SalesOrderCustomerDto
                {
                    CustomerId = savedCustomer.CustomerId,
                    CustomerCode = savedCustomer.CustomerCode,
                    FirstName = savedCustomer.FirstName,
                    LastName = savedCustomer.LastName,
                    DisplayName = savedCustomer.DisplayName,
                    Email = savedCustomer.Email,
                    AlternativeEmail = savedCustomer.AlternativeEmail,
                    Phone = savedCustomer.Phone,
                    Mobile = savedCustomer.Mobile,
                    IsCompanyDirector = savedCustomer.IsCompanyDirector ?? false
                },

                PersonalAddress = personalAddress == null ? null : new SalesOrderCustomerAddressDto
                {
                    CustomerAddressId = personalAddress.CustomerAddressId,
                    HouseNo = personalAddress.HouseNo,
                    RoadName = personalAddress.RoadName,
                    PostCode = personalAddress.PostCode,
                    City = personalAddress.City,
                    CountryId = personalAddress.CountryId,
                    RegionId = personalAddress.RegionId,
                    AddressLine = personalAddress.AddressLine
                },

                Business = savedBusiness == null ? null : new SalesOrderBusinessDto
                {
                    CustomerBusinessId = savedBusiness.CustomerBusinessId,
                    BusinessType = savedBusiness.BusinessType,
                    BusinessName = savedBusiness.BusinessName,
                    BusinessEmail = savedBusiness.BusinessEmail,
                    TradingName = savedBusiness.TradingName,
                    RegistrationNo = savedBusiness.RegistrationNo,
                    ContactPersonName = savedBusiness.ContactPersonName,
                    ContactPersonPhone = savedBusiness.ContactPersonPhone
                },

                BusinessAddress = businessAddress == null ? null : new SalesOrderCustomerAddressDto
                {
                    CustomerAddressId = businessAddress.CustomerAddressId,
                    HouseNo = businessAddress.HouseNo,
                    RoadName = businessAddress.RoadName,
                    PostCode = businessAddress.PostCode,
                    City = businessAddress.City,
                    CountryId = businessAddress.CountryId,
                    RegionId = businessAddress.RegionId,
                    AddressLine = businessAddress.AddressLine
                },

                BankAccount = savedBankAccount == null ? null : new SalesOrderBankAccountDto
                {
                    CustomerBankAccountId = savedBankAccount.CustomerBankAccountId,
                    BankName = savedBankAccount.BankName,
                    AccountName = savedBankAccount.AccountName,
                    AccountNumber = savedBankAccount.AccountNumber,
                    SortCode = savedBankAccount.SortCode
                }
            };
        }

        public Task<List<CustomerSearchResultDto>> SearchCustomersAsync(string keyword, CancellationToken cancellationToken = default)
        {
            return _repository.SearchCustomersAsync(keyword, cancellationToken);
        }

        public async Task<SalesOrderCustomerSaveResultDto> SaveCustomerAsync(SaveSalesOrderCustomerDto request, CancellationToken cancellationToken = default)
        {
            if (request.CurrentUserId == Guid.Empty)
                return Fail(request.SalesOrderDraftId, "Invalid login session.");

            var draft = await _repository.GetDraftWithLinesAsync(request.SalesOrderDraftId, cancellationToken);
            if (draft == null)
                return Fail(request.SalesOrderDraftId, "Sales order draft was not found.");

            Guid customerId;
            Guid? customerBusinessId = null;
            Guid? customerAddressId = null;
            Guid? customerBankAccountId = null;

            if (request.ExistingCustomerId.HasValue && request.ExistingCustomerId.Value != Guid.Empty)
            {
                var existing = await _repository.GetCustomerAsync(request.ExistingCustomerId.Value, cancellationToken);
                if (existing == null)
                    return Fail(request.SalesOrderDraftId, "Selected customer was not found.");

                customerId = existing.CustomerId;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
                    return Fail(request.SalesOrderDraftId, "First name and last name are required.");

                customerId = Guid.NewGuid();
                var displayName = string.IsNullOrWhiteSpace(request.DisplayName)
                    ? (request.FirstName.Trim() + " " + request.LastName.Trim()).Trim()
                    : request.DisplayName.Trim();

                var customer = new Customer
                {
                    CustomerId = customerId,
                    CustomerCode = await _repository.GenerateNextCustomerCodeAsync(cancellationToken),
                    RegistrationSource = 1, // AgentCreated/AdminCreated for demo flow
                    FirstName = request.FirstName.Trim(),
                    LastName = request.LastName.Trim(),
                    DisplayName = displayName,
                    Email = request.Email,
                    AlternativeEmail = request.AlternativeEmail,
                    Phone = request.Phone,
                    Mobile = request.Mobile,
                    IsCompanyDirector = request.IsBusinessFlow,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = request.CurrentUserId
                };

                await _repository.AddCustomerAsync(customer, cancellationToken);

                var isDirectorPersonalAddress = request.IsBusinessFlow && request.BusinessType == (byte)CustomerBusinessType.Limited;

                //var personalAddress = BuildAddress(
                //    request.PersonalAddress,
                //    customerId,
                //    null,
                //    (byte)CustomerAddressType.Personal,
                //    isDefault: !isDirectorPersonalAddress,
                //    isBusinessAddressSame: request.IsBusinessFlow && request.IsBusinessAddressSameAsPersonal,
                //    request.CurrentUserId);
                //customerAddressId = personalAddress.CustomerAddressId;

                //await _repository.AddCustomerAddressAsync(personalAddress, cancellationToken);

                if (HasAddressValue(request.PersonalAddress))
                {
                    var personalAddress = new CustomerAddress
                    {
                        CustomerAddressId = Guid.NewGuid(),
                        CustomerId = customer.CustomerId,
                        AddressType = 1,
                        HouseNo = request.PersonalAddress!.HouseNo,
                        RoadName = request.PersonalAddress.RoadName,
                        PostCode = request.PersonalAddress.PostCode,
                        City = request.PersonalAddress.City,
                        CountryId = request.PersonalAddress.CountryId,
                        RegionId = request.PersonalAddress.RegionId,
                        AddressLine = request.PersonalAddress.AddressLine,
                        IsDefault = !request.IsBusinessFlow,
                        IsBusinessAddressSame = false,
                        CreatedAt = DateTime.UtcNow
                    };
                    customerAddressId = personalAddress.CustomerAddressId;
                    await _repository.AddCustomerAddressAsync(personalAddress, cancellationToken);
                }

                if (request.IsBusinessFlow)
                {
                    if (string.IsNullOrWhiteSpace(request.Business.BusinessName))
                        return Fail(request.SalesOrderDraftId, "Business name is required.");

                    if (request.BusinessType == (byte)CustomerBusinessType.Limited &&
                        (string.IsNullOrWhiteSpace(request.Business.TradingName) || string.IsNullOrWhiteSpace(request.Business.RegistrationNo)))
                    {
                        return Fail(request.SalesOrderDraftId, "Trading name and registration number are required for LTD company.");
                    }

                    var business = new CustomerBusiness
                    {
                        CustomerBusinessId = Guid.NewGuid(),
                        CustomerId = customerId,
                        BusinessType = request.BusinessType <= 0 ? (byte)CustomerBusinessType.Solo : request.BusinessType,
                        BusinessName = request.Business.BusinessName,
                        BusinessEmail = request.Business.BusinessEmail,
                        TradingName = request.Business.TradingName,
                        RegistrationNo = request.Business.RegistrationNo,
                        ContactPersonName = request.Business.ContactPersonName,
                        ContactPersonPhone = request.Business.ContactPersonPhone,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    customerBusinessId = business.CustomerBusinessId;
                    await _repository.AddCustomerBusinessAsync(business, cancellationToken);

                    // Save Customer + Business first so SalesOrderDrafts FK can reference valid rows
                    await _repository.SaveChangesAsync(cancellationToken);

                    if (!request.IsBusinessAddressSameAsPersonal)
                    {
                        var businessAddress = BuildAddress(
                            request.BusinessAddress,
                            customerId,
                            business.CustomerBusinessId,
                            (byte)CustomerAddressType.Business,
                            isDefault: true,
                            isBusinessAddressSame: false,
                            request.CurrentUserId);

                        customerAddressId = businessAddress.CustomerAddressId;
                        await _repository.AddCustomerAddressAsync(businessAddress, cancellationToken);
                        await _repository.SaveChangesAsync(cancellationToken);
                    }
                }

                if (request.RequiresBankInformation)
                {
                    if (!string.IsNullOrWhiteSpace(request.BankAccount.BankName) && !string.IsNullOrWhiteSpace(request.BankAccount.AccountName) && !string.IsNullOrWhiteSpace(request.BankAccount.AccountNumber) )
                    {
                        var bankAccount = new CustomerBankAccount
                        {
                            CustomerBankAccountId = Guid.NewGuid(),
                            CustomerId = customerId,
                            BankName = request.BankAccount.BankName,
                            AccountName = request.BankAccount.AccountName,
                            AccountNumber = request.BankAccount.AccountNumber,
                            SortCode = request.BankAccount.SortCode,
                            CreatedAt = DateTime.UtcNow
                        };
                        customerBankAccountId = bankAccount.CustomerBankAccountId;
                        await _repository.AddCustomerBankAccountAsync(bankAccount, cancellationToken);
                        // Added later for Storing bank account
                        await _repository.SaveChangesAsync(cancellationToken);
                    }
                    
                }
            }

            draft.CustomerId = customerId;
            draft.CustomerBusinessId = customerBusinessId;
            draft.CustomerAddressId = customerAddressId;
            //draft.CustomerBankAccountId = customerBankAccountId;
            draft.CustomerBankAccountId = request.RequiresBankInformation? customerBankAccountId: null;

            draft.DraftStatus = 2; // CustomerSelected for demo flow
            draft.UpdatedAt = DateTime.UtcNow;
            draft.UpdatedByUserId = request.CurrentUserId;

            await _repository.SaveChangesAsync(cancellationToken);

            return new SalesOrderCustomerSaveResultDto
            {
                Success = true,
                Message = "Customer information saved successfully.",
                SalesOrderDraftId = draft.SalesOrderDraftId,
                CustomerId = customerId,
                CustomerBusinessId = customerBusinessId,
                CustomerAddressId = customerAddressId,
                CustomerBankAccountId = customerBankAccountId
            };
        }
        private static bool HasAddressValue(SaveSalesOrderAddressDto? address)
        {
            if (address == null)
                return false;

            return !string.IsNullOrWhiteSpace(address.HouseNo)
                || !string.IsNullOrWhiteSpace(address.RoadName)
                || !string.IsNullOrWhiteSpace(address.PostCode)
                || !string.IsNullOrWhiteSpace(address.City)
                || address.CountryId.HasValue
                || address.RegionId.HasValue;
        }
        

        private static SalesOrderCustomerRequirementDto BuildRequirement(List<Product> products)
        {
            var hasBusiness = products.Any(x => x.CustomerType == ProductCustomerType.Business || x.CustomerType == ProductCustomerType.Both);
            var hasResidential = products.Any(x => x.CustomerType == ProductCustomerType.Residential || x.CustomerType == ProductCustomerType.Both);
            var mixed = hasBusiness && hasResidential;
            var bank = products.Any(x => x.IsRequiredBankInformation);

            return new SalesOrderCustomerRequirementDto
            {
                HasBusinessProduct = hasBusiness,
                HasResidentialProduct = hasResidential,
                HasMixedBusinessResidential = mixed,
                RequiresBankInformation = bank,
                ScenarioName = mixed ? "Mixed products - treat as Business Customer"
                    : hasBusiness ? "Business Customer"
                    : "Residential Customer"
            };
        }

        private static CustomerAddress BuildAddress(
            SaveSalesOrderAddressDto dto,
            Guid? customerId,
            Guid? customerBusinessId,
            byte addressType,
            bool isDefault,
            bool isBusinessAddressSame,
            Guid currentUserId)
        {
            return new CustomerAddress
            {
                CustomerAddressId = Guid.NewGuid(),
                CustomerId = customerId,
                CustomerBusinessId = customerBusinessId,
                AddressType = addressType,
                HouseNo = dto.HouseNo,
                RoadName = dto.RoadName,
                PostCode = dto.PostCode,
                City = dto.City,
                CountryId = dto.CountryId,
                RegionId = dto.RegionId,
                AddressLine = dto.AddressLine,
                IsDefault = isDefault,
                IsBusinessAddressSame = isBusinessAddressSame,
                CreatedAt = DateTime.UtcNow,
                UpdatedByUserId = currentUserId
            };
        }

        private static SalesOrderCustomerSaveResultDto Fail(Guid draftId, string message)
        {
            return new SalesOrderCustomerSaveResultDto
            {
                Success = false,
                Message = message,
                SalesOrderDraftId = draftId
            };
        }

        // Search Customer

        public async Task<SalesOrderCustomerLoadDto?> GetCustomerForSalesOrderAsync(
     Guid customerId,
     CancellationToken cancellationToken = default)
        {
            if (customerId == Guid.Empty)
            {
                return new SalesOrderCustomerLoadDto
                {
                    Success = false,
                    Message = "Invalid customer."
                };
            }

            var customer = await _repository.GetCustomerWithDetailsAsync(customerId, cancellationToken);
            if (customer == null)
            {
                return new SalesOrderCustomerLoadDto
                {
                    Success = false,
                    Message = "Customer was not found."
                };
            }

            var personalAddress = await _repository.GetDefaultCustomerAddressAsync(customerId, cancellationToken);
            var business = await _repository.GetCustomerBusinessByCustomerIdAsync(customerId, cancellationToken);
            var businessAddress = business == null
                ? null
                : await _repository.GetBusinessAddressAsync(business.CustomerBusinessId, cancellationToken);
            var bankAccount = await _repository.GetCustomerBankAccountAsync(customerId, cancellationToken);

            return new SalesOrderCustomerLoadDto
            {
                Success = true,
                Message = "Customer loaded successfully.",
                CustomerId = customer.CustomerId,
                Customer = new SalesOrderCustomerDto
                {
                    CustomerId = customer.CustomerId,
                    CustomerCode = customer.CustomerCode,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    DisplayName = customer.DisplayName,
                    Email = customer.Email,
                    AlternativeEmail = customer.AlternativeEmail,
                    Phone = customer.Phone,
                    Mobile = customer.Mobile,
                    IsCompanyDirector = customer.IsCompanyDirector ?? false
                },
                PersonalAddress = personalAddress == null ? null : new SalesOrderCustomerAddressDto
                {
                    CustomerAddressId = personalAddress.CustomerAddressId,
                    HouseNo = personalAddress.HouseNo,
                    RoadName = personalAddress.RoadName,
                    PostCode = personalAddress.PostCode,
                    City = personalAddress.City,
                    CountryId = personalAddress.CountryId,
                    RegionId = personalAddress.RegionId,
                    AddressLine = personalAddress.AddressLine
                },
                Business = business == null ? null : new SalesOrderBusinessDto
                {
                    CustomerBusinessId = business.CustomerBusinessId,
                    BusinessType = business.BusinessType,
                    BusinessName = business.BusinessName,
                    BusinessEmail = business.BusinessEmail,
                    TradingName = business.TradingName,
                    RegistrationNo = business.RegistrationNo,
                    ContactPersonName = business.ContactPersonName,
                    ContactPersonPhone = business.ContactPersonPhone
                },
                BusinessAddress = businessAddress == null ? null : new SalesOrderCustomerAddressDto
                {
                    CustomerAddressId = businessAddress.CustomerAddressId,
                    HouseNo = businessAddress.HouseNo,
                    RoadName = businessAddress.RoadName,
                    PostCode = businessAddress.PostCode,
                    City = businessAddress.City,
                    CountryId = businessAddress.CountryId,
                    RegionId = businessAddress.RegionId,
                    AddressLine = businessAddress.AddressLine
                },
                BankAccount = bankAccount == null ? null : new SalesOrderBankAccountDto
                {
                    CustomerBankAccountId = bankAccount.CustomerBankAccountId,
                    BankName = bankAccount.BankName,
                    AccountName = bankAccount.AccountName,
                    AccountNumber = bankAccount.AccountNumber,
                    SortCode = bankAccount.SortCode
                }
            };
        }

        // END

    }
}
