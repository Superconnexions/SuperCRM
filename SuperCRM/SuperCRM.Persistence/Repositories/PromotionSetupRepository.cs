using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.DTOs.PromotionSetup;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Domain.Entities;
using SuperCRM.Domain.Enums;
using SuperCRM.Persistence.DbContexts;

namespace SuperCRM.Persistence.Repositories
{
    public class PromotionSetupRepository : IPromotionSetupRepository
    {
        private readonly SuperCrmDbContext _db;

        public PromotionSetupRepository(
            SuperCrmDbContext db)
        {
            _db = db;
        }

        // -----------------------------------------------------
        // PROMOTION LIST / HISTORY SEARCH
        // -----------------------------------------------------

        public async Task<PromotionListResultDto> SearchAsync(
            PromotionListFilterDto filter,
            CancellationToken ct = default)
        {
            var query =
                _db.PromotionSetups
                    .AsNoTracking()
                    .AsQueryable();

            if (!string.IsNullOrWhiteSpace(
                filter.PromotionName))
            {
                var promotionName =
                    filter.PromotionName.Trim();

                query = query.Where(x =>
                    EF.Functions.Like(
                        x.PromotionName,
                        $"%{promotionName}%"));
            }

            // Search the exact Promotion Start Date.
            // A range is used instead of .Date so SQL can use an index efficiently.
            if (filter.PromotionStartDate.HasValue)
            {
                var startDate =
                    filter.PromotionStartDate.Value.Date;

                var nextDate =
                    startDate.AddDays(1);

                query = query.Where(x =>
                    x.PromotionStartDate >= startDate
                    && x.PromotionStartDate < nextDate);
            }

            // Search the exact Promotion End Date.
            if (filter.PromotionEndDate.HasValue)
            {
                var endDate =
                    filter.PromotionEndDate.Value.Date;

                var nextDate =
                    endDate.AddDays(1);

                query = query.Where(x =>
                    x.PromotionEndDate >= endDate
                    && x.PromotionEndDate < nextDate);
            }

            if (filter.Published.HasValue)
            {
                query = query.Where(x =>
                    x.Published == filter.Published.Value);
            }

            var totalRecords =
                await query.CountAsync(ct);

            var totalPages =
                totalRecords == 0
                    ? 0
                    : (int)Math.Ceiling(
                        totalRecords
                        / (double)filter.PageSize);

            var page =
                filter.Page;

            if (totalPages > 0
                && page > totalPages)
            {
                page = totalPages;
            }

            var skip =
                (page - 1)
                * filter.PageSize;

            var items =
                await query
                    .OrderByDescending(x =>
                        x.SubmittedAt)
                    .ThenByDescending(x =>
                        x.PromotionCode)
                    .Skip(skip)
                    .Take(filter.PageSize)
                    .Select(x =>
                        new PromotionListItemDto
                        {
                            PromotionId =
                                x.PromotionId,

                            PromotionCode =
                                x.PromotionCode,

                            PromotionName =
                                x.PromotionName,

                            PromotionStartDate =
                                x.PromotionStartDate,

                            PromotionEndDate =
                                x.PromotionEndDate,

                            Published =
                                x.Published,

                            IsActive =
                                x.IsActive,

                            Cancelled =
                                x.Cancelled,

                            NoOfEmailNotificationSend =
                                x.NoOfEmailNotificationSend,

                            TotalProducts =
                                x.Items.Count(),

                            ActiveProducts =
                                x.Items.Count(i =>
                                    i.IsActive),

                            SubmittedAt =
                                x.SubmittedAt,

                            UpdatedAt =
                                x.UpdatedAt
                        })
                    .ToListAsync(ct);

            return new PromotionListResultDto
            {
                Items = items,
                Page = page,
                PageSize = filter.PageSize,
                TotalRecords = totalRecords,
                TotalPages = totalPages
            };
        }

        // -----------------------------------------------------
        // GET BY ID
        // -----------------------------------------------------

