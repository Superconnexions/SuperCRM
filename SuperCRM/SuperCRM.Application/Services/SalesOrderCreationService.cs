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

            var orderDate = DateTime.UtcNow;
            var productIds = draft.DraftLines.Select(x => x.ProductId).Distinct().ToList();
            var products = await _repository.GetProductsByIdsAsync(productIds, cancellationToken);
            var productMap = products.ToDictionary(x => x.ProductId, x => x);

            var commissions = await _repository.GetActiveProductBaseCommissionsAsync(productIds, orderDate, cancellationToken);
            var commissionMap = commissions
                .GroupBy(x => x.ProductId)
                .ToDictionary(x => x.Key, x => x.OrderByDescending(c => c.EffectiveFrom ?? DateTime.MinValue).First());

            var createdSaleIds = new List<Guid>();
            var salesGroups = draft.DraftLines
                .GroupBy(x => x.ProviderId)
                .OrderBy(x => x.Key.HasValue ? 1 : 0)
                .ThenBy(x => x.First().ProviderName ?? "SuperCRM");

            foreach (var group in salesGroups)
            {
                var saleId = Guid.NewGuid();
                var sale = new Sale
                {
                    SaleId = saleId,
                    CustomerId = draft.CustomerId.Value,
                    CustomerBusinessId = draft.CustomerBusinessId,
                    ProviderId = group.Key,
                    OrderSourceType = 1,     // Demo default: Agent/Admin entry
                    SaleChannelType = 1,     // Demo default: Web/Admin portal
                    SoldByUserId = request.CurrentUserId,
                    SoldByAgentId = null,
                    SoldByAgentCode = null,
                    IsCommissionApplicable = false,
                    OrderDate = orderDate,
                    OrderStatus = "Created",
                    ProviderCommissionEarned = 0,
                    AgentCommissionAmount = 0,
                    IsProviderCommissionReceived = false,
                    IsAgentCommissionDistributed = false,
                    CreatedByUserId = request.CurrentUserId,
                    CreatedAt = orderDate
                };

                await _repository.AddSaleAsync(sale, cancellationToken);

                var saleCommissionTotal = 0m;
                var saleOrderNo = BuildOrderNo(sale);

                foreach (var draftLine in group)
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
                        ProductId = draftLine.ProductId,
                        ProductCode = draftLine.ProductCode,
                        ProductName = draftLine.ProductName,
                        ProductVariantId = draftLine.ProductVariantId,
                        VariantCode = draftLine.VariantCode,
                        VariantName = draftLine.VariantName,
                        ProviderProductId = draftLine.ProviderProductId,
                        Quantity = quantity,
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
                        FinalAgentCommission = commissionAmount,
                        SuperCRMCommissionEarned = commissionAmount,
                        IsCommissionFinalized = commission != null,
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

                sale.AgentCommissionAmount = saleCommissionTotal;
                sale.ProviderCommissionEarned = saleCommissionTotal;
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
                    ContactPersonPhone = business.ContactPersonPhone
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
                    OrderNo = BuildOrderNo(sale),
                    ProviderId = sale.ProviderId,
                    ProviderName = provider?.ProviderName ?? "SuperCRM",
                    ProviderEmail = provider?.ContactEmail,
                    OrderDate = sale.OrderDate,
                    OrderStatus = sale.OrderStatus ?? string.Empty,
                    OrderTotal = saleLines.Sum(x => x.LineTotalAmount),
                    AgentCommissionAmount = sale.AgentCommissionAmount,
                    HasResidentialLines = hasResidentialLines,
                    HasBusinessLines = hasBusinessLines,
                    Lines = saleLines.Select(x => new SalesOrderLineSummaryDto
                    {
                        SaleLineId = x.SaleLineId,
                        ProductName = x.ProductName ?? string.Empty,
                        VariantName = x.VariantName,
                        Quantity = x.Quantity,
                        SalesUnitCode = x.SalesUnitCode,
                        UnitPrice = x.SalePrice,
                        LineTotalAmount = x.LineTotalAmount,
                        IsInstallment = x.NoOfInstallment.HasValue && x.NoOfInstallment.Value > 0,
                        MonthlyInstallmentAmount = x.MonthlyInstallmentAmount,
                        NoOfInstallment = x.NoOfInstallment,
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
                            PaymentStatus = i.PaymentStatus
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

        public static string BuildOrderNo(Sale sale)
        {
            return $"SO-{sale.OrderDate:yyyyMMdd}-{sale.SaleId.ToString("N")[..8].ToUpper()}";
        }

        private static CreateSalesOrderResultDto Fail(Guid draftId, string message)
        {
            return new CreateSalesOrderResultDto
            {
                Success = false,
                Message = message,
                SalesOrderDraftId = draftId
            };
        }
    }
}
