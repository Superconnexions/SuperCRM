using SuperCRM.Application.DTOs.SalesOrders;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Entities;
using SuperCRM.Domain.Enums;

namespace SuperCRM.Application.Services
{
    public class SalesOrderCreationService : ISalesOrderCreationService
    {
        private readonly ISalesOrderCreationRepository _repository;

        public SalesOrderCreationService(ISalesOrderCreationRepository repository)
        {
            _repository = repository;
        }

        public async Task<CreateSalesOrderResultDto> CreateSalesOrderFromDraftAsync(
            CreateSalesOrderFromDraftRequestDto request,
            CancellationToken cancellationToken = default)
        {
            if (request.CurrentUserId == Guid.Empty)
                return Fail(request.SalesOrderDraftId, "Invalid login session.");

            var draft = await _repository.GetDraftWithLinesAsync(request.SalesOrderDraftId, cancellationToken);
            if (draft == null)
                return Fail(request.SalesOrderDraftId, "Sales order draft was not found.");

            if (draft.DraftStatus == 3)
                return Fail(request.SalesOrderDraftId, "Sales order has already been created for this draft.");

            if (!draft.CustomerId.HasValue || draft.CustomerId.Value == Guid.Empty)
                return Fail(request.SalesOrderDraftId, "Please save or select customer before creating sales order.");

            if (draft.DraftLines == null || !draft.DraftLines.Any())
                return Fail(request.SalesOrderDraftId, "No product lines were found in this draft.");

            var customer = await _repository.GetCustomerAsync(draft.CustomerId.Value, cancellationToken);
            if (customer == null)
                return Fail(request.SalesOrderDraftId, "Selected customer was not found.");

            Guid? soldByAgentId = null;
            string? soldByAgentCode = null;

            var agent = await _repository.GetAgentByUserIdAsync( request.CurrentUserId, cancellationToken);

            if (agent != null)
            {

                soldByAgentId = agent.AgentId;
                soldByAgentCode = agent.AgentCode;
            }

            var orderDate = DateTime.UtcNow;
            var productIds = draft.DraftLines.Select(x => x.ProductId).Distinct().ToList();
            var products = await _repository.GetProductsByIdsAsync(productIds, cancellationToken);
            var productMap = products.ToDictionary(x => x.ProductId, x => x);

            //var specialNoteMap = request.LineSpecialNotes
            //                    .Where(x => x.SalesOrderDraftLineId != Guid.Empty)
            //                    .ToDictionary(
            //                        x => x.SalesOrderDraftLineId,
            //                        x => string.IsNullOrWhiteSpace(x.SpecialNotes)
            //                    ? null
            //                    : x.SpecialNotes.Trim());

            var commissions = await _repository.GetActiveProductBaseCommissionsAsync(productIds, orderDate, cancellationToken);
            var commissionMap = commissions
                .GroupBy(x => x.ProductId)
                .ToDictionary(x => x.Key, x => x.OrderByDescending(c => c.EffectiveFrom ?? DateTime.MinValue).First());

            var createdSaleIds = new List<Guid>();

            // Sales order split by Provdier product wise

            //var salesGroups = draft.DraftLines
            //    .GroupBy(x => x.ProviderId)
            //    .OrderBy(x => x.Key.HasValue ? 1 : 0)
            //    .ThenBy(x => x.First().ProviderName ?? "SuperCRM");

            // Sales order split by each product of Provdier product except SuperCRM

            var salesGroups = draft.DraftLines
                            .Select((line, index) => new
                            {
                                Line = line,
                                Index = index
                            })
                            .GroupBy(x => new
                            {
                                // Provider product: each selected item creates separate Sale
                                GroupKey = x.Line.ProviderProductId.HasValue
                                    ? x.Line.SalesOrderDraftLineId
                                    : x.Line.ProviderId,

                                ProviderId = x.Line.ProviderId
                            })
                            .OrderBy(x => x.Key.ProviderId.HasValue ? 1 : 0)
                            .ThenBy(x => x.First().Line.ProviderName ?? "SuperCRM")
                            .ThenBy(x => x.First().Index);


            foreach (var group in salesGroups)
            {
                var saleId = Guid.NewGuid();

                var groupLines = group.Select(x => x.Line).ToList();

                var sale = new Sale
                {
                    SaleId = saleId,
                    OrderNo = await GenerateOrderNoAsync(cancellationToken),
                    CustomerId = draft.CustomerId.Value,
                    CustomerBusinessId = draft.CustomerBusinessId,
                    //ProviderId = group.Key,
                    ProviderId = group.Key.ProviderId,
                    OrderSourceType = (byte)request.OrderSourceType,     // From Controller
                    SoldByAgentId = soldByAgentId,
                    SoldByAgentCode = soldByAgentCode,
                    SaleChannelType = (byte)OrderChannel.WebPortal, // default:
                    SoldByUserId = request.CurrentUserId,
                    IsCommissionApplicable = false,
                    OrderDate = orderDate,
                    OrderStatus = "Created",

                    SalesOrderStatus = (byte)SalesOrderStatus.Created,
                    EmailSentToCustomer = false,
                    EmailSentToProvider = false,
                    NoOfRenew = 0,

                    ProviderCommissionEarned = 0,
                    AgentCommissionAmount = 0,
                    IsProviderCommissionReceived = false,
                    IsAgentCommissionDistributed = false,
                    CreatedByUserId = request.CurrentUserId,
                    CreatedAt = orderDate
                };

                await _repository.AddSaleAsync(sale, cancellationToken);

                var saleCommissionTotal = 0m;
                var saleOrderNo = sale.OrderNo;


                //foreach (var draftLine in group)
                  foreach (var draftLine in groupLines)
                  {
                    productMap.TryGetValue(draftLine.ProductId, out var product);
                    commissionMap.TryGetValue(draftLine.ProductId, out var commission);

                    var quantity = draftLine.Quantity <= 0 ? 1 : draftLine.Quantity;
                    var lineTotal = draftLine.LineTotalAmount > 0
                        ? draftLine.LineTotalAmount
                        : draftLine.SalePrice * quantity;

                    var commissionAmount = CalculateCommission(commission, lineTotal, quantity);
                    saleCommissionTotal += commissionAmount;

                    var firstInstallmentDate = draftLine.IsInstallmentSelected
                        ? orderDate.Date.AddMonths(1)
                        : (DateTime?)null;

                    var saleLine = new SaleLine
                    {
                        SaleLineId = Guid.NewGuid(),
                        SaleId = sale.SaleId,
                        
                        Completed = false,
                        CompletedDate = null,
                        CancelledOrRejected = false,
                        CancelledOrRejectedDate = null,

                        ProductId = draftLine.ProductId,
                        ProductCode = draftLine.ProductCode,
                        ProductName = draftLine.ProductName,
                        ProductVariantId = draftLine.ProductVariantId,
                        VariantCode = draftLine.VariantCode,
                        VariantName = draftLine.VariantName,
                        ProviderProductId = draftLine.ProviderProductId,
                        Quantity = quantity,
                        // Special Notes
                        SpecialNotes = draftLine.SpecialNotes,
                        Remarks = null,

                        SettledQty = 0,
                        PaidQty = 0,
                        BasePriceType = draftLine.BasePriceType,
                        BasePrice = draftLine.BasePrice,
                        SalePrice = draftLine.SalePrice,
                        IsSalePriceEdited = draftLine.IsPriceEditable && draftLine.SalePrice != draftLine.BasePrice,
                        PriceFinalizedAt = orderDate,
                        PriceFinalizedByUserId = request.CurrentUserId,
                        ProductBaseCommissionId = commission?.ProductBaseCommissionId,
                        CommissionType = commission?.CommissionType,
                        CommissionValue = commission?.CommissionType == CommissionType.FixedAmount
                            ? commission.FixedAmount
                            : commission?.Percentage,
                        CalculatedAgentCommission = commissionAmount,
                        //FinalAgentCommission = commissionAmount,
                        //SuperCRMCommissionEarned = commissionAmount,
                        FinalAgentCommission = 0, // finalyze by admin
                        SuperCRMCommissionEarned = 0, // finalyze by admin
                        //IsCommissionFinalized = commission != null,
                        IsCommissionFinalized = false, // Commission finalyze by admin
                        CreatedAt = orderDate,
                        SalesUnitId = product?.SalesUnitId ?? 0,
                        SalesUnitCode = product?.SalesUnitCode ?? string.Empty,
                        LineTotalAmount = lineTotal,
                        MonthlyInstallmentAmount = draftLine.IsInstallmentSelected ? draftLine.MonthlyInstallmentAmount : null,
                        NoOfInstallment = draftLine.IsInstallmentSelected ? draftLine.NoOfInstallment : null,
                        FirstInstallmentDate = firstInstallmentDate
                    };


                    await _repository.AddSaleLineAsync(saleLine, cancellationToken);


                    if (draftLine.IsInstallmentSelected &&
                        draftLine.NoOfInstallment.HasValue && draftLine.NoOfInstallment.Value > 0 &&
                        draftLine.MonthlyInstallmentAmount.HasValue && draftLine.MonthlyInstallmentAmount.Value > 0)
                    {
                        await CreateInstallmentSchedulesAsync(
                            saleLine,
                            customer,
                            saleOrderNo,
                            orderDate,
                            draftLine.NoOfInstallment.Value,
                            draftLine.MonthlyInstallmentAmount.Value,
                            cancellationToken);
                    }
                }

                
                sale.ProviderCommissionEarned = 0;  // Default = 0

                sale.AgentCommissionAmount = saleCommissionTotal;
                sale.IsCommissionApplicable = saleCommissionTotal > 0;

                createdSaleIds.Add(sale.SaleId);
            }

            draft.DraftStatus = 3; // SalesOrderCreated / Confirmed
            draft.UpdatedAt = orderDate;
            draft.UpdatedByUserId = request.CurrentUserId;

            await _repository.SaveChangesAsync(cancellationToken);

            return new CreateSalesOrderResultDto
            {
                Success = true,
                Message = "Sales order created successfully.",
                SalesOrderDraftId = draft.SalesOrderDraftId,
                SaleIds = createdSaleIds
            };
        }