        public Task<PromotionSetup?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default)
        {
            return _db.PromotionSetups
                .Include(x => x.Items)
                    .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(
                    x => x.PromotionId == id,
                    ct);
        }

        // -----------------------------------------------------
        // PRODUCT / BASE COMMISSION LOOKUP
        // -----------------------------------------------------

        public async Task<List<PromotionProductLookupDto>>
            GetProductCommissionOptionsAsync(
                DateTime asOfDate,
                CancellationToken ct = default)
        {
            var rows =
                await _db.ProductBaseCommissions
                    .AsNoTracking()
                    .Include(x => x.Product)
                    .Where(x =>
                        x.IsActive
                        && x.Product != null
                        && x.Product.IsActive
                        && (x.EffectiveFrom == null
                            || x.EffectiveFrom <= asOfDate)
                        && (x.EffectiveTo == null
                            || x.EffectiveTo >= asOfDate)
                        && x.FixedAmount != null)
                    .OrderBy(x =>
                        x.Product!.ProductName)
                    .ThenByDescending(x =>
                        x.EffectiveFrom)
                    .ToListAsync(ct);

            return rows
                .GroupBy(x => x.ProductId)
                .Select(g => g.First())
                .Select(x =>
                    new PromotionProductLookupDto
                    {
                        ProductId =
                            x.ProductId,

                        ProductBaseCommissionId =
                            x.ProductBaseCommissionId,

                        ProductCode =
                            x.Product!.ProductCode,

                        ProductName =
                            x.Product.ProductName,

                        StandardCommission =
                            x.FixedAmount!.Value
                    })
                .ToList();
        }

        public Task<ProductBaseCommission?>
            GetProductBaseCommissionAsync(
                Guid id,
                CancellationToken ct = default)
        {
            return _db.ProductBaseCommissions
                .Include(x => x.Product)
                .FirstOrDefaultAsync(
                    x =>
                        x.ProductBaseCommissionId == id
                        && x.IsActive,
                    ct);
        }

        // -----------------------------------------------------
        // PROMOTION CODE
        // -----------------------------------------------------

        public async Task<int> GetNextMonthlySequenceAsync(
            int year,
            int month,
            CancellationToken ct = default)
        {
            var prefix =
                $"PRO{year % 100:00}{month:00}";

            var codes =
                await _db.PromotionSetups
                    .AsNoTracking()
                    .Where(x =>
                        x.PromotionCode.StartsWith(prefix))
                    .Select(x =>
                        x.PromotionCode)
                    .ToListAsync(ct);

            var max =
                codes
                    .Select(x =>
                        int.TryParse(
                            x.Substring(prefix.Length),
                            out var n)
                            ? n
                            : 0)
                    .DefaultIfEmpty(0)
                    .Max();

            return max + 1;
        }

        public Task<bool> PromotionCodeExistsAsync(
            string code,
            CancellationToken ct = default)
        {
            return _db.PromotionSetups
                .AnyAsync(
                    x => x.PromotionCode == code,
                    ct);
        }

        // -----------------------------------------------------
        // ACTIVE AGENT EMAILS
        // -----------------------------------------------------

        public Task<List<string>> GetActiveAgentEmailsAsync(
            CancellationToken ct = default)
        {
            return (
                from a in _db.Agents.AsNoTracking()
                join u in _db.Users.AsNoTracking()
                    on a.UserId equals u.Id
                where a.IsApproved
                    && a.RegistrationStatus
                        == (byte)AgentRegistrationStatus.Active
                    && u.Email != null
                    && u.Email != ""
                select u.Email!)
                .Distinct()
                .ToListAsync(ct);
        }

        // -----------------------------------------------------
        // SAVE
        // -----------------------------------------------------

        public Task AddAsync(
            PromotionSetup entity,
            CancellationToken ct = default)
        {
            return _db.PromotionSetups
                .AddAsync(
                    entity,
                    ct)
                .AsTask();
        }

        public void RemoveItem(
            PromotionItem item)
        {
            _db.PromotionItems.Remove(item);
        }

        public Task SaveChangesAsync(
            CancellationToken ct = default)
        {
            return _db.SaveChangesAsync(ct);
        }


