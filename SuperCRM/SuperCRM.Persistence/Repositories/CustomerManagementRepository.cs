using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.DTOs.SalesOrders;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Domain.Enums;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    public class CustomerManagementRepository : ICustomerManagementRepository
    {
        private readonly SuperCrmDbContext _dbContext;

        public CustomerManagementRepository(SuperCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Customer?> GetCustomerForEditAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return _dbContext.Customers.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.IsActive, cancellationToken);
        }

        public Task<bool> CanEditCustomerAsync(Guid customerId, Guid currentUserId, bool canManageAllCustomers, CancellationToken cancellationToken = default)
        {
            return _dbContext.Customers.AsNoTracking().AnyAsync(x =>
                x.CustomerId == customerId &&
                x.IsActive &&
                (canManageAllCustomers || x.CreatedByUserId == currentUserId), cancellationToken);
        }

        public Task<List<CustomerEditAddressDto>> GetAddressesForEditAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerAddresses
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId)
                .OrderBy(x => x.AddressType)
                .ThenByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.CreatedAt)
                .Select(x => new CustomerEditAddressDto
                {
                    CustomerAddressId = x.CustomerAddressId,
                    CustomerId = customerId,
                    CustomerBusinessId = x.CustomerBusinessId,
                    AddressType = x.AddressType,
                    AddressTypeText = x.AddressType == (byte)AddressType.Personal ? "Personal" :
                                      x.AddressType == (byte)AddressType.Business ? "Business" :
                                      x.AddressType == (byte)AddressType.Shipping ? "Shipping" : "Unknown",
                    AddressLine = x.AddressLine,
                    HouseNo = x.HouseNo,
                    RoadName = x.RoadName,
                    PostCode = x.PostCode,
                    City = x.City,
                    CountryId = x.CountryId,
                    RegionId = x.RegionId,
                    CityId = x.CityId,
                    CountryName = x.Country == null ? null : x.Country.CountryName,
                    RegionName = x.Region == null ? null : x.Region.RegionName,
                    IsDefault = x.IsDefault
                })
                .ToListAsync(cancellationToken);
        }

        public Task<CustomerBusiness?> GetBusinessForEditAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerBusinesses
                .Where(x => x.CustomerId == customerId && x.IsActive)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<CustomerBankAccount?> GetBankAccountForEditAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerBankAccounts
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<CustomerAddress?> GetAddressForUpdateAsync(Guid customerId, Guid addressId, CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerAddresses.FirstOrDefaultAsync(
                x => x.CustomerId == customerId && x.CustomerAddressId == addressId,
                cancellationToken);
        }

        public async Task ResetDefaultAddressAsync(Guid customerId, byte addressType, Guid? exceptAddressId, CancellationToken cancellationToken = default)
        {
            var addresses = await _dbContext.CustomerAddresses
                .Where(x => x.CustomerId == customerId &&
                            x.AddressType == addressType &&
                            x.IsDefault &&
                            (!exceptAddressId.HasValue || x.CustomerAddressId != exceptAddressId.Value))
                .ToListAsync(cancellationToken);

            foreach (var address in addresses)
            {
                address.IsDefault = false;
            }
        }

        public Task<bool> CustomerEmailExistsAsync(string email, Guid excludeCustomerId, CancellationToken cancellationToken = default)
        {
            var normalized = email.Trim().ToLower();
            return _dbContext.Customers.AsNoTracking().AnyAsync(
                x => x.CustomerId != excludeCustomerId &&
                     x.Email != null &&
                     x.Email.Trim().ToLower() == normalized,
                cancellationToken);
        }

        public Task<bool> CustomerMobileExistsAsync(string mobile, Guid excludeCustomerId, CancellationToken cancellationToken = default)
        {
            var normalized = mobile.Trim();
            return _dbContext.Customers.AsNoTracking().AnyAsync(
                x => x.CustomerId != excludeCustomerId &&
                     x.Mobile != null &&
                     x.Mobile.Trim() == normalized,
                cancellationToken);
        }

        public Task<bool> BankAccountExistsAsync(string accountNumber, Guid? excludeBankAccountId, CancellationToken cancellationToken = default)
        {
            var normalized = accountNumber.Trim();
            return _dbContext.CustomerBankAccounts.AsNoTracking().AnyAsync(
                x => (!excludeBankAccountId.HasValue || x.CustomerBankAccountId != excludeBankAccountId.Value) &&
                     x.AccountNumber != null &&
                     x.AccountNumber.Trim() == normalized,
                cancellationToken);
        }

        public Task<List<SalesOrderLookupOptionDto>> GetCountryOptionsAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.Countries.AsNoTracking()
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
            return _dbContext.Regions.AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.RegionName)
                .Select(x => new SalesOrderLookupOptionDto
                {
                    Value = x.RegionId.ToString(),
                    Text = x.RegionName
                })
                .ToListAsync(cancellationToken);
        }

        public Task AddAddressAsync(CustomerAddress address, CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerAddresses.AddAsync(address, cancellationToken).AsTask();
        }

        public Task AddBusinessAsync(CustomerBusiness business, CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerBusinesses.AddAsync(business, cancellationToken).AsTask();
        }

        public Task AddBankAccountAsync(CustomerBankAccount bankAccount, CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerBankAccounts.AddAsync(bankAccount, cancellationToken).AsTask();
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
