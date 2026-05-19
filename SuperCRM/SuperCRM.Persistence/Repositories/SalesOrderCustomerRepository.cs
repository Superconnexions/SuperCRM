using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.DTOs.SalesOrders;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
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

        /// END
    }
}