        public async Task<List<PromotionProductConflictDto>>
    GetActivePromotionProductConflictsAsync(
        Guid currentPromotionId,
        List<Guid> productIds,
        DateTime today,
        CancellationToken ct = default)
        {
            if (productIds == null
                || productIds.Count == 0)
            {
                return new List<PromotionProductConflictDto>();
            }

            var checkDate =
                today.Date;

            return await _db.PromotionSetups
                .AsNoTracking()

                // Exclude the promotion currently being published.
                .Where(p =>
                    p.PromotionId != currentPromotionId

                    // Existing promotion must already be published.
                    && p.Published

                    // Promotion itself must be active.
                    && p.IsActive

                    // Cancelled promotions must not block products.
                    && !p.Cancelled

                    // Promotion ending today is still active.
                    && p.PromotionEndDate >= checkDate)

                // Check only active items/products.
                .SelectMany(
                    p => p.Items
                        .Where(i =>
                            i.IsActive
                            && productIds.Contains(i.ProductId)),
                    (p, i) =>
                        new PromotionProductConflictDto
                        {
                            ProductId =
                                i.ProductId,

                            ProductCode =
                                i.Product != null
                                    ? i.Product.ProductCode
                                    : string.Empty,

                            ProductName =
                                i.Product != null
                                    ? i.Product.ProductName
                                    : string.Empty,

                            PromotionId =
                                p.PromotionId,

                            PromotionCode =
                                p.PromotionCode,

                            PromotionName =
                                p.PromotionName,

                            PromotionEndDate =
                                p.PromotionEndDate
                        })

                .OrderBy(x => x.ProductName)
                .ThenBy(x => x.PromotionCode)

                .ToListAsync(ct);
        }

        public async Task<List<ApplicablePromotionCommissionDto>>
    GetApplicablePromotionCommissionsAsync(
        List<Guid> productIds,
        DateTime saleDate,
        CancellationToken ct = default)
        {
            if (productIds == null
                || productIds.Count == 0)
            {
                return new List<ApplicablePromotionCommissionDto>();
            }

            var checkDate =
                saleDate.Date;

            return await _db.PromotionSetups
                .AsNoTracking()
                .Where(p =>
                    p.Published
                    && p.IsActive
                    && !p.Cancelled
                    && p.PromotionStartDate <= checkDate
                    && p.PromotionEndDate >= checkDate)
                .SelectMany(
                    p => p.Items
                        .Where(i =>
                            i.IsActive
                            && productIds.Contains(i.ProductId)),
                    (p, i) =>
                        new ApplicablePromotionCommissionDto
                        {
                            PromotionId =
                                p.PromotionId,

                            PromotionCode =
                                p.PromotionCode,

                            PromotionName =
                                p.PromotionName,

                            PromotionItemId =
                                i.PromotionItemId,

                            ProductId =
                                i.ProductId,

                            StandardCommission =
                                i.StandardCommission,

                            PromotionAmount =
                                i.PromotionAmount,

                            FinalCommissionAmount =
                                i.FinalCommissionAmount
                        })
                .ToListAsync(ct);
        }



        // -----------------------------------------------------
        // AGENT ACTIVE PROMOTIONS / NOTIFICATION
        // -----------------------------------------------------

        public async Task<List<AgentActivePromotionDto>> GetAgentActivePromotionsAsync(
            Guid agentUserId,
            DateTime currentDate,
            CancellationToken ct = default)
        {
            var checkDate = currentDate.Date;

            return await _db.PromotionSetups
                .AsNoTracking()
                .Where(p =>
                    p.Published
                    && p.Submitted
                    && p.IsActive
                    && !p.Cancelled
                    && p.PromotionStartDate <= checkDate
                    && p.PromotionEndDate >= checkDate
                    && p.Items.Any(i => i.IsActive))
                .OrderByDescending(p => p.PromotionStartDate)
                .ThenByDescending(p => p.PromotionCode)
                .Select(p => new AgentActivePromotionDto
                {
                    PromotionId = p.PromotionId,
                    PromotionCode = p.PromotionCode,
                    PromotionName = p.PromotionName,
                    PromotionSummary = p.PromotionSummary,
                    PromotionStartDate = p.PromotionStartDate,
                    PromotionEndDate = p.PromotionEndDate,
                    ActiveItemCount = p.Items.Count(i => i.IsActive),

                    // NEW is Agent-specific: no view row exists yet.
                    IsNew = !_db.AgentPromotionViews.Any(v =>
                        v.AgentUserId == agentUserId
                        && v.PromotionId == p.PromotionId)
                })
                .ToListAsync(ct);
        }