        private async Task<string> GenerateOrderNoAsync(CancellationToken cancellationToken)
        {
            var prefix = "SO";
            var datePart = DateTime.UtcNow.ToString("yyMM");

            var random = Random.Shared.Next(1000, 9999);

            return  $"{prefix}-{datePart}-{random}";
        }

        public async Task<SalesOrderCreatedSummaryDto?> GetCreatedSalesOrderSummaryAsync(
            List<Guid> saleIds,
            CancellationToken cancellationToken = default)
        {
            saleIds = saleIds.Where(x => x != Guid.Empty).Distinct().ToList();
            if (!saleIds.Any()) return null;

            var sales = await _repository.GetSalesByIdsAsync(saleIds, cancellationToken);
            if (!sales.Any()) return null;

            var customer = await _repository.GetCustomerAsync(sales.First().CustomerId, cancellationToken);
            if (customer == null) return null;

            CustomerBusiness? business = null;
            if (sales.First().CustomerBusinessId.HasValue)
                business = await _repository.GetCustomerBusinessAsync(sales.First().CustomerBusinessId.Value, cancellationToken);

            var homeAddress = await _repository.GetHomeAddressAsync(customer.CustomerId, cancellationToken);
            CustomerAddress? businessAddress = null;
            if (business != null)
                businessAddress = await _repository.GetBusinessAddressAsync(business.CustomerBusinessId, cancellationToken);

            var lineItems = await _repository.GetSaleLinesBySaleIdsAsync(saleIds, cancellationToken);
            var productIds = lineItems.Select(x => x.ProductId).Distinct().ToList();
            var products = await _repository.GetProductsByIdsAsync(productIds, cancellationToken);
            var productMap = products.ToDictionary(x => x.ProductId, x => x);

            var installments = await _repository.GetInstallmentSchedulesBySaleLineIdsAsync(
                lineItems.Select(x => x.SaleLineId).ToList(), cancellationToken);

            var providerIds = sales.Where(x => x.ProviderId.HasValue).Select(x => x.ProviderId!.Value).Distinct().ToList();
            var providers = await _repository.GetProvidersByIdsAsync(providerIds, cancellationToken);
            var providerMap = providers.ToDictionary(x => x.ProviderId, x => x);

            var summary = new SalesOrderCreatedSummaryDto
            {
                SalesOrderDraftId = Guid.Empty,
                DraftNo = string.Empty,
                Customer = new SalesOrderCustomerSummaryDto
                {
                    CustomerId = customer.CustomerId,
                    CustomerCode = customer.CustomerCode ?? string.Empty,
                    DisplayName = customer.DisplayName ?? (customer.FirstName + " " + customer.LastName).Trim(),
                    Email = customer.Email,
                    Phone = customer.Phone,
                    Mobile = customer.Mobile
                },
                Business = business == null ? null : new SalesOrderBusinessSummaryDto
                {
                    CustomerBusinessId = business.CustomerBusinessId,
                    BusinessName = business.BusinessName,
                    BusinessEmail = business.BusinessEmail,
                    TradingName = business.TradingName,
                    RegistrationNo = business.RegistrationNo,
                    ContactPersonName = business.ContactPersonName,
                    ContactPersonPhone = business.ContactPersonPhone,
                    BusinessType = (byte)business.BusinessType
                },
                HomeAddress = await MapAddressAsync(homeAddress, cancellationToken),
                BusinessAddress = await MapAddressAsync(businessAddress, cancellationToken),
                Orders = new List<SalesOrderProviderSummaryDto>()
            };

            foreach (var sale in sales.OrderBy(x => x.ProviderId.HasValue ? 1 : 0).ThenBy(x => x.OrderDate))
            {
                Provider? provider = null;
                if (sale.ProviderId.HasValue) providerMap.TryGetValue(sale.ProviderId.Value, out provider);

                var saleLines = lineItems.Where(x => x.SaleId == sale.SaleId).ToList();
                var saleLineIds = saleLines.Select(x => x.SaleLineId).ToList();

                var hasResidentialLines = saleLines.Any(x =>
                    productMap.TryGetValue(x.ProductId, out var p) &&
                    (p.CustomerType == ProductCustomerType.Residential || p.CustomerType == ProductCustomerType.Both));

                var hasBusinessLines = saleLines.Any(x =>
                    productMap.TryGetValue(x.ProductId, out var p) &&
                    (p.CustomerType == ProductCustomerType.Business || p.CustomerType == ProductCustomerType.Both));

                summary.Orders.Add(new SalesOrderProviderSummaryDto
                {
                    SaleId = sale.SaleId,
                    //OrderNo = BuildOrderNo(sale),
                    OrderNo = sale.OrderNo,
                    ProviderId = sale.ProviderId,
                    ProviderName = provider?.ProviderName ?? "SuperCRM",
                    ProviderEmail = provider?.ContactEmail,
                    OrderDate = sale.OrderDate,
                    OrderStatus = sale.OrderStatus ?? string.Empty,

                    
                    SalesOrderStatus = sale.SalesOrderStatus,
                    SalesOrderStatusText = ((SalesOrderStatus)sale.SalesOrderStatus).ToString(),

                    OrderTotal = saleLines.Sum(x => x.LineTotalAmount),
                    AgentCommissionAmount = sale.AgentCommissionAmount,
                    HasResidentialLines = hasResidentialLines,
                    HasBusinessLines = hasBusinessLines,
                    Lines = saleLines.Select(x => new SalesOrderLineSummaryDto
                    {
                        SaleLineId = x.SaleLineId,
                        
                        SpecialNotes = x.SpecialNotes,
                        ProductName = x.ProductName ?? string.Empty,
                        VariantName = x.VariantName,
                        Quantity = x.Quantity,
                        SalesUnitCode = x.SalesUnitCode,
                        UnitPrice = x.SalePrice,
                        LineTotalAmount = x.LineTotalAmount,
                        IsInstallment = x.NoOfInstallment.HasValue && x.NoOfInstallment.Value > 0,
                        MonthlyInstallmentAmount = x.MonthlyInstallmentAmount,
                        NoOfInstallment = x.NoOfInstallment,
                        CalculatedAgentCommission = x.CalculatedAgentCommission,
                        FinalAgentCommission = x.FinalAgentCommission
                    }).ToList(),
                    Installments = installments
                        .Where(i => saleLineIds.Contains(i.SaleLineId))
                        .OrderBy(i => i.InstallmentNo)
                        .Select(i => new SalesOrderInstallmentScheduleSummaryDto
                        {
                            InstallmentScheduleId = i.InstallmentScheduleId,
                            SaleLineId = i.SaleLineId,
                            ProductName = saleLines.FirstOrDefault(l => l.SaleLineId == i.SaleLineId)?.ProductName ?? string.Empty,
                            InstallmentNo = i.InstallmentNo,
                            InstallmentAmount = i.InstallmentAmount,
                            DueDate = i.DueDate,
                            PaymentStatus = i.PaymentStatus,
                            PaymentStatusText = ((InstallmentPaymentStatus)i.PaymentStatus).ToString()
                        }).ToList()
                });
            }

            return summary;
        }

