using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.DTOs.SalesOrders;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Domain.Enums;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    public class SalesOrderCreationRepository : ISalesOrderCreationRepository
    {
        private readonly SuperCrmDbContext _dbContext;

        public SalesOrderCreationRepository(SuperCrmDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<SalesOrderDraft?> GetDraftWithLinesAsync(Guid draftId, CancellationToken cancellationToken = default)
        {
            return _dbContext.SalesOrderDrafts
                .Include(x => x.DraftLines)
                .FirstOrDefaultAsync(x => x.SalesOrderDraftId == draftId, cancellationToken);
        }

        public Task<Customer?> GetCustomerAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return _dbContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerId == customerId, cancellationToken);
        }

        public Task<CustomerBusiness?> GetCustomerBusinessAsync(Guid customerBusinessId, CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerBusinesses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerBusinessId == customerBusinessId, cancellationToken);
        }

        public Task<CustomerAddress?> GetHomeAddressAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerAddresses
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId && x.AddressType == 1)
                .OrderByDescending(x => x.IsDefault)
                .ThenByDescending(x => x.CreatedAt)
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

        public Task<CustomerBankAccount?> GetCustomerBankAccountAsync(Guid customerBankAccountId, CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerBankAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerBankAccountId == customerBankAccountId, cancellationToken);
        }

        public Task<List<Product>> GetProductsByIdsAsync(List<Guid> productIds, CancellationToken cancellationToken = default)
        {
            return _dbContext.Products
                .AsNoTracking()
                .Where(x => productIds.Contains(x.ProductId))
                .ToListAsync(cancellationToken);
        }

        public Task<List<ProductBaseCommission>> GetActiveProductBaseCommissionsAsync(List<Guid> productIds, DateTime orderDate, CancellationToken cancellationToken = default)
        {
            return _dbContext.ProductBaseCommissions
                .AsNoTracking()
                .Where(x => productIds.Contains(x.ProductId)
                    && x.IsActive
                    && (!x.EffectiveFrom.HasValue || x.EffectiveFrom.Value <= orderDate)
                    && (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= orderDate))
                .ToListAsync(cancellationToken);
        }

        public Task<List<Provider>> GetProvidersByIdsAsync(List<Guid> providerIds, CancellationToken cancellationToken = default)
        {
            if (providerIds == null || providerIds.Count == 0)
                return Task.FromResult(new List<Provider>());

            return _dbContext.Providers
                .AsNoTracking()
                .Where(x => providerIds.Contains(x.ProviderId))
                .ToListAsync(cancellationToken);
        }

        public async Task AddSaleAsync(Sale sale, CancellationToken cancellationToken = default)
        {
            await _dbContext.Sales.AddAsync(sale, cancellationToken);
        }

        public async Task AddSaleLineAsync(SaleLine saleLine, CancellationToken cancellationToken = default)
        {
            await _dbContext.SaleLines.AddAsync(saleLine, cancellationToken);
        }

        public async Task AddInstallmentScheduleAsync(InstallmentSchedule schedule, CancellationToken cancellationToken = default)
        {
            await _dbContext.InstallmentSchedules.AddAsync(schedule, cancellationToken);
        }

        public Task<List<Sale>> GetSalesByIdsAsync(List<Guid> saleIds, CancellationToken cancellationToken = default)
        {
            return _dbContext.Sales
                .AsNoTracking()
                .Where(x => saleIds.Contains(x.SaleId))
                .ToListAsync(cancellationToken);
        }

        public Task<List<SaleLine>> GetSaleLinesBySaleIdsAsync(List<Guid> saleIds, CancellationToken cancellationToken = default)
        {
            return _dbContext.SaleLines
                .AsNoTracking()
                .Where(x => saleIds.Contains(x.SaleId))
                .ToListAsync(cancellationToken);
        }

        public Task<List<InstallmentSchedule>> GetInstallmentSchedulesBySaleLineIdsAsync(List<Guid> saleLineIds, CancellationToken cancellationToken = default)
        {
            if (saleLineIds == null || saleLineIds.Count == 0)
                return Task.FromResult(new List<InstallmentSchedule>());

            return _dbContext.InstallmentSchedules
                .AsNoTracking()
                .Where(x => saleLineIds.Contains(x.SaleLineId))
                .ToListAsync(cancellationToken);
        }

        public Task<CustomerAddress?> GetAddressByIdAsync(Guid addressId, CancellationToken cancellationToken = default)
        {
            return _dbContext.CustomerAddresses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerAddressId == addressId, cancellationToken);
        }

        public Task<string?> GetCountryNameAsync(int? countryId, CancellationToken cancellationToken = default)
        {
            if (!countryId.HasValue) return Task.FromResult<string?>(null);

            return _dbContext.Countries
                .AsNoTracking()
                .Where(x => x.CountryId == countryId.Value)
                .Select(x => (string?)x.CountryName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<string?> GetRegionNameAsync(int? regionId, CancellationToken cancellationToken = default)
        {
            if (!regionId.HasValue) return Task.FromResult<string?>(null);

            return _dbContext.Regions
                .AsNoTracking()
                .Where(x => x.RegionId == regionId.Value)
                .Select(x => (string?)x.RegionName)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<Agent?> GetAgentByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
        {
            return _dbContext.Agents
                .AsNoTracking()
                .Where(x =>
                    x.UserId == userId &&
                    x.IsApproved )
                .FirstOrDefaultAsync(cancellationToken);
        }

        public Task<bool> IsApprovedAgentAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
        {
            return _dbContext.Agents
                .AsNoTracking()
                .AnyAsync(x =>
                    x.UserId == userId &&
                    x.IsApproved,
                    cancellationToken);
        }


        // Sales Order History

        public async Task<List<SalesOrderHistoryDto>> GetSalesOrderHistoryAsync(
        Guid? soldByUserId,
        CancellationToken cancellationToken = default)
        {
            var query =
                from s in _dbContext.Sales.AsNoTracking()
                join c in _dbContext.Customers.AsNoTracking()
                    on s.CustomerId equals c.CustomerId
                join sl in _dbContext.SaleLines.AsNoTracking()
                    on s.SaleId equals sl.SaleId into saleLines
                join p in _dbContext.Providers.AsNoTracking()
                    on s.ProviderId equals p.ProviderId into providers
                from provider in providers.DefaultIfEmpty()
                where !soldByUserId.HasValue || s.SoldByUserId == soldByUserId.Value
                select new
                {
                    Sale = s,
                    Customer = c,
                    Provider = provider,
                    Lines = saleLines
                };

            var result = await query
                .OrderByDescending(x => x.Sale.OrderDate)
                .Select(x => new SalesOrderHistoryDto
                {
                    SaleId = x.Sale.SaleId,
                    OrderNo = x.Sale.OrderNo,
                    OrderDate = x.Sale.OrderDate,

                    CustomerId = x.Customer.CustomerId,
                    CustomerCode = x.Customer.CustomerCode ?? "",
                    CustomerName = x.Customer.DisplayName
                        ?? ((x.Customer.FirstName ?? "") + " " + (x.Customer.LastName ?? "")).Trim(),
                    CustomerEmail = x.Customer.Email,
                    CustomerMobile = x.Customer.Mobile,

                    ProviderId = x.Sale.ProviderId,
                    ProviderName = x.Provider != null ? x.Provider.ProviderName : "SuperCRM",

                    SalesOrderStatus = x.Sale.SalesOrderStatus,
                    SalesOrderStatusText = ((SalesOrderStatus)x.Sale.SalesOrderStatus).ToString(),
                    OrderStatus = x.Sale.OrderStatus ?? "",

                    OrderTotal = x.Lines.Sum(l => l.LineTotalAmount),
                    AgentCommissionAmount = x.Sale.AgentCommissionAmount,

                    TotalLines = x.Lines.Count(),
                    CompletedLines = x.Lines.Count(l => l.Completed),
                    CancelledOrRejectedLines = x.Lines.Count(l => l.CancelledOrRejected),

                    SoldByUserId = (Guid)x.Sale.SoldByUserId,
                    SoldByAgentId = x.Sale.SoldByAgentId,
                    SoldByAgentCode = x.Sale.SoldByAgentCode,

                    EmailSentToCustomer = x.Sale.EmailSentToCustomer,
                    EmailSentToProvider = x.Sale.EmailSentToProvider
                })
                .Take(500)
                .ToListAsync(cancellationToken);

            return result;
        }



    }
}