        public async Task<AgentPromotionNotificationDto> GetAgentPromotionNotificationAsync(
            Guid agentUserId,
            DateTime currentDate,
            CancellationToken ct = default)
        {
            var checkDate = currentDate.Date;

            var query = _db.PromotionSetups
                .AsNoTracking()
                .Where(p =>
                    p.Published
                    && p.Submitted
                    && p.IsActive
                    && !p.Cancelled
                    && p.PromotionStartDate <= checkDate
                    && p.PromotionEndDate >= checkDate
                    && p.Items.Any(i => i.IsActive));

            var activePromotionCount = await query.CountAsync(ct);

            var newPromotionCount = await query.CountAsync(
                p => !_db.AgentPromotionViews.Any(v =>
                    v.AgentUserId == agentUserId
                    && v.PromotionId == p.PromotionId),
                ct);

            return new AgentPromotionNotificationDto
            {
                ActivePromotionCount = activePromotionCount,
                NewPromotionCount = newPromotionCount
            };
        }

        public Task<AgentPromotionDetailsDto?> GetAgentPromotionItemsAsync(
            Guid promotionId,
            DateTime currentDate,
            CancellationToken ct = default)
        {
            var checkDate = currentDate.Date;

            return _db.PromotionSetups
                .AsNoTracking()
                .Where(p =>
                    p.PromotionId == promotionId
                    && p.Published
                    && p.Submitted
                    && p.IsActive
                    && !p.Cancelled
                    && p.PromotionStartDate <= checkDate
                    && p.PromotionEndDate >= checkDate
                    && p.Items.Any(i => i.IsActive))
                .Select(p => new AgentPromotionDetailsDto
                {
                    PromotionId = p.PromotionId,
                    PromotionCode = p.PromotionCode,
                    PromotionName = p.PromotionName,
                    Items = p.Items
                        .Where(i => i.IsActive)
                        .OrderBy(i => i.Product != null
                            ? i.Product.ProductName
                            : string.Empty)
                        .Select(i => new AgentPromotionItemDto
                        {
                            PromotionItemId = i.PromotionItemId,
                            PromotionCode = p.PromotionCode,
                            ProductCode = i.Product != null
                                ? i.Product.ProductCode
                                : string.Empty,
                            ProductName = i.Product != null
                                ? i.Product.ProductName
                                : string.Empty,
                            StandardCommission = i.StandardCommission,
                            PromotionAmount = i.PromotionAmount,
                            FinalCommissionAmount = i.FinalCommissionAmount
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync(ct);
        }

        public async Task MarkAgentPromotionViewedAsync(
            Guid promotionId,
            Guid agentUserId,
            DateTime viewedAt,
            CancellationToken ct = default)
        {
            // Idempotent for normal use. The database unique index is the final safeguard.
            var alreadyViewed = await _db.AgentPromotionViews
                .AnyAsync(v =>
                    v.AgentUserId == agentUserId
                    && v.PromotionId == promotionId,
                    ct);

            if (alreadyViewed)
            {
                return;
            }

            _db.AgentPromotionViews.Add(
                new AgentPromotionView
                {
                    AgentPromotionViewId = Guid.NewGuid(),
                    PromotionId = promotionId,
                    AgentUserId = agentUserId,
                    ViewedAt = viewedAt
                });

            await _db.SaveChangesAsync(ct);
        }

        // END

    }
}