        private async Task CreateInstallmentSchedulesAsync(
            SaleLine saleLine,
            Customer customer,
            string orderNo,
            DateTime orderDate,
            int noOfInstallment,
            decimal monthlyInstallmentAmount,
            CancellationToken cancellationToken)
        {
            for (var i = 1; i <= noOfInstallment; i++)
            {
                var dueDate = orderDate.Date.AddMonths(i);
                var schedule = new InstallmentSchedule
                {
                    InstallmentScheduleId = Guid.NewGuid(),
                    SaleLineId = saleLine.SaleLineId,
                    CustomerId = customer.CustomerId,
                    OrderNo = orderNo,
                    CustomerCode = customer.CustomerCode ?? string.Empty,
                    InstallmentNo = i,
                    InstallmentAmount = monthlyInstallmentAmount,
                    DueDate = dueDate,
                    PaymentStatus = 1, // Demo default: Pending/Unpaid
                    PaidAmount = null,
                    PaidDate = null,
                    CollectedByUserId = null,
                    Remarks = null,
                    PaymentNotes = null,
                    CreatedAt = DateTime.UtcNow
                };

                await _repository.AddInstallmentScheduleAsync(schedule, cancellationToken);
            }
        }

        private static decimal CalculateCommission(ProductBaseCommission? commission, decimal lineTotal, int quantity)
        {
            if (commission == null) return 0m;

            if (commission.CommissionType == CommissionType.FixedAmount)
                return Math.Round((commission.FixedAmount ?? 0m) * quantity, 2);

            if (commission.CommissionType == CommissionType.Percentage)
                return Math.Round(lineTotal * ((commission.Percentage ?? 0m) / 100m), 2);

            return 0m;
        }

