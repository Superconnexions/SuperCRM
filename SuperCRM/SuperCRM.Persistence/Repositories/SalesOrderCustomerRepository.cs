using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.DTOs.SalesOrders;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Domain.Enums;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    public class SalesOrderCustomerRepository : ISalesOrderCustomerRepository
    {
        private readonly SuperCrmDbContext _dbContext;

        public SalesOrderCustomerRepository(SuperCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<SalesOrderDraft?> GetDraftWithLinesAsync(Guid draftId, CancellationToken cancellationToken = default)
        {
            return _dbContext.SalesOrderDrafts
                .Include(x => x.DraftLines)
                .FirstOrDefaultAsync(x => x.SalesOrderDraftId == draftId, cancellationToken);
        }

        public Task<List<Product>> GetProductsByIdsAsync(List<Guid> productIds, CancellationToken cancellationToken = default)
        {
            return _dbContext.Products
                .AsNoTracking()
                .Where(x => productIds.Contains(x.ProductId))
                .ToListAsync(cancellationToken);
        }

        public async Task<List<CustomerSearchResultDto>> SearchCustomersAsync(string keyword, CancellationToken cancellationToken = default)
        {
            keyword = (keyword ?? string.Empty).Trim();
            if (keyword.Length < 2) return new List<CustomerSearchResultDto>();

            return await _dbContext.Customers
                .AsNoTracking()
                .Where(x => x.IsActive &&
                    ((x.CustomerCode ?? string.Empty).Contains(keyword) ||
                     (x.DisplayName ?? string.Empty).Contains(keyword) ||
                     (x.Email ?? string.Empty).Contains(keyword) ||
                     (x.FirstName ?? string.Empty).Contains(keyword) ||
                     (x.LastName ?? string.Empty).Contains(keyword) ||
                     (x.Phone ?? string.Empty).Contains(keyword) ||
                     (x.Mobile ?? string.Empty).Contains(keyword)))
                .OrderBy(x => x.DisplayName)
                .Take(20)
                .Select(x => new CustomerSearchResultDto
                {
                    CustomerId = x.CustomerId,
                    CustomerCode = x.CustomerCode ?? string.Empty,
                    DisplayName = x.DisplayName ?? (x.FirstName + " " + x.LastName),
                    Email = x.Email ?? string.Empty,
                    Phone = x.Phone ?? string.Empty,
                    Mobile = x.Mobile ?? string.Empty,
                    IsCompanyDirector = x.IsCompanyDirector ?? false
                })
                .ToListAsync(cancellationToken);
        }

        public Task<List<SalesOrderLookupOptionDto>> GetCountryOptionsAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.Countries
                .AsNoTracking()
                .OrderBy(x => x.CountryName)
                .Select(x => new SalesOrderLookupOptionDto
                {
                    Value = x.CountryId.ToString(),
                    Text = x.CountryName
                })
                .ToListAsync(cancellationToken);
        }

        public Task<List<SalesOrderLookupOptionDto>> GetRegionOptionsAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.Regions
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.RegionName)
                .Select(x => new SalesOrderLookupOptionDto
                {
                    Value = x.RegionId.ToString(),
                    Text = x.RegionName
                })
                .ToListAsync(cancellationToken);
        }

        public async Task<string> GenerateNextCustomerCodeAsync(CancellationToken cancellationToken = default)
        {
            var prefix = "CUS-" + DateTime.UtcNow.ToString("yyyyMMdd") + "-";
            var countToday = await _dbContext.Customers
                .AsNoTracking()
                .CountAsync(x => (x.CustomerCode ?? string.Empty).StartsWith(prefix), cancellationToken);

            return prefix + (countToday + 1).ToString("0000");
        }

        public Task<Customer?> GetCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return _dbContext.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);
        }

        public async Task AddCustomerAsync(Customer customer, CancellationToken cancellationToken = default)
        {
            await _dbContext.Customers.AddAsync(customer, cancellationToken);
        }

        public async Task AddCustomerAddressAsync(CustomerAddress address, CancellationToken cancellationToken = default)
        {
            await _dbContext.CustomerAddresses.AddAsync(address, cancellationToken);
        }

        public async Task AddCustomerBusinessAsync(CustomerBusiness business, CancellationToken cancellationToken = default)
        {
            await _dbContext.CustomerBusinesses.AddAsync(business, cancellationToken);
        }

        public async Task AddCustomerBankAccountAsync(CustomerBankAccount bankAccount, CancellationToken cancellationToken = default)
        {
            await _dbContext.CustomerBankAccounts.AddAsync(bankAccount, cancellationToken);
        }

        // Start Search Customer
        public Task<CustomerAddress?> GetPersonalAddressForUpdateAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerAddresses
                .Where(x => x.CustomerId == customerId && x.AddressType == 1)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<CustomerBusiness?> GetCustomerBusinessForUpdateAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerBusinesses
                .Where(x => x.CustomerId == customerId && x.IsActive)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<CustomerAddress?> GetBusinessAddressForUpdateAsync(
            Guid customerBusinessId,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerAddresses
                .Where(x => x.CustomerBusinessId == customerBusinessId && x.AddressType == 2)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<CustomerBankAccount?> GetCustomerBankAccountForUpdateAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerBankAccounts
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        // END
        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        // Added Page Load for Customer
        public Task<Customer?> GetCustomerWithDetailsAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return _dbContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);
        }

        public Task<CustomerBusiness?> GetCustomerBusinessAsync(Guid customerBusinessId, CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerBusinesses
                .FirstOrDefaultAsync(x => x.CustomerBusinessId == customerBusinessId, cancellationToken);
        }

        public Task<CustomerAddress?> GetDefaultCustomerAddressAsync( Guid customerId, CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerAddresses
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId && x.AddressType == 1)
            .OrderByDescending(x => x.IsDefault)
            .ThenByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
            }

        public Task<CustomerBusiness?> GetCustomerBusinessByCustomerIdAsync( Guid customerId, CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerBusinesses
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId && x.IsActive)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<CustomerBankAccount?> GetCustomerBankAccountAsync( Guid customerId, CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerBankAccounts
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<CustomerAddress?> GetBusinessAddressAsync(Guid customerBusinessId, CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerAddresses
            .AsNoTracking()
            .Where(x => x.CustomerBusinessId == customerBusinessId && x.AddressType == 2)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
            }

        public Task<int?> GetAnyRegionIdByCountryIdAsync(
        int countryId,
        CancellationToken cancellationToken = default)
            {
            return _dbContext.Regions
                .AsNoTracking()
                .Where(x => x.CountryId == countryId
                    && x.IsActive
                    && x.RegionName == "Any")
                .Select(x => (int?)x.RegionId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<List<SalesOrderLookupOptionDto>> GetCityOptionsByRegionIdAsync(
            int regionId,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.Cities
                .AsNoTracking()
                .Where(x => x.RegionId == regionId && x.IsActive)
                .OrderBy(x => x.CityName)
                .Select(x => new SalesOrderLookupOptionDto
                {
                    Value = x.CityId.ToString(),
                    Text = x.CityName
                })
                .ToListAsync(cancellationToken);
        }

        public Task<string?> GetCityNameAsync(
            int? cityId,
            CancellationToken cancellationToken = default)
        {
            if (!cityId.HasValue)
                return Task.FromResult<string?>(null);

            return _dbContext.Cities
                .AsNoTracking()
                .Where(x => x.CityId == cityId.Value)
                .Select(x => (string?)x.CityName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<CustomerSearchResultDto>> GetCustomersCreatedByUserAsync( Guid currentUserId, CancellationToken cancellationToken = default)
        {
            return await _dbContext.Customers
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    x.CreatedByUserId == currentUserId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(50)
                .Select(x => new CustomerSearchResultDto
                {
                    CustomerId = x.CustomerId,
                    CustomerCode = x.CustomerCode ?? "",
                    DisplayName = x.DisplayName ?? "",
                    Email = x.Email ?? "",
                    Phone = x.Phone ?? "",
                    Mobile = x.Mobile ?? "",
                    IsCompanyDirector = x.IsCompanyDirector ?? false
                })
                .ToListAsync(cancellationToken);
        }

        public Task<bool> CustomerEmailExistsAsync(
        string email,
        Guid? excludeCustomerId,
        CancellationToken cancellationToken = default)
        {
            email = email.Trim();

            return _dbContext.Customers
                .AsNoTracking()
                .AnyAsync(x =>
                    x.IsActive &&
                    x.Email != null &&
                    x.Email == email &&
                    (!excludeCustomerId.HasValue || x.CustomerId != excludeCustomerId.Value),
                    cancellationToken);
        }

        public Task<bool> CustomerMobileExistsAsync(
            string mobile,
            Guid? excludeCustomerId,
            CancellationToken cancellationToken = default)
        {
            mobile = mobile.Trim();

            return _dbContext.Customers
                .AsNoTracking()
                .AnyAsync(x =>
                    x.IsActive &&
                    x.Mobile != null &&
                    x.Mobile == mobile &&
                    (!excludeCustomerId.HasValue || x.CustomerId != excludeCustomerId.Value),
                    cancellationToken);
        }

        public Task<bool> BankAccountExistsAsync(
        
        string accountNumber,
        Guid? excludeBankAccountId,
        CancellationToken cancellationToken = default)
        {
            
            accountNumber = accountNumber.Trim();

            return _dbContext.CustomerBankAccounts
                .AsNoTracking()
                .AnyAsync(x =>
                    
                    x.AccountNumber == accountNumber &&
                    (!excludeBankAccountId.HasValue || x.CustomerBankAccountId != excludeBankAccountId.Value),
                    cancellationToken);
        }

        public async Task<(List<CustomerManagementListDto> Items, int TotalRecords)> GetCustomerManagementListAsync(
        Guid? createdByUserId,
        DateTime? createdDateFrom,
        DateTime? createdDateTo,
        string? customerCode,
        string? emailOrMobile,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 20 : pageSize;
            customerCode = (customerCode ?? "").Trim();


            //var query = _dbContext.Customers
            //    .AsNoTracking()
            //    .Where(c =>
            //        c.IsActive &&
            //        (!createdByUserId.HasValue || c.CreatedByUserId == createdByUserId.Value || c.UpdatedByUserId == createdByUserId.Value ) &&
            //        (!createdDateFrom.HasValue || c.CreatedAt.Date >= createdDateFrom.Value.Date) &&
            //        (!createdDateTo.HasValue || c.CreatedAt.Date <= createdDateTo.Value.Date) &&
            //        (string.IsNullOrWhiteSpace(customerCode) || (c.CustomerCode ?? "").Contains(customerCode)));


            var query = _dbContext.Customers
                        .AsNoTracking()
                        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(emailOrMobile))
            {
                emailOrMobile = emailOrMobile.Trim();

                query = query.Where(x =>
                    (x.Email != null && x.Email.Contains(emailOrMobile)) ||
                    (x.Mobile != null && x.Mobile.Contains(emailOrMobile)));
            }
            else
            {
                query = query.Where(x =>
                    (!createdByUserId.HasValue || x.CreatedByUserId == createdByUserId.Value) &&
                    (!createdDateFrom.HasValue || x.CreatedAt.Date >= createdDateFrom.Value.Date) &&
                    (!createdDateTo.HasValue || x.CreatedAt.Date <= createdDateTo.Value.Date) &&
                    (string.IsNullOrWhiteSpace(customerCode) || x.CustomerCode.Contains(customerCode)));
            }

            var totalRecords = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerManagementListDto
                {
                    CustomerId = c.CustomerId,
                    CustomerCode = c.CustomerCode ?? "",
                    FirstName = c.FirstName ?? "",
                    LastName = c.LastName ?? "",
                    RegistrationSource = c.RegistrationSource,
                    RegistrationSourceText = ((RegistrationSource)c.RegistrationSource).ToString(),
                    Email = c.Email,
                    Phone = c.Phone,
                    Mobile = c.Mobile,
                    CreatedAt = c.CreatedAt,

                    SalesOrderCount = _dbContext.Sales.Count(s => s.CustomerId == c.CustomerId),
                    HasBusiness = _dbContext.CustomerBusinesses.Any(b => b.CustomerId == c.CustomerId && b.IsActive),
                    HasBank = _dbContext.CustomerBankAccounts.Any(b => b.CustomerId == c.CustomerId)
                })
                .ToListAsync(cancellationToken);

            return (items, totalRecords);
        }

        public async Task<List<CustomerSalesOrderListDto>> GetCustomerSalesOrdersAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var query =
                from s in _dbContext.Sales.AsNoTracking()
                join p in _dbContext.Providers.AsNoTracking()
                    on s.ProviderId equals p.ProviderId into providers
                from provider in providers.DefaultIfEmpty()
                join sl in _dbContext.SaleLines.AsNoTracking()
                    on s.SaleId equals sl.SaleId into saleLines
                where s.CustomerId == customerId
                orderby s.OrderDate descending
                select new CustomerSalesOrderListDto
                {
                    SaleId = s.SaleId,
                    OrderNo = s.OrderNo,
                    OrderDate = s.OrderDate,
                    ProviderName = provider != null ? provider.ProviderName : "SuperCRM",
                    SalesOrderStatus = s.SalesOrderStatus,
                    SalesOrderStatusText = ((SalesOrderStatus)s.SalesOrderStatus).ToString(),
                    OrderTotal = saleLines.Sum(x => x.LineTotalAmount)
                };

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<List<CustomerAddressListDto>> GetCustomerAddressesAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var query =
                from a in _dbContext.CustomerAddresses.AsNoTracking()
                join c in _dbContext.Countries.AsNoTracking()
                    on a.CountryId equals c.CountryId into countries
                from country in countries.DefaultIfEmpty()
                where a.CustomerId == customerId
                orderby a.AddressType
                select new CustomerAddressListDto
                {
                    AddressType = a.AddressType,
                    AddressTypeText = a.AddressType == 1 ? "Home/Personal"
                        : a.AddressType == 2 ? "Business"
                        : "Other",
                    HouseNo = a.HouseNo,
                    RoadName = a.RoadName,
                    PostCode = a.PostCode,
                    City = a.City,
                    CountryName = country != null ? country.CountryName : ""
                };

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<CustomerBusinessViewDto?> GetCustomerBusinessViewAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.CustomerBusinesses
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId && x.IsActive)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new CustomerBusinessViewDto
                {
                    BusinessName = x.BusinessName,
                    BusinessType = x.BusinessType,
                    BusinessTypeText = x.BusinessType == 1 ? "Solo"
                        : x.BusinessType == 2 ? "Limited"
                        : "Unknown",
                    BusinessEmail = x.BusinessEmail,
                    TradingName = x.TradingName,
                    RegistrationNo = x.RegistrationNo,
                    ContactPersonName = x.ContactPersonName,
                    ContactPersonPhone = x.ContactPersonPhone
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<CustomerBankAccountViewDto>> GetCustomerBankAccountsViewAsync(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            return await _dbContext.CustomerBankAccounts
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new CustomerBankAccountViewDto
                {
                    BankName = x.BankName,
                    AccountName = x.AccountName,
                    AccountNumber = x.AccountNumber,
                    SortCode = x.SortCode
                })
                .ToListAsync(cancellationToken);
        }

        /// END
    }
}
