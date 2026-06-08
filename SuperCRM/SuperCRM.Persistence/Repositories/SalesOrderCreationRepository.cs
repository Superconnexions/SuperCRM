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

        public async Task<(List<SalesOrderHistoryDto> Items, int TotalRecords)> GetSalesOrderHistoryAsync(
    Guid? soldByUserId,
    DateTime? orderDateFrom,
    DateTime? orderDateTo,
    byte? salesOrderStatus,
    int page,
    int pageSize,
    CancellationToken cancellationToken = default)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var salesQuery = _dbContext.Sales
                .AsNoTracking()
                .Where(s =>
                    (!soldByUserId.HasValue || s.SoldByUserId == soldByUserId.Value) &&
                    (!orderDateFrom.HasValue || s.OrderDate.Date >= orderDateFrom.Value.Date) &&
                    (!orderDateTo.HasValue || s.OrderDate.Date <= orderDateTo.Value.Date) &&
                    (!salesOrderStatus.HasValue || s.SalesOrderStatus == salesOrderStatus.Value));

            var totalRecords = await salesQuery.CountAsync(cancellationToken);

            var query =
                from s in salesQuery
                join c in _dbContext.Customers.AsNoTracking()
                    on s.CustomerId equals c.CustomerId
                join p in _dbContext.Providers.AsNoTracking()
                    on s.ProviderId equals p.ProviderId into providers
                from provider in providers.DefaultIfEmpty()
                join sl in _dbContext.SaleLines.AsNoTracking()
                    on s.SaleId equals sl.SaleId into saleLines
                orderby s.OrderDate descending
                select new SalesOrderHistoryDto
                {
                    SaleId = s.SaleId,
                    OrderNo = s.OrderNo,
                    OrderDate = s.OrderDate,

                    CustomerId = c.CustomerId,
                    CustomerCode = c.CustomerCode ?? "",
                    CustomerName = c.DisplayName
                        ?? ((c.FirstName ?? "") + " " + (c.LastName ?? "")).Trim(),
                    CustomerEmail = c.Email,
                    CustomerMobile = c.Mobile,

                    ProviderId = s.ProviderId,
                    ProviderName = provider != null ? provider.ProviderName : "SuperCRM",

                    SalesOrderStatus = s.SalesOrderStatus,
                    SalesOrderStatusText = ((SalesOrderStatus)s.SalesOrderStatus).ToString(),

                    IsCommissionApplicable = s.IsCommissionApplicable,

                    CommissionFinalizedText =
                        !saleLines.Any()
                            ? "No"
                            : saleLines.All(l => l.IsCommissionFinalized)
                                ? "Yes"
                                : saleLines.All(l => !l.IsCommissionFinalized)
                                    ? "No"
                                    : "Partial",

                    ServiceStartDate = s.ServiceStartDate,
                    NextRenewDate = s.NextRenewDate,
                    NoOfRenew = s.NoOfRenew,

                    OrderTotal = saleLines.Sum(l => l.LineTotalAmount),
                    AgentCommissionAmount = s.AgentCommissionAmount,

                    TotalLines = saleLines.Count(),
                    CompletedLines = saleLines.Count(l => l.Completed),
                    CancelledOrRejectedLines = saleLines.Count(l => l.CancelledOrRejected),

                    SoldByUserId = s.SoldByUserId,
                    SoldByAgentId = s.SoldByAgentId,
                    SoldByAgentCode = s.SoldByAgentCode,

                    EmailSentToCustomer = s.EmailSentToCustomer,
                    EmailSentToProvider = s.EmailSentToProvider
                };

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalRecords);
        }



    }
}
