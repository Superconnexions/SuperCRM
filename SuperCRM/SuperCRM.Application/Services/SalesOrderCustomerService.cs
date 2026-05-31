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
                BusinessType = savedBusiness == null
                ? CustomerBusinessType.Solo
                : (CustomerBusinessType)savedBusiness.BusinessType,

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
                    CityId = personalAddress.CityId,
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
                    CityId = businessAddress.CityId,
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

            // Validaation----------------

            //var productIds = draft.DraftLines.Select(x => x.ProductId).Distinct().ToList();
            //var products = await _repository.GetProductsByIdsAsync(productIds, cancellationToken);
            //var requirement = BuildRequirement(products);

            //var validation = ValidateCustomerRequest(request, requirement);
            //if (!validation.Success)
            //    return Fail(request.SalesOrderDraftId, validation.Message);
            // END Validation

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
                //if (string.IsNullOrWhiteSpace(request.FirstName) || string.IsNullOrWhiteSpace(request.LastName))
                //    return Fail(request.SalesOrderDraftId, "First name and last name are required.");

                customerId = Guid.NewGuid();
                var displayName = string.IsNullOrWhiteSpace(request.DisplayName)
                    ? (request.FirstName.Trim() + " " + request.LastName.Trim()).Trim()
                    : request.DisplayName.Trim();

                var customer = new Customer
                {
                    CustomerId = customerId,
                    CustomerCode = await _repository.GenerateNextCustomerCodeAsync(cancellationToken),
                    //RegistrationSource = 1, // AgentCreated/AdminCreated for demo flow
                    RegistrationSource = (byte)request.RegistrationSource,
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

            var isBusinessAddressSame = request.IsBusinessAddressSameAsPersonal;

                //if ((HasAddressValue(request.PersonalAddress) && !request.IsBusinessFlow) ||
                //   (HasAddressValue(request.PersonalAddress) && request.IsBusinessFlow && request.IsBusinessAddressSameAsPersonal))

                if ( HasAddressValue(request.PersonalAddress) )
                {
                    var personalAddress = BuildAddress(
                        request.PersonalAddress!,
                        customerId,
                        null,
                        (byte)CustomerAddressType.Personal,
                        isDefault: true,
                        isBusinessAddressSame: request.IsBusinessAddressSameAsPersonal,
                        request.CurrentUserId);

                    customerAddressId = personalAddress.CustomerAddressId;
                    await _repository.AddCustomerAddressAsync(personalAddress, cancellationToken);
                    await _repository.SaveChangesAsync(cancellationToken);
                }

                if (request.IsBusinessFlow)
                {
                    //if (string.IsNullOrWhiteSpace(request.Business.BusinessName))
                    //    return Fail(request.SalesOrderDraftId, "Business name is required.");

                    //if (request.BusinessType == (byte)CustomerBusinessType.Limited && string.IsNullOrWhiteSpace(request.Business.RegistrationNo))
                    //{
                    //    return Fail(request.SalesOrderDraftId, "Registration number are required for LTD company.");
                    //}

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

                    var businessAddressDto = request.IsBusinessAddressSameAsPersonal? request.PersonalAddress: request.BusinessAddress;

                    if (HasAddressValue(businessAddressDto))
                    {
                        var businessAddress = BuildAddress(
                            businessAddressDto!,
                            customerId,
                            business.CustomerBusinessId,
                            (byte)CustomerAddressType.Business,
                            isDefault: true,
                            isBusinessAddressSame: request.IsBusinessAddressSameAsPersonal,
                            request.CurrentUserId);

                        customerAddressId = businessAddress.CustomerAddressId;
                        await _repository.AddCustomerAddressAsync(businessAddress, cancellationToken);
                        await _repository.SaveChangesAsync(cancellationToken);
                    }
                } // Business Flow mean Business Product

                if (request.RequiresBankInformation)
                {
                    if (!string.IsNullOrWhiteSpace(request.BankAccount.SortCode)  && !string.IsNullOrWhiteSpace(request.BankAccount.AccountNumber) )
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

        private static (bool Success, string Message) ValidateCustomerRequest(
                SaveSalesOrderCustomerDto request,
                SalesOrderCustomerRequirementDto requirement)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName))
                return (false, "First name is required.");

            if (string.IsNullOrWhiteSpace(request.LastName))
                return (false, "Last name is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                return (false, "Email is required.");

            if (string.IsNullOrWhiteSpace(request.Mobile))
                return (false, "Mobile is required.");

            if (requirement.HasResidentialProduct)
            {
                var homeValidation = ValidateAddress(request.PersonalAddress, "Home Address");
                if (!homeValidation.Success)
                    return homeValidation;
            }

            if (requirement.HasBusinessProduct)
            {
                if (request.Business == null || string.IsNullOrWhiteSpace(request.Business.BusinessName))
                    return (false, "Business name is required.");

                if (string.IsNullOrWhiteSpace(request.Business.BusinessEmail))
                    return (false, "Business email is required.");

                if (request.BusinessType == (byte)CustomerBusinessType.Limited &&
                    string.IsNullOrWhiteSpace(request.Business.RegistrationNo))
                {
                    return (false, "Registration number is required for LTD company.");
                }

                if (request.BusinessType == (byte)CustomerBusinessType.Limited &&
                    string.IsNullOrWhiteSpace(request.Business.ContactPersonName))
                {
                    return (false, "Contact person is required for LTD company.");
                }

                if (request.BusinessType == (byte)CustomerBusinessType.Limited &&
                    string.IsNullOrWhiteSpace(request.Business.ContactPersonPhone))
                {
                    return (false, "Contact person phone number is required for LTD company.");
                }

                var businessAddressDto = request.IsBusinessAddressSameAsPersonal
                    ? request.PersonalAddress
                    : request.BusinessAddress;

                var businessValidation = ValidateAddress(businessAddressDto, "Business Address");
                if (!businessValidation.Success)
                    return businessValidation;
            }


            //if (request.RequiresBankInformation)
            //{
            //    if (string.IsNullOrWhiteSpace(request.BankAccount?.SortCode))
            //        return (false, "Sort code is required.");

            //    if (string.IsNullOrWhiteSpace(request.BankAccount?.AccountNumber))
            //        return (false, "Account number is required.");
            //}


            return (true, string.Empty);
        }

        private static (bool Success, string Message) ValidateAddress(
            SaveSalesOrderAddressDto? address,
            string addressTitle)
        {
            if (address == null)
                return (false, $"{addressTitle} is required.");

            if (string.IsNullOrWhiteSpace(address.HouseNo))
                return (false, $"{addressTitle}: House No is required.");

            if (string.IsNullOrWhiteSpace(address.PostCode))
                return (false, $"{addressTitle}: Post Code is required.");

            if (!address.CountryId.HasValue || address.CountryId.Value <= 0)
                return (false, $"{addressTitle}: Country is required.");

            //if (!address.RegionId.HasValue || address.RegionId.Value <= 0)
            //    return (false, $"{addressTitle}: Region is required.");

            if (!address.CityId.HasValue || address.CityId.Value <= 0)
                return (false, $"{addressTitle}: City is required.");

            return (true, string.Empty);
        }
        private static bool HasAddressValue(SaveSalesOrderAddressDto? address)
        {
            if (address == null)
                return false;

            return !string.IsNullOrWhiteSpace(address.HouseNo)
                && !string.IsNullOrWhiteSpace(address.PostCode)
                && address.CityId.HasValue
                && address.CountryId.HasValue;
                
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
                CityId = dto.CityId,
                CountryId = dto.CountryId,
                RegionId = dto.RegionId,
                AddressLine = dto.AddressLine,
                IsDefault = isDefault,
                IsBusinessAddressSame = isBusinessAddressSame,
                CreatedAt = DateTime.UtcNow,
                UpdatedByUserId = currentUserId
            };
        }

        private static void UpdateAddressEntity(
        CustomerAddress address,
        SaveSalesOrderAddressDto dto,
        Guid? customerBusinessId,
        bool isDefault,
        bool isBusinessAddressSame,
        Guid currentUserId)
        {
            address.CustomerBusinessId = customerBusinessId;
            address.HouseNo = dto.HouseNo;
            address.RoadName = dto.RoadName;
            address.PostCode = dto.PostCode;
            address.City = dto.City;
            address.CityId = dto.CityId;
            address.CountryId = dto.CountryId;
            address.RegionId = dto.RegionId;
            address.AddressLine = dto.AddressLine;
            address.IsDefault = isDefault;
            address.IsBusinessAddressSame = isBusinessAddressSame;
            address.UpdatedAt = DateTime.UtcNow;
            address.UpdatedByUserId = currentUserId;
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

        public async Task<SalesOrderCustomerLoadDto?> GetCustomerForSalesOrderAsync( Guid customerId, CancellationToken cancellationToken = default)
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
                    CityId = personalAddress.CityId,
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
                    CityId = businessAddress.CityId,
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

        public async Task<int?>GetAnyRegionIdByCountryIdAsync(int countryId, CancellationToken cancellationToken = default) { 
        
            return await _repository.GetAnyRegionIdByCountryIdAsync(countryId, cancellationToken);
        }

        public async Task<string?> GetCityNameByCountryIdAsync(int? cityId,
            CancellationToken cancellationToken = default)
        {
            return await _repository.GetCityNameAsync(cityId, cancellationToken);
        }

        public async Task<List<SalesOrderLookupOptionDto>> GetCityOptionsByRegionIdAsync(int regionId,
            CancellationToken cancellationToken = default)
        { 
            return await _repository.GetCityOptionsByRegionIdAsync(regionId, cancellationToken);
        
        }

        public Task<List<CustomerSearchResultDto>> GetCustomersCreatedByUserAsync( Guid currentUserId, CancellationToken cancellationToken = default)
        {
            return _repository.GetCustomersCreatedByUserAsync(
                currentUserId,
                cancellationToken);
        }

        public async Task<(bool EmailExists, bool MobileExists)> CheckCustomerDuplicateAsync(
        string? email,
        string? mobile,
        Guid? excludeCustomerId,
        CancellationToken cancellationToken = default)
        {
            var emailExists = false;
            var mobileExists = false;

            if (!string.IsNullOrWhiteSpace(email))
            {
                emailExists = await _repository.CustomerEmailExistsAsync(
                    email.Trim(),
                    excludeCustomerId,
                    cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(mobile))
            {
                mobileExists = await _repository.CustomerMobileExistsAsync(
                    mobile.Trim(),
                    excludeCustomerId,
                    cancellationToken);
            }

            return (emailExists, mobileExists);
        }

        public async Task<(bool EmailExists, bool MobileExists, bool BankAccountExists)> CheckCustomerDuplicateForOrderAsync(
        string? email,
        string? mobile,
        string? sortCode,
        string? accountNumber,
        Guid? excludeCustomerId,
        Guid? excludeBankAccountId,
        CancellationToken cancellationToken = default)
        {
            var emailExists = false;
            var mobileExists = false;
            var bankAccountExists = false;

            if (!string.IsNullOrWhiteSpace(email))
            {
                emailExists = await _repository.CustomerEmailExistsAsync(
                    email.Trim(),
                    excludeCustomerId,
                    cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(mobile))
            {
                mobileExists = await _repository.CustomerMobileExistsAsync(
                    mobile.Trim(),
                    excludeCustomerId,
                    cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(sortCode) &&
                !string.IsNullOrWhiteSpace(accountNumber))
            {
                bankAccountExists = await _repository.BankAccountExistsAsync(
                    sortCode.Trim(),
                    accountNumber.Trim(),
                    excludeBankAccountId,
                    cancellationToken);
            }

            return (emailExists, mobileExists, bankAccountExists);
        }

        public async Task<SalesOrderCustomerSaveResultDto> UpdateCustomerAsync(
        SaveSalesOrderCustomerDto request,
        CancellationToken cancellationToken = default)
        {
            if (request.CurrentUserId == Guid.Empty)
                return Fail(request.SalesOrderDraftId, "Invalid login session.");

            if (!request.ExistingCustomerId.HasValue || request.ExistingCustomerId.Value == Guid.Empty)
                return Fail(request.SalesOrderDraftId, "Please select a customer before update.");

            var draft = await _repository.GetDraftWithLinesAsync(request.SalesOrderDraftId, cancellationToken);
            if (draft == null)
                return Fail(request.SalesOrderDraftId, "Sales order draft was not found.");

            var customer = await _repository.GetCustomerAsync(request.ExistingCustomerId.Value, cancellationToken);
            if (customer == null)
                return Fail(request.SalesOrderDraftId, "Selected customer was not found.");

            var customerId = customer.CustomerId;

            customer.FirstName = request.FirstName?.Trim() ?? "";
            customer.LastName = request.LastName?.Trim() ?? "";
            customer.DisplayName = string.IsNullOrWhiteSpace(request.DisplayName)
                ? ((request.FirstName ?? "").Trim() + " " + (request.LastName ?? "").Trim()).Trim()
                : request.DisplayName.Trim();

            customer.Email = request.Email;
            customer.AlternativeEmail = request.AlternativeEmail;
            customer.Phone = request.Phone;
            customer.Mobile = request.Mobile;
            customer.IsCompanyDirector = request.IsBusinessFlow;
            customer.UpdatedAt = DateTime.UtcNow;
            customer.UpdatedByUserId = request.CurrentUserId;

            Guid? customerAddressId = null;
            Guid? customerBusinessId = null;
            Guid? customerBankAccountId = null;

            if (HasAddressValue(request.PersonalAddress))
            {
                var personalAddress = await _repository.GetPersonalAddressForUpdateAsync(customerId, cancellationToken);

                if (personalAddress == null)
                {
                    personalAddress = BuildAddress(
                        request.PersonalAddress!,
                        customerId,
                        null,
                        (byte)CustomerAddressType.Personal,
                        isDefault: true,
                        isBusinessAddressSame: request.IsBusinessAddressSameAsPersonal,
                        request.CurrentUserId);

                    await _repository.AddCustomerAddressAsync(personalAddress, cancellationToken);
                }
                else
                {
                    UpdateAddressEntity(
                        personalAddress,
                        request.PersonalAddress!,
                        null,
                        isDefault: true,
                        isBusinessAddressSame: request.IsBusinessAddressSameAsPersonal,
                        request.CurrentUserId);
                }

                customerAddressId = personalAddress.CustomerAddressId;
            }

            CustomerBusiness? business = null;

            if (request.IsBusinessFlow)
            {
                business = await _repository.GetCustomerBusinessForUpdateAsync(customerId, cancellationToken);

                if (business == null)
                {
                    business = new CustomerBusiness
                    {
                        CustomerBusinessId = Guid.NewGuid(),
                        CustomerId = customerId,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    await _repository.AddCustomerBusinessAsync(business, cancellationToken);
                }

                business.BusinessType = request.BusinessType <= 0
                    ? (byte)CustomerBusinessType.Solo
                    : request.BusinessType;

                business.BusinessName = request.Business.BusinessName;
                business.BusinessEmail = request.Business.BusinessEmail;
                business.TradingName = request.Business.TradingName;
                business.RegistrationNo = request.Business.RegistrationNo;
                business.ContactPersonName = request.Business.ContactPersonName;
                business.ContactPersonPhone = request.Business.ContactPersonPhone;
                business.IsActive = true;

                customerBusinessId = business.CustomerBusinessId;

                var businessAddressDto = request.IsBusinessAddressSameAsPersonal
                    ? request.PersonalAddress
                    : request.BusinessAddress;

                if (HasAddressValue(businessAddressDto))
                {
                    var businessAddress = await _repository.GetBusinessAddressForUpdateAsync(
                        business.CustomerBusinessId,
                        cancellationToken);

                    if (businessAddress == null)
                    {
                        businessAddress = BuildAddress(
                            businessAddressDto!,
                            customerId,
                            business.CustomerBusinessId,
                            (byte)CustomerAddressType.Business,
                            isDefault: true,
                            isBusinessAddressSame: request.IsBusinessAddressSameAsPersonal,
                            request.CurrentUserId);

                        await _repository.AddCustomerAddressAsync(businessAddress, cancellationToken);
                    }
                    else
                    {
                        UpdateAddressEntity(
                            businessAddress,
                            businessAddressDto!,
                            business.CustomerBusinessId,
                            isDefault: true,
                            isBusinessAddressSame: request.IsBusinessAddressSameAsPersonal,
                            request.CurrentUserId);
                    }

                    customerAddressId = businessAddress.CustomerAddressId;
                }
            }

            if (request.RequiresBankInformation &&
                !string.IsNullOrWhiteSpace(request.BankAccount.SortCode) &&
                !string.IsNullOrWhiteSpace(request.BankAccount.AccountNumber))
            {
                var bankAccount = await _repository.GetCustomerBankAccountForUpdateAsync(
                    customerId,
                    cancellationToken);

                if (bankAccount == null)
                {
                    bankAccount = new CustomerBankAccount
                    {
                        CustomerBankAccountId = Guid.NewGuid(),
                        CustomerId = customerId,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _repository.AddCustomerBankAccountAsync(bankAccount, cancellationToken);
                }

                bankAccount.BankName = request.BankAccount.BankName;
                bankAccount.AccountName = request.BankAccount.AccountName;
                bankAccount.AccountNumber = request.BankAccount.AccountNumber;
                bankAccount.SortCode = request.BankAccount.SortCode;

                customerBankAccountId = bankAccount.CustomerBankAccountId;
            }

            draft.CustomerId = customerId;
            draft.CustomerBusinessId = customerBusinessId;
            draft.CustomerAddressId = customerAddressId;
            draft.CustomerBankAccountId = request.RequiresBankInformation ? customerBankAccountId : null;
            draft.DraftStatus = 2;
            draft.UpdatedAt = DateTime.UtcNow;
            draft.UpdatedByUserId = request.CurrentUserId;

            await _repository.SaveChangesAsync(cancellationToken);

            return new SalesOrderCustomerSaveResultDto
            {
                Success = true,
                Message = "Customer information updated successfully.",
                SalesOrderDraftId = draft.SalesOrderDraftId,
                CustomerId = customerId,
                CustomerBusinessId = customerBusinessId,
                CustomerAddressId = customerAddressId,
                CustomerBankAccountId = customerBankAccountId
            };
        }

        // END

    }
}
