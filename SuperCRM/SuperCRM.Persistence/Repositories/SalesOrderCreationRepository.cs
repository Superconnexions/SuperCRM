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
        string? salesOrderNo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
            {
                page = page <= 0 ? 1 : page;
                pageSize = pageSize <= 0 ? 20 : pageSize;


            //var salesQuery = _dbContext.Sales
            //    .AsNoTracking()
            //    .Where(s =>
            //        (!soldByUserId.HasValue || s.SoldByUserId == soldByUserId.Value) &&
            //        (!orderDateFrom.HasValue || s.OrderDate.Date >= orderDateFrom.Value.Date) &&
            //        (!orderDateTo.HasValue || s.OrderDate.Date <= orderDateTo.Value.Date) &&
            //        (!salesOrderStatus.HasValue || s.SalesOrderStatus == salesOrderStatus.Value));


            var salesQuery =
                _dbContext.Sales
                .AsNoTracking()
                .AsQueryable();


            //===================================================
            // Agent security filter
            // Agent can only see his/her own sales orders
            //===================================================

            if (soldByUserId.HasValue)
            {
                salesQuery =
                    salesQuery.Where(s =>
                        s.SoldByUserId == soldByUserId.Value);
            }


            //===================================================
            // Search by Sales Order No
            // Ignore date/status filters, but DO NOT ignore agent filter
            //===================================================

            if (!string.IsNullOrWhiteSpace(salesOrderNo))
            {
                salesOrderNo = salesOrderNo.Trim();

                salesQuery =
                    salesQuery.Where(s =>
                        s.OrderNo.Contains(salesOrderNo));
            }
            else
            {
                salesQuery =
                    salesQuery.Where(s =>

                        (!orderDateFrom.HasValue ||
                            s.OrderDate.Date >= orderDateFrom.Value.Date)

                        &&

                        (!orderDateTo.HasValue ||
                            s.OrderDate.Date <= orderDateTo.Value.Date)

                        &&

                        (!salesOrderStatus.HasValue ||
                            s.SalesOrderStatus == salesOrderStatus.Value)
                    );
            }

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

                    HasSpecialNotes = saleLines.Any(l =>!string.IsNullOrWhiteSpace(l.SpecialNotes)),

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

        public Task<Sale?> GetSaleForUpdateAsync(
        Guid saleId,
        CancellationToken cancellationToken = default)
        {
            return _dbContext.Sales
                .FirstOrDefaultAsync(x => x.SaleId == saleId, cancellationToken);
        }

        public Task<List<SaleLine>> GetSaleLinesForUpdateAsync(
            Guid saleId,
            CancellationToken cancellationToken = default)
        {
            return _dbContext.SaleLines
                .Where(x => x.SaleId == saleId)
                .ToListAsync(cancellationToken);
        }

        public async Task AddSalesOrderStatusHistoryAsync(
            SalesOrderStatusHistory history,
            CancellationToken cancellationToken = default)
        {
            await _dbContext.SalesOrderStatusHistories.AddAsync(history, cancellationToken);
        }

        public async Task<List<AdminUserOptionDto>> GetAdminUsersAsync(
        CancellationToken cancellationToken = default)
        {
            var query =
                from user in _dbContext.Users.AsNoTracking()

                join profile in _dbContext.UserProfiles.AsNoTracking()
                    on user.Id equals profile.UserId into profileJoin
                from profile in profileJoin.DefaultIfEmpty()

                join userRole in _dbContext.UserRoles.AsNoTracking()
                    on user.Id equals userRole.UserId

                join role in _dbContext.Roles.AsNoTracking()
                    on userRole.RoleId equals role.Id

                where role.Name == "SuperAdmin"
                   || role.Name == "SuperCRMAdmin"

                orderby user.Email

                select new AdminUserOptionDto
                {
                    UserId = user.Id,
                    FullName =
                        profile != null &&
                        (!string.IsNullOrWhiteSpace(profile.FirstName) ||
                         !string.IsNullOrWhiteSpace(profile.LastName))
                            ? ((profile.FirstName ?? "") + " " + (profile.LastName ?? "")).Trim()
                            : user.Email ?? user.UserName ?? user.Id.ToString()
                };

            return await query
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task<SalesOrderManagementDetailDto?> GetSalesOrderManagementDetailAsync(
            Guid saleId,
            CancellationToken cancellationToken = default)
        {
            var sale = await _dbContext.Sales
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SaleId == saleId, cancellationToken);

            if (sale == null)
                return null;

            var lines = await _dbContext.SaleLines
                .AsNoTracking()
                .Where(x => x.SaleId == saleId)
                .OrderBy(x => x.ProductName)
                .Select(x => new SalesOrderManagementLineDto
                {
                    SaleLineId = x.SaleLineId,
                    ProductName = x.ProductName ?? "",
                    VariantName = x.VariantName,
                    Quantity = x.Quantity,
                    LineTotalAmount = x.LineTotalAmount,
                    CalculatedAgentCommission = x.CalculatedAgentCommission,
                    FinalAgentCommission = x.FinalAgentCommission,
                    IsCommissionFinalized = x.IsCommissionFinalized,
                    SuperCRMCommissionEarned = x.SuperCRMCommissionEarned

                })
                .ToListAsync(cancellationToken);

            var adminUsers = await GetAdminUsersAsync(cancellationToken);

            return new SalesOrderManagementDetailDto
            {
                SaleId = sale.SaleId,
                OrderNo = sale.OrderNo,
                SalesOrderStatus = sale.SalesOrderStatus,

                ServiceStartDate = sale.ServiceStartDate,
                NextRenewDate = sale.NextRenewDate,
                NoOfRenew = sale.NoOfRenew,
                EmailSentToProvider = sale.EmailSentToProvider,
                EmailSentToCustomer = sale.EmailSentToCustomer,
                SpecialNotes = sale.SpecialNotes,

                SentToProviderDate = sale.SentToProviderDate,
                SentToProviderUserId = sale.SentToProviderUserId,

                ProviderAcceptedDate = sale.ProviderAcceptedDate,
                ProviderAcceptedUserId = sale.ProviderAcceptedUserId,

                ProviderRejectedDate = sale.ProviderRejectedDate,
                ProviderRejectedUserId = sale.ProviderRejectedUserId,

                CompletedDate = sale.CompletedDate,

                OnHoldDate = sale.OnHoldDate,
                OnHoldByUserId = sale.OnHoldByUserId,
                OnHoldReason = sale.OnHoldReason,

                CancelledDate = sale.CancelledDate,
                CancelledByUserId = sale.CancelledByUserId,
                CancelledReason = sale.CancelledReason,

                ProviderCommissionEarned = sale.ProviderCommissionEarned,
                IsProviderCommissionReceived = sale.IsProviderCommissionReceived,

                AdminUsers = adminUsers,
                Lines = lines
            };
        }

        public async Task MarkCustomerEmailSentAsync(
        List<Guid> saleIds,
        CancellationToken cancellationToken = default)
        {
            var sales = await _dbContext.Sales
                .Where(x => saleIds.Contains(x.SaleId))
                .ToListAsync(cancellationToken);

            foreach (var sale in sales)
            {
                sale.EmailSentToCustomer = true;
                sale.UpdatedAt = DateTime.UtcNow;
            }
        }

        public async Task<AgentDashboardDto> GetAgentDashboardAsync(
        Guid currentUserId,
        CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.Date;
            var last30Days = today.AddDays(-30);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var excludedStatuses = new byte[]
            {
            (byte)SalesOrderStatus.ProviderRejected,
            (byte)SalesOrderStatus.Cancelled,
            (byte)SalesOrderStatus.OnHold
            };

            var activePendingExcluded = new byte[]
            {
            (byte)SalesOrderStatus.Completed,
            (byte)SalesOrderStatus.ProviderRejected,
            (byte)SalesOrderStatus.Cancelled,
            (byte)SalesOrderStatus.OnHold
            };

            var salesQuery = _dbContext.Sales
                .AsNoTracking()
                .Where(x => x.SoldByUserId == currentUserId);

            var customerQuery = _dbContext.Customers
                .AsNoTracking()
                .Where(x =>
                    x.IsActive &&
                    (x.CreatedByUserId == currentUserId ||
                     x.UpdatedByUserId == currentUserId));

            var myCustomerCount = await customerQuery.CountAsync(cancellationToken);

            var mySalesOrderCount = await salesQuery.CountAsync(cancellationToken);

            var ordersThisMonthCount = await salesQuery
                .CountAsync(x => x.OrderDate.Date >= monthStart && x.OrderDate.Date <= today, cancellationToken);

            var pendingOrderCount = await salesQuery
                .CountAsync(x => !activePendingExcluded.Contains(x.SalesOrderStatus), cancellationToken);

            var totalCommissionUnsettled = await
                (from s in _dbContext.Sales.AsNoTracking()
                 join l in _dbContext.SaleLines.AsNoTracking()
                    on s.SaleId equals l.SaleId
                 where s.SoldByUserId == currentUserId
                    && !excludedStatuses.Contains(s.SalesOrderStatus)
                    && !l.IsCommissionFinalized
                 select (decimal?)l.CalculatedAgentCommission)
                .SumAsync(cancellationToken) ?? 0m;

            var totalCommissionSettled = await
                (from s in _dbContext.Sales.AsNoTracking()
                 join l in _dbContext.SaleLines.AsNoTracking()
                    on s.SaleId equals l.SaleId
                 where s.SoldByUserId == currentUserId
                    && !excludedStatuses.Contains(s.SalesOrderStatus)
                    && l.IsCommissionFinalized
                 select (decimal?)l.FinalAgentCommission)
                .SumAsync(cancellationToken) ?? 0m;

            var receivedCommission = await salesQuery
                .Where(x => x.IsAgentCommissionDistributed)
                .Select(x => (decimal?)x.AgentCommissionAmount)
                .SumAsync(cancellationToken) ?? 0m;

            var statusSummary = await salesQuery
                .GroupBy(x => x.SalesOrderStatus)
                .Select(g => new AgentDashboardStatusDto
                {
                    Status = g.Key,
                    StatusText = ((SalesOrderStatus)g.Key).ToString(),
                    Count = g.Count()
                })
                .OrderBy(x => x.Status)
                .ToListAsync(cancellationToken);

            var recentCustomers = await customerQuery
                .Where(x => x.CreatedAt.Date >= last30Days && x.CreatedAt.Date <= today)
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .Select(x => new AgentDashboardCustomerDto
                {
                    CustomerId = x.CustomerId,
                    CustomerCode = x.CustomerCode ?? "",
                    CustomerName = x.DisplayName ?? ((x.FirstName ?? "") + " " + (x.LastName ?? "")).Trim(),
                    Mobile = x.Mobile,
                    Email = x.Email,
                    RegistrationSourceText = ((RegistrationSource)x.RegistrationSource).ToString(),
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync(cancellationToken);

            var recentOrders =
                await (from s in _dbContext.Sales.AsNoTracking()
                       join c in _dbContext.Customers.AsNoTracking()
                            on s.CustomerId equals c.CustomerId
                       join p in _dbContext.Providers.AsNoTracking()
                            on s.ProviderId equals p.ProviderId into providers
                       from provider in providers.DefaultIfEmpty()
                       join l in _dbContext.SaleLines.AsNoTracking()
                            on s.SaleId equals l.SaleId into lines
                       where s.SoldByUserId == currentUserId
                          && s.OrderDate.Date >= last30Days
                          && s.OrderDate.Date <= today
                       orderby s.OrderDate descending
                       select new AgentDashboardOrderDto
                       {
                           SaleId = s.SaleId,
                           OrderNo = s.OrderNo,
                           OrderDate = s.OrderDate,
                           CustomerName = c.DisplayName ?? ((c.FirstName ?? "") + " " + (c.LastName ?? "")).Trim(),
                           ProviderName = provider != null ? provider.ProviderName : "SuperCRM",
                           StatusText = ((SalesOrderStatus)s.SalesOrderStatus).ToString(),
                           OrderTotal = lines.Sum(x => x.LineTotalAmount)
                       })
                      .Take(10)
                      .ToListAsync(cancellationToken);

            return new AgentDashboardDto
            {
                MyCustomerCount = myCustomerCount,
                MySalesOrderCount = mySalesOrderCount,
                OrdersThisMonthCount = ordersThisMonthCount,
                PendingOrderCount = pendingOrderCount,
                TotalCommissionUnsettled = totalCommissionUnsettled,
                TotalCommissionSettled = totalCommissionSettled,
                ReceivedCommission = receivedCommission,
                StatusSummary = statusSummary,
                RecentCustomers = recentCustomers,
                RecentOrders = recentOrders
            };
        }

        // Start Admin Dasahboard

        public async Task<List<DashboardUserOptionDto>> GetAgentUserOptionsAsync(
        CancellationToken cancellationToken = default)
        {
            var query =
                from user in _dbContext.Users.AsNoTracking()

                join profile in _dbContext.UserProfiles.AsNoTracking()
                    on user.Id equals profile.UserId into profileJoin
                from profile in profileJoin.DefaultIfEmpty()

                join agent in _dbContext.Agents.AsNoTracking()
                    on user.Id equals agent.UserId into agentJoin
                from agent in agentJoin.DefaultIfEmpty()

                join userRole in _dbContext.UserRoles.AsNoTracking()
                    on user.Id equals userRole.UserId

                join role in _dbContext.Roles.AsNoTracking()
                    on userRole.RoleId equals role.Id

                where role.Name == "Agent"

                orderby agent.AgentCode, user.Email

                select new DashboardUserOptionDto
                {
                    UserId = user.Id,
                    Email = user.Email,
                    AgentCode = agent != null ? agent.AgentCode : "",

                    FullName = profile != null &&
                               (!string.IsNullOrWhiteSpace(profile.FirstName) ||
                                !string.IsNullOrWhiteSpace(profile.LastName))
                        ? ((profile.FirstName ?? "") + " " + (profile.LastName ?? "")).Trim()
                        : user.Email ?? user.UserName ?? user.Id.ToString(),

                    DisplayText =
                        ((agent != null ? agent.AgentCode : "") + " - " +
                        (profile != null &&
                         (!string.IsNullOrWhiteSpace(profile.FirstName) ||
                          !string.IsNullOrWhiteSpace(profile.LastName))
                            ? ((profile.FirstName ?? "") + " " + (profile.LastName ?? "")).Trim()
                            : user.Email ?? user.UserName ?? user.Id.ToString()))
                };

            return await query.Distinct().ToListAsync(cancellationToken);
        }

        public async Task<List<DashboardUserOptionDto>> GetAdminUserOptionsAsync(
    CancellationToken cancellationToken = default)
        {
            var query =
                from user in _dbContext.Users.AsNoTracking()

                join profile in _dbContext.UserProfiles.AsNoTracking()
                    on user.Id equals profile.UserId into profileJoin
                from profile in profileJoin.DefaultIfEmpty()

                join userRole in _dbContext.UserRoles.AsNoTracking()
                    on user.Id equals userRole.UserId

                join role in _dbContext.Roles.AsNoTracking()
                    on userRole.RoleId equals role.Id

                where role.Name == "SuperAdmin"
                   || role.Name == "SuperCRMAdmin"

                orderby user.Email

                select new DashboardUserOptionDto
                {
                    UserId = user.Id,
                    Email = user.Email,

                    FullName = profile != null &&
                               (!string.IsNullOrWhiteSpace(profile.FirstName) ||
                                !string.IsNullOrWhiteSpace(profile.LastName))
                        ? ((profile.FirstName ?? "") + " " + (profile.LastName ?? "")).Trim()
                        : user.Email ?? user.UserName ?? user.Id.ToString(),

                    DisplayText =
                        (profile != null &&
                         (!string.IsNullOrWhiteSpace(profile.FirstName) ||
                          !string.IsNullOrWhiteSpace(profile.LastName))
                            ? ((profile.FirstName ?? "") + " " + (profile.LastName ?? "")).Trim()
                            : user.Email ?? user.UserName ?? user.Id.ToString())
                        + " - " + (user.Email ?? "")
                };

            return await query.Distinct().ToListAsync(cancellationToken);
        }

        public async Task<AdminDashboardDto> GetAdminDashboardAsync(
        string searchMode,
        Guid? agentUserId,
        Guid? adminUserId,
        CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.Date;
            var last30Days = today.AddDays(-30);
            var monthStart = new DateTime(today.Year, today.Month, 1);

            var excludedStatuses = new byte[]
            {
                (byte)SalesOrderStatus.ProviderRejected,
                (byte)SalesOrderStatus.Cancelled,
                (byte)SalesOrderStatus.OnHold
            };

            var pendingExcludedStatuses = new byte[]
            {
                (byte)SalesOrderStatus.Completed,
                (byte)SalesOrderStatus.ProviderRejected,
                (byte)SalesOrderStatus.Cancelled,
                (byte)SalesOrderStatus.OnHold
            };

            IQueryable<Sale> salesQuery = _dbContext.Sales.AsNoTracking();
            IQueryable<Customer> customerQuery = _dbContext.Customers.AsNoTracking()
                .Where(x => x.IsActive);


            // Replace Sales and Customer SQL
            switch (searchMode)
            {
                case "Agent":

                    // All Agents
                    if (!agentUserId.HasValue)
                    {
                        //salesQuery =
                        //    from s in salesQuery
                        //    where s.SoldByAgentId != null
                        //    select s;

                        salesQuery = salesQuery.Where(x => x.SoldByAgentId != null);

                        customerQuery =
                            customerQuery.Where(x =>
                                x.RegistrationSource ==
                                (byte)RegistrationSource.AgentCreated);
                    }
                    else
                    {
                        salesQuery =
                            from s in salesQuery
                            join a in _dbContext.Agents
                                on s.SoldByAgentId equals a.AgentId
                            where a.UserId == agentUserId.Value
                            select s;

                        customerQuery =
                            customerQuery.Where(x =>
                                x.CreatedByUserId == agentUserId.Value ||
                                x.UpdatedByUserId == agentUserId.Value);
                    }

                    break;

                case "Admin":

                    // All Admins
                    if (!adminUserId.HasValue)
                    {
                        salesQuery =
                            salesQuery.Where(x =>
                                x.SoldByAgentId == null);

                        customerQuery =
                            customerQuery.Where(x =>
                                x.RegistrationSource == (byte)RegistrationSource.AdminCreated);
                    }
                    else
                    {
                        salesQuery =
                            salesQuery.Where(x =>
                                x.SoldByAgentId == null &&
                                x.SoldByUserId == adminUserId.Value);

                        customerQuery =
                            customerQuery.Where(x =>
                                x.CreatedByUserId == adminUserId.Value ||
                                x.UpdatedByUserId == adminUserId.Value);
                    }

                    break;

                default:

                    // Search All
                    break;
            }



            // END

            var totalCustomerCount = await customerQuery.CountAsync(cancellationToken);
            var totalSalesOrderCount = await salesQuery.CountAsync(cancellationToken);

            var ordersThisMonthCount = await salesQuery
                .CountAsync(x => x.OrderDate.Date >= monthStart && x.OrderDate.Date <= today, cancellationToken);

            var pendingOrderCount = await salesQuery
                .CountAsync(x => !pendingExcludedStatuses.Contains(x.SalesOrderStatus), cancellationToken);

            var filteredSaleIds = salesQuery.Select(x => x.SaleId);

            var totalCommissionUnsettled = await
                (from s in _dbContext.Sales.AsNoTracking()
                 join l in _dbContext.SaleLines.AsNoTracking()
                    on s.SaleId equals l.SaleId
                 where filteredSaleIds.Contains(s.SaleId)
                    && !excludedStatuses.Contains(s.SalesOrderStatus)
                    && !l.IsCommissionFinalized
                 select (decimal?)l.CalculatedAgentCommission)
                .SumAsync(cancellationToken) ?? 0m;

            var totalCommissionSettled = await
                (from s in _dbContext.Sales.AsNoTracking()
                 join l in _dbContext.SaleLines.AsNoTracking()
                    on s.SaleId equals l.SaleId
                 where filteredSaleIds.Contains(s.SaleId)
                    && !excludedStatuses.Contains(s.SalesOrderStatus)
                    && l.IsCommissionFinalized
                 select (decimal?)l.FinalAgentCommission)
                .SumAsync(cancellationToken) ?? 0m;

            var receivedCommission = await salesQuery
                .Where(x => x.IsAgentCommissionDistributed)
                .Select(x => (decimal?)x.AgentCommissionAmount)
                .SumAsync(cancellationToken) ?? 0m;

            var receivedCommissionBySuperCRM = await salesQuery
                .Where(x => x.IsProviderCommissionReceived)
                .Select(x => (decimal?)x.ProviderCommissionEarned)
                .SumAsync(cancellationToken) ?? 0m;

            var statusSummary = await salesQuery
                .GroupBy(x => x.SalesOrderStatus)
                .Select(g => new AgentDashboardStatusDto
                {
                    Status = g.Key,
                    StatusText = ((SalesOrderStatus)g.Key).ToString(),
                    Count = g.Count()
                })
                .OrderBy(x => x.Status)
                .ToListAsync(cancellationToken);

            //var recentCustomers = await customerQuery
            //    .Where(x => x.CreatedAt.Date >= last30Days && x.CreatedAt.Date <= today)
            //    .OrderByDescending(x => x.CreatedAt)
            //    .Take(10)
            //    .Select(x => new AgentDashboardCustomerDto
            //    {
            //        CustomerId = x.CustomerId,
            //        CustomerCode = x.CustomerCode ?? "",
            //        CustomerName = x.DisplayName ?? ((x.FirstName ?? "") + " " + (x.LastName ?? "")).Trim(),
            //        Mobile = x.Mobile,
            //        Email = x.Email,
            //        RegistrationSourceText = ((RegistrationSource)x.RegistrationSource).ToString(),
            //        CreatedAt = x.CreatedAt
            //    })
            //    .ToListAsync(cancellationToken);

            var recentCustomers =
    await (from c in customerQuery
           join createdByUser in _dbContext.Users.AsNoTracking()
                on c.CreatedByUserId equals createdByUser.Id into createdByJoin
           from createdByUser in createdByJoin.DefaultIfEmpty()

           join createdByProfile in _dbContext.UserProfiles.AsNoTracking()
                on createdByUser.Id equals createdByProfile.UserId into createdByProfileJoin
           from createdByProfile in createdByProfileJoin.DefaultIfEmpty()

           join updatedByUser in _dbContext.Users.AsNoTracking()
                on c.UpdatedByUserId equals updatedByUser.Id into updatedByJoin
           from updatedByUser in updatedByJoin.DefaultIfEmpty()

           join updatedByProfile in _dbContext.UserProfiles.AsNoTracking()
                on updatedByUser.Id equals updatedByProfile.UserId into updatedByProfileJoin
           from updatedByProfile in updatedByProfileJoin.DefaultIfEmpty()

           where c.CreatedAt.Date >= last30Days &&
                 c.CreatedAt.Date <= today

           orderby c.CreatedAt descending

           select new AgentDashboardCustomerDto
           {
               CustomerId = c.CustomerId,
               CustomerCode = c.CustomerCode ?? "",
               CustomerName = c.DisplayName ?? ((c.FirstName ?? "") + " " + (c.LastName ?? "")).Trim(),
               Mobile = c.Mobile,
               Email = c.Email,
               RegistrationSourceText = ((RegistrationSource)c.RegistrationSource).ToString(),
               CreatedAt = c.CreatedAt,

               CreatedByName =
                   createdByUser == null
                       ? ""
                       : ((createdByProfile != null &&
                           (!string.IsNullOrWhiteSpace(createdByProfile.FirstName) ||
                            !string.IsNullOrWhiteSpace(createdByProfile.LastName))
                               ? ((createdByProfile.FirstName ?? "") + " " + (createdByProfile.LastName ?? "")).Trim()
                               : createdByUser.Email)
                          + " - " + createdByUser.Email),

               UpdatedByName =
                   updatedByUser == null
                       ? ""
                       : ((updatedByProfile != null &&
                           (!string.IsNullOrWhiteSpace(updatedByProfile.FirstName) ||
                            !string.IsNullOrWhiteSpace(updatedByProfile.LastName))
                               ? ((updatedByProfile.FirstName ?? "") + " " + (updatedByProfile.LastName ?? "")).Trim()
                               : updatedByUser.Email)
                          + " - " + updatedByUser.Email)
           })
          .Take(10)
          .ToListAsync(cancellationToken);

            //var recentOrders =
            //    await (from s in salesQuery
            //           join c in _dbContext.Customers.AsNoTracking()
            //                on s.CustomerId equals c.CustomerId
            //           join p in _dbContext.Providers.AsNoTracking()
            //                on s.ProviderId equals p.ProviderId into providers
            //           from provider in providers.DefaultIfEmpty()
            //           join l in _dbContext.SaleLines.AsNoTracking()
            //                on s.SaleId equals l.SaleId into lines
            //           where s.OrderDate.Date >= last30Days
            //              && s.OrderDate.Date <= today
            //           orderby s.OrderDate descending
            //           select new AgentDashboardOrderDto
            //           {
            //               SaleId = s.SaleId,
            //               OrderNo = s.OrderNo,
            //               OrderDate = s.OrderDate,
            //               CustomerName = c.DisplayName ?? ((c.FirstName ?? "") + " " + (c.LastName ?? "")).Trim(),
            //               ProviderName = provider != null ? provider.ProviderName : "SuperCRM",
            //               StatusText = ((SalesOrderStatus)s.SalesOrderStatus).ToString(),
            //               OrderTotal = lines.Sum(x => x.LineTotalAmount)
            //           })
            //          .Take(10)
            //          .ToListAsync(cancellationToken);

            var recentOrders =
    await (from s in salesQuery

           join c in _dbContext.Customers.AsNoTracking()
                on s.CustomerId equals c.CustomerId

           join p in _dbContext.Providers.AsNoTracking()
                on s.ProviderId equals p.ProviderId into providers
           from provider in providers.DefaultIfEmpty()

           join l in _dbContext.SaleLines.AsNoTracking()
                on s.SaleId equals l.SaleId into lines

           join agent in _dbContext.Agents.AsNoTracking()
                on s.SoldByAgentId equals agent.AgentId into agentJoin
           from agent in agentJoin.DefaultIfEmpty()

           join agentUser in _dbContext.Users.AsNoTracking()
                on agent.UserId equals agentUser.Id into agentUserJoin
           from agentUser in agentUserJoin.DefaultIfEmpty()

           join agentProfile in _dbContext.UserProfiles.AsNoTracking()
                on agentUser.Id equals agentProfile.UserId into agentProfileJoin
           from agentProfile in agentProfileJoin.DefaultIfEmpty()

           join adminUser in _dbContext.Users.AsNoTracking()
                on s.SoldByUserId equals adminUser.Id into adminUserJoin
           from adminUser in adminUserJoin.DefaultIfEmpty()

           join adminProfile in _dbContext.UserProfiles.AsNoTracking()
                on adminUser.Id equals adminProfile.UserId into adminProfileJoin
           from adminProfile in adminProfileJoin.DefaultIfEmpty()

           where s.OrderDate.Date >= last30Days &&
                 s.OrderDate.Date <= today

           orderby s.OrderDate descending

           select new AgentDashboardOrderDto
           {
               SaleId = s.SaleId,
               OrderNo = s.OrderNo,
               OrderDate = s.OrderDate,
               CustomerName = c.DisplayName ?? ((c.FirstName ?? "") + " " + (c.LastName ?? "")).Trim(),
               ProviderName = provider != null ? provider.ProviderName : "SuperCRM",
               StatusText = ((SalesOrderStatus)s.SalesOrderStatus).ToString(),
               OrderTotal = lines.Sum(x => x.LineTotalAmount),

               AgentDisplayName =
                   agent == null
                       ? ""
                       : agent.AgentCode + " - " +
                         (agentProfile != null &&
                          (!string.IsNullOrWhiteSpace(agentProfile.FirstName) ||
                           !string.IsNullOrWhiteSpace(agentProfile.LastName))
                            ? ((agentProfile.FirstName ?? "") + " " + (agentProfile.LastName ?? "")).Trim()
                            : agentUser.Email),

               AdminDisplayName =
                   adminUser == null
                       ? ""
                       : ((adminProfile != null &&
                           (!string.IsNullOrWhiteSpace(adminProfile.FirstName) ||
                            !string.IsNullOrWhiteSpace(adminProfile.LastName))
                               ? ((adminProfile.FirstName ?? "") + " " + (adminProfile.LastName ?? "")).Trim()
                               : adminUser.Email)
                          + " - " + adminUser.Email)
           })
          .Take(10)
          .ToListAsync(cancellationToken);

            return new AdminDashboardDto
            {
                TotalCustomerCount = totalCustomerCount,
                TotalSalesOrderCount = totalSalesOrderCount,
                OrdersThisMonthCount = ordersThisMonthCount,
                PendingOrderCount = pendingOrderCount,
                TotalCommissionUnsettled = totalCommissionUnsettled,
                TotalCommissionSettled = totalCommissionSettled,
                ReceivedCommission = receivedCommission,
                ReceivedCommissionBySuperCRM = receivedCommissionBySuperCRM,
                StatusSummary = statusSummary,
                RecentCustomers = recentCustomers,
                RecentOrders = recentOrders,
                SelectedAgentUserId = agentUserId,
                SelectedAdminUserId = adminUserId,
                AgentOptions = await GetAgentUserOptionsAsync(cancellationToken),
                AdminOptions = await GetAdminUserOptionsAsync(cancellationToken)
            };
        }

        // END Admin Dashboard

        public async Task<AgentsKpiDto> GetAgentsKpiAsync(
        DateTime orderDateFrom,
        DateTime orderDateTo,
        Guid? agentId,
        byte? salesOrderStatus,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
        {
            page = page <= 0 ? 1 : page;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var fromDate = orderDateFrom.Date;
            var toDate = orderDateTo.Date.AddDays(1).AddTicks(-1);

            var excludedStatusesForCommission = new byte[]
            {
            (byte)SalesOrderStatus.ProviderRejected,
            (byte)SalesOrderStatus.Cancelled,
            (byte)SalesOrderStatus.OnHold
            };

            var agentQuery =
                from agent in _dbContext.Agents.AsNoTracking()
                join user in _dbContext.Users.AsNoTracking()
                    on agent.UserId equals user.Id
                join profile in _dbContext.UserProfiles.AsNoTracking()
                    on user.Id equals profile.UserId into profileJoin
                from profile in profileJoin.DefaultIfEmpty()
                orderby agent.AgentCode
                select new
                {
                    agent.AgentId,
                    agent.UserId,
                    agent.AgentCode,
                    user.Email,
                    FirstName = profile != null ? profile.FirstName : "",
                    LastName = profile != null ? profile.LastName : "",
                    MobileNo = profile != null ? profile.MobileNo : ""
                };

            if (agentId.HasValue)
            {
                agentQuery = agentQuery.Where(x => x.AgentId == agentId.Value);
            }

            var totalRecords = await agentQuery.CountAsync(cancellationToken);

            var agents = await agentQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            var rows = new List<AgentsKpiRowDto>();

            foreach (var agent in agents)
            {
                var salesQuery = _dbContext.Sales.AsNoTracking()
                    .Where(s =>
                        s.SoldByAgentId == agent.AgentId &&
                        s.OrderDate >= fromDate &&
                        s.OrderDate <= toDate);

                if (salesOrderStatus.HasValue)
                {
                    salesQuery = salesQuery.Where(s =>
                        s.SalesOrderStatus == salesOrderStatus.Value);
                }

                var totalSalesOrder = await salesQuery.CountAsync(cancellationToken);

                var totalCustomer = await _dbContext.Customers.AsNoTracking()
                    .CountAsync(c =>
                        c.CreatedByUserId == agent.UserId ||
                        c.UpdatedByUserId == agent.UserId,
                        cancellationToken);

                var commissionUnsettled = await
                    (from sale in salesQuery
                     join line in _dbContext.SaleLines.AsNoTracking()
                        on sale.SaleId equals line.SaleId
                     where !line.IsCommissionFinalized
                        && !excludedStatusesForCommission.Contains(sale.SalesOrderStatus)
                     select (decimal?)line.CalculatedAgentCommission)
                    .SumAsync(cancellationToken) ?? 0m;

                var commissionSettled = await
                    (from sale in salesQuery
                     join line in _dbContext.SaleLines.AsNoTracking()
                        on sale.SaleId equals line.SaleId
                     where line.IsCommissionFinalized
                        && !excludedStatusesForCommission.Contains(sale.SalesOrderStatus)
                     select (decimal?)line.FinalAgentCommission)
                    .SumAsync(cancellationToken) ?? 0m;

                var commissionDistributed = await salesQuery
                    .Where(s => s.IsAgentCommissionDistributed)
                    .Select(s => (decimal?)s.AgentCommissionAmount)
                    .SumAsync(cancellationToken) ?? 0m;

                var statusCounts = await salesQuery
                    .GroupBy(s => s.SalesOrderStatus)
                    .Select(g => new AgentsKpiStatusCountDto
                    {
                        Status = g.Key,
                        StatusText = ((SalesOrderStatus)g.Key).ToString(),
                        Count = g.Count()
                    })
                    .OrderBy(x => x.Status)
                    .ToListAsync(cancellationToken);

                rows.Add(new AgentsKpiRowDto
                {
                    AgentId = agent.AgentId,
                    AgentCode = agent.AgentCode,
                    FullName = ((agent.FirstName ?? "") + " " + (agent.LastName ?? "")).Trim(),
                    Email = agent.Email ?? "",
                    Mobile = agent.MobileNo,
                    TotalCustomer = totalCustomer,
                    TotalSalesOrder = totalSalesOrder,
                    CommissionUnsettled = commissionUnsettled,
                    CommissionSettled = commissionSettled,
                    CommissionDistributed = commissionDistributed,
                    StatusCounts = statusCounts
                });
            }

            var agentOptions = await
                (from agent in _dbContext.Agents.AsNoTracking()
                 join user in _dbContext.Users.AsNoTracking()
                    on agent.UserId equals user.Id
                 join profile in _dbContext.UserProfiles.AsNoTracking()
                    on user.Id equals profile.UserId into profileJoin
                 from profile in profileJoin.DefaultIfEmpty()
                 orderby agent.AgentCode
                 select new AgentsKpiAgentOptionDto
                 {
                     AgentId = agent.AgentId,
                     AgentCode = agent.AgentCode,
                     FullName =
                        profile != null &&
                        (!string.IsNullOrWhiteSpace(profile.FirstName) ||
                         !string.IsNullOrWhiteSpace(profile.LastName))
                            ? ((profile.FirstName ?? "") + " " + (profile.LastName ?? "")).Trim()
                            : user.Email ?? ""
                 })
                .ToListAsync(cancellationToken);

            return new AgentsKpiDto
            {
                OrderDateFrom = orderDateFrom,
                OrderDateTo = orderDateTo,
                AgentId = agentId,
                SalesOrderStatus = salesOrderStatus,
                Page = page,
                PageSize = pageSize,
                TotalRecords = totalRecords,
                AgentOptions = agentOptions,
                Items = rows
            };
        }


        public async Task SaveDraftLineSpecialNotesAsync(
        Guid salesOrderDraftId,
        List<SaveDraftLineSpecialNoteDto> notes,
        CancellationToken cancellationToken = default)
        {
            if (notes == null || !notes.Any())
                return;

            var noteMap = notes
                .Where(x => x.SalesOrderDraftLineId != Guid.Empty)
                .GroupBy(x => x.SalesOrderDraftLineId)
                .ToDictionary(
                    x => x.Key,
                    x => string.IsNullOrWhiteSpace(x.First().SpecialNotes)
                        ? null
                        : x.First().SpecialNotes!.Trim());

            var draftLines = await _dbContext.SalesOrderDraftLines
                .Where(x => x.SalesOrderDraftId == salesOrderDraftId)
                .ToListAsync(cancellationToken);

            foreach (var line in draftLines)
            {
                if (noteMap.TryGetValue(line.SalesOrderDraftLineId, out var note))
                {
                    line.SpecialNotes = note;
                }
            }
        }

        public async Task<List<ProductVariantCommissionOverrideDto>> GetActiveProductVariantCommissionOverridesAsync(
        List<Guid> productIds,
        DateTime orderDate,
        CancellationToken cancellationToken = default)
        {
            if (productIds == null || !productIds.Any())
                return new List<ProductVariantCommissionOverrideDto>();

            return await _dbContext.ProductVariantCommissionOverrides
                .AsNoTracking()
                .Where(x =>
                    productIds.Contains(x.ProductId) &&
                    x.IsActive &&
                    (!x.EffectiveFrom.HasValue || x.EffectiveFrom.Value <= orderDate) &&
                    (!x.EffectiveTo.HasValue || x.EffectiveTo.Value >= orderDate))
                .Select(x => new ProductVariantCommissionOverrideDto
                {
                    ProductId = x.ProductId,
                    ProductCode = x.ProductCode,
                    ProductVariantId = x.ProductVariantId,
                    VariantCode = x.VariantCode,
                    ExtraCommissionAmount = x.ExtraCommissionAmount
                })
                .ToListAsync(cancellationToken);
        }

        // END

    }
}