        private async Task<SalesOrderAddressSummaryDto?> MapAddressAsync(CustomerAddress? address, CancellationToken cancellationToken)
        {
            if (address == null) return null;

            return new SalesOrderAddressSummaryDto
            {
                HouseNo = address.HouseNo,
                RoadName = address.RoadName,
                PostCode = address.PostCode,
                City = address.City,
                AddressLine = address.AddressLine,
                CountryName = await _repository.GetCountryNameAsync(address.CountryId, cancellationToken),
                RegionName = await _repository.GetRegionNameAsync(address.RegionId, cancellationToken)
            };
        }

        //public static string BuildOrderNo(Sale sale)
        //{
        //    return $"SO-{sale.OrderDate:yyyyMMdd}-{sale.SaleId.ToString("N")[..8].ToUpper()}";
        //}

        private static CreateSalesOrderResultDto Fail(Guid draftId, string message)
        {
            return new CreateSalesOrderResultDto
            {
                Success = false,
                Message = message,
                SalesOrderDraftId = draftId
            };
        }

        public async Task<bool> CanCreateSalesOrderAsync(
        Guid userId,
        bool isAgent,
        CancellationToken cancellationToken = default)
        {
            if (!isAgent)
                return true;

            return await _repository.IsApprovedAgentAsync(userId, cancellationToken);
        }

        public async Task<Agent?> GetAgentByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
        {
            return await _repository.GetAgentByUserIdAsync(userId, cancellationToken);

        }

        public Task<(List<SalesOrderHistoryDto> Items, int TotalRecords)> GetSalesOrderHistoryAsync(
        Guid? soldByUserId,
        DateTime? orderDateFrom,
        DateTime? orderDateTo,
        byte? salesOrderStatus,
        string? salesOrderNo,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
            {
                return _repository.GetSalesOrderHistoryAsync(
                    soldByUserId,
                    orderDateFrom,
                    orderDateTo,
                    salesOrderStatus,
                    salesOrderNo,
                    page,
                    pageSize,
                    cancellationToken);
            }

        public Task<SalesOrderManagementDetailDto?> GetSalesOrderManagementDetailAsync(
        Guid saleId,
        CancellationToken cancellationToken = default)
        {
            return _repository.GetSalesOrderManagementDetailAsync(saleId, cancellationToken);
        }

        public async Task<bool> UpdateSalesInformationAsync(
            UpdateSalesInformationDto request,
            CancellationToken cancellationToken = default)
        {
            var sale = await _repository.GetSaleForUpdateAsync(request.SaleId, cancellationToken);
            if (sale == null) return false;

            sale.ServiceStartDate = request.ServiceStartDate;
            sale.NextRenewDate = request.NextRenewDate;
            sale.NoOfRenew = request.NoOfRenew;
            sale.EmailSentToProvider = request.EmailSentToProvider;
            sale.EmailSentToCustomer = request.EmailSentToCustomer;
            sale.SpecialNotes = request.SpecialNotes;
            sale.UpdatedAt = DateTime.UtcNow;
            sale.UpdatedByUserId = request.UpdatedByUserId;

            await _repository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> UpdateSalesCommissionAsync(
            UpdateSalesCommissionDto request,
            CancellationToken cancellationToken = default)
        {
            var sale = await _repository.GetSaleForUpdateAsync(request.SaleId, cancellationToken);
            if (sale == null) return false;

            var lines = await _repository.GetSaleLinesForUpdateAsync(request.SaleId, cancellationToken);

            foreach (var inputLine in request.Lines)
            {
                var line = lines.FirstOrDefault(x => x.SaleLineId == inputLine.SaleLineId);
                if (line == null) continue;

                line.FinalAgentCommission = inputLine.FinalAgentCommission;
                line.IsCommissionFinalized = true;
                line.UpdatedAt = DateTime.UtcNow;
                line.UpdatedByUserId = request.UpdatedByUserId;
            }

            sale.AgentCommissionAmount = lines.Sum(x => x.FinalAgentCommission);
            sale.UpdatedAt = DateTime.UtcNow;
            sale.UpdatedByUserId = request.UpdatedByUserId;

            await _repository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> UpdateSalesOrderStatusAsync(
            UpdateSalesOrderStatusDto request,
            CancellationToken cancellationToken = default)
        {
            var sale = await _repository.GetSaleForUpdateAsync(request.SaleId, cancellationToken);
            if (sale == null) return false;

            var oldStatus = sale.SalesOrderStatus;

            sale.SalesOrderStatus = request.SalesOrderStatus;
            sale.OrderStatus = ((SalesOrderStatus)request.SalesOrderStatus).ToString();
            sale.UpdatedAt = DateTime.UtcNow;
            sale.UpdatedByUserId = request.UpdatedByUserId;

            var status = (SalesOrderStatus)request.SalesOrderStatus;

            switch (status)
            {
                case SalesOrderStatus.SentToProvider:
                    sale.SentToProviderDate = request.SentToProviderDate;
                    sale.SentToProviderUserId = request.SentToProviderUserId;
                    break;

                case SalesOrderStatus.ProviderAccepted:
                    sale.ProviderAcceptedDate = request.ProviderAcceptedDate;
                    sale.ProviderAcceptedUserId = request.ProviderAcceptedUserId;
                    break;

                case SalesOrderStatus.ProviderRejected:
                    sale.ProviderRejectedDate = request.ProviderRejectedDate;
                    sale.ProviderRejectedUserId = request.ProviderRejectedUserId;
                    break;

                case SalesOrderStatus.Completed:
                    sale.CompletedDate = request.CompletedDate;
                    sale.ServiceStartDate = request.ServiceStartDate;
                    sale.NextRenewDate = request.NextRenewDate;
                    break;

                case SalesOrderStatus.OnHold:
                    sale.OnHoldDate = request.OnHoldDate;
                    sale.OnHoldByUserId = request.OnHoldByUserId;
                    sale.OnHoldReason = request.OnHoldReason;
                    break;

                case SalesOrderStatus.Cancelled:
                    sale.CancelledDate = request.CancelledDate;
                    sale.CancelledByUserId = request.CancelledByUserId;
                    sale.CancelledReason = request.CancelledReason;
                    break;
            }

            await _repository.AddSalesOrderStatusHistoryAsync(new SalesOrderStatusHistory
            {
                SalesOrderStatusHistoryId = Guid.NewGuid(),
                SaleId = sale.SaleId,
                OldStatus = oldStatus,
                NewStatus = request.SalesOrderStatus,
                Remarks = BuildStatusHistoryRemarks(request),
                ChangedByUserId = request.UpdatedByUserId,
                ChangedAt = DateTime.UtcNow
            }, cancellationToken);

            await _repository.SaveChangesAsync(cancellationToken);
            return true;
        }

        private static string? BuildStatusHistoryRemarks(UpdateSalesOrderStatusDto request)
        {
            var status = (SalesOrderStatus)request.SalesOrderStatus;

            return status switch
            {
                SalesOrderStatus.OnHold => request.OnHoldReason,
                SalesOrderStatus.Cancelled => request.CancelledReason,
                SalesOrderStatus.SentToProvider => "Order sent to provider.",
                SalesOrderStatus.ProviderAccepted => "Provider accepted the order.",
                SalesOrderStatus.ProviderRejected => "Provider rejected the order.",
                SalesOrderStatus.Completed => "Order completed.",
                _ => status.ToString()
            };
        }

        public async Task<bool> UpdateSuperCRMCommissionAsync(
    UpdateSuperCRMCommissionDto request,
    CancellationToken cancellationToken = default)
        {
            var sale = await _repository.GetSaleForUpdateAsync(request.SaleId, cancellationToken);
            if (sale == null)
                return false;

            var lines = await _repository.GetSaleLinesForUpdateAsync(request.SaleId, cancellationToken);

            foreach (var inputLine in request.Lines)
            {
                var line = lines.FirstOrDefault(x => x.SaleLineId == inputLine.SaleLineId);
                if (line == null)
                    continue;

                line.SuperCRMCommissionEarned = inputLine.SuperCRMCommissionEarned;
                line.UpdatedAt = DateTime.UtcNow;
                line.UpdatedByUserId = request.UpdatedByUserId;
            }

            sale.ProviderCommissionEarned = lines.Sum(x => x.SuperCRMCommissionEarned);
            sale.IsProviderCommissionReceived = request.IsProviderCommissionReceived;
            sale.UpdatedAt = DateTime.UtcNow;
            sale.UpdatedByUserId = request.UpdatedByUserId;

            await _repository.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task MarkCustomerEmailSentAsync(
        List<Guid> saleIds,
        CancellationToken cancellationToken = default)
        {
            if (saleIds == null || !saleIds.Any())
                return;

            await _repository.MarkCustomerEmailSentAsync(saleIds, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
        }

        public Task<AgentDashboardDto> GetAgentDashboardAsync(
        Guid currentUserId,
        CancellationToken cancellationToken = default)
        {
            return _repository.GetAgentDashboardAsync(
                currentUserId,
                cancellationToken);
        }

        public Task<AdminDashboardDto> GetAdminDashboardAsync(
        string searchMode,
        Guid? agentUserId,
        Guid? adminUserId,
        CancellationToken cancellationToken = default)
        {
            return _repository.GetAdminDashboardAsync(
                searchMode,
                agentUserId,
                adminUserId,
                cancellationToken);
        }

        public Task<AgentsKpiDto> GetAgentsKpiAsync(
        DateTime orderDateFrom,
        DateTime orderDateTo,
        Guid? agentId,
        byte? salesOrderStatus,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
        {
            return _repository.GetAgentsKpiAsync(
                orderDateFrom,
                orderDateTo,
                agentId,
                salesOrderStatus,
                page,
                pageSize,
                cancellationToken);
        }


        public async Task SaveDraftLineSpecialNotesAsync(
        Guid salesOrderDraftId,
        List<SaveDraftLineSpecialNoteDto> notes,
        CancellationToken cancellationToken = default)
        {
            await _repository.SaveDraftLineSpecialNotesAsync(
                salesOrderDraftId,
                notes,
                cancellationToken);
        }


        // END
    }
}
