using SuperCRM.Application.DTOs.EmailSettings;
using SuperCRM.Application.DTOs.PromotionSetup;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Entities;
using SuperCRM.Domain.Enums;
using System.Net;
using System.Text;

namespace SuperCRM.Application.Services
{
    public class PromotionSetupService : IPromotionSetupService
    {
        private readonly IPromotionSetupRepository _repo;
        private readonly IEmailSenderService _email;

        public PromotionSetupService(
            IPromotionSetupRepository repo,
            IEmailSenderService email)
        {
            _repo = repo;
            _email = email;
        }

        // -----------------------------------------------------
        // PROMOTION LIST / HISTORY
        // -----------------------------------------------------

        public Task<PromotionListResultDto> SearchAsync(
            PromotionListFilterDto filter,
            CancellationToken ct = default)
        {
            filter.PromotionName =
                string.IsNullOrWhiteSpace(filter.PromotionName)
                    ? null
                    : filter.PromotionName.Trim();

            filter.Page =
                filter.Page < 1
                    ? 1
                    : filter.Page;

            var allowedPageSizes =
                new[] { 10, 20, 50, 100 };

            if (!allowedPageSizes.Contains(
                filter.PageSize))
            {
                filter.PageSize = 20;
            }

            return _repo.SearchAsync(
                filter,
                ct);
        }

        public Task<List<PromotionProductLookupDto>> GetProductOptionsAsync(
            DateTime asOfDate,
            CancellationToken ct = default)
        {
            return _repo.GetProductCommissionOptionsAsync(
                asOfDate,
                ct);
        }

        public async Task<PromotionDto?> GetByIdAsync(
            Guid id,
            CancellationToken ct = default)
        {
            var entity = await _repo.GetByIdAsync(id, ct);

            if (entity == null)
            {
                return null;
            }

            return new PromotionDto
            {
                PromotionId = entity.PromotionId,
                PromotionCode = entity.PromotionCode,
                PromotionName = entity.PromotionName,
                PromotionSummary = entity.PromotionSummary,
                NotificationMessage = entity.NotificationMessage,
                PromotionStartDate = entity.PromotionStartDate,
                PromotionEndDate = entity.PromotionEndDate,

                Published = entity.Published,
                Cancelled = entity.Cancelled,
                CancelledAt = entity.CancelledAt,
                CancelledByUserId = entity.CancelledByUserId,
                IsActive = entity.IsActive,
                Submitted = entity.Submitted,

                EmailNotificationToAgents =
                    entity.EmailNotificationToAgents,

                NoOfEmailNotificationSend =
                    entity.NoOfEmailNotificationSend,

                Remarks = entity.Remarks,

                Items = entity.Items
                    .Select(item => new PromotionItemDto
                    {
                        PromotionItemId =
                            item.PromotionItemId,

                        ProductId =
                            item.ProductId,

                        ProductBaseCommissionId =
                            item.ProductBaseCommissionId,

                        ProductCode =
                            item.Product?.ProductCode
                            ?? string.Empty,

                        ProductName =
                            item.Product?.ProductName
                            ?? string.Empty,

                        StandardCommission =
                            item.StandardCommission,

                        PromotionType =
                            item.PromotionType,

                        PromotionPercentage =
                            item.PromotionPercentage,

                        PromotionAmount =
                            item.PromotionAmount,

                        FinalCommissionAmount =
                            item.FinalCommissionAmount,

                        IsActive =
                            item.IsActive
                    })
                    .ToList()
            };
        }

        // -----------------------------------------------------
        // SUBMIT / CREATE
        // -----------------------------------------------------
        public async Task<PromotionOperationResultDto> SubmitAsync(
            SubmitPromotionDto request,
            CancellationToken ct = default)
        {
            var validationError =
                await ValidateAsync(request, ct);

            if (validationError != null)
            {
                return Fail(validationError);
            }

            var now = DateTime.UtcNow;

            var promotionCode =
                await GeneratePromotionCodeAsync(
                    now,
                    ct);

            if (string.IsNullOrWhiteSpace(promotionCode))
            {
                return Fail(
                    "Unable to generate promotion code.");
            }

            var entity = new PromotionSetup
            {
                PromotionId = Guid.NewGuid(),

                PromotionCode = promotionCode,

                PromotionName =
                    request.PromotionName.Trim(),

                PromotionSummary =
                    Clean(request.PromotionSummary),

                NotificationMessage =
                    Clean(request.NotificationMessage),

                PromotionStartDate =
                    request.PromotionStartDate.Date,

                PromotionEndDate =
                    request.PromotionEndDate.Date,

                Remarks =
                    Clean(request.Remarks),

                // Required initial status after Submit.
                IsActive = true,
                Cancelled = false,
                Submitted = true,
                Published = false,

                EmailNotificationToAgents = false,
                NoOfEmailNotificationSend = 0,

                SubmittedAt = now,
                SubmittedByUserId = request.UserId
            };

            foreach (var input in request.Items)
            {
                // New PromotionItem is ALWAYS active.
                entity.Items.Add(
                    await BuildNewItemAsync(
                        entity,
                        input,
                        request.UserId,
                        ct));
            }

            await _repo.AddAsync(entity, ct);

            try
            {
                await _repo.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                return Fail(
                    "Promotion could not be saved. "
                    + ex.Message);
            }

            return Ok(
                entity.PromotionId,
                $"Promotion {promotionCode} submitted successfully.");
        }

        // -----------------------------------------------------
        // UPDATE
        // -----------------------------------------------------
        public async Task<PromotionOperationResultDto> UpdateAsync(
            UpdatePromotionDto request,
            CancellationToken ct = default)
        {
            var entity =
                await _repo.GetByIdAsync(
                    request.PromotionId,
                    ct);

            if (entity == null)
            {
                return Fail("Promotion not found.");
            }

            if (entity.Cancelled)
            {
                return Fail(
                    "Cancelled promotion cannot be updated.");
            }

            var validationError =
                await ValidateAsync(request, ct);

            if (validationError != null)
            {
                return Fail(validationError);
            }

            entity.PromotionName =
                request.PromotionName.Trim();

            entity.PromotionSummary =
                Clean(request.PromotionSummary);

            entity.NotificationMessage =
                Clean(request.NotificationMessage);

            entity.PromotionStartDate =
                request.PromotionStartDate.Date;

            entity.PromotionEndDate =
                request.PromotionEndDate.Date;

            entity.Remarks =
                Clean(request.Remarks);

            // Promotion-level Active is maintained from Edit UI.
            entity.IsActive =
                request.IsActive;

            entity.UpdatedAt =
                DateTime.UtcNow;

            entity.UpdatedByUserId =
                request.UserId;

            var incomingExistingItemIds =
                request.Items
                    .Where(x => x.PromotionItemId.HasValue)
                    .Select(x => x.PromotionItemId!.Value)
                    .ToHashSet();

            // Before publish: removing a row physically removes it.
            // After publish: rows are retained for history and made inactive.
            foreach (var existingItem in entity.Items
                .Where(x =>
                    !incomingExistingItemIds.Contains(
                        x.PromotionItemId))
                .ToList())
            {
                if (entity.Published)
                {
                    existingItem.IsActive = false;
                    existingItem.UpdatedAt =
                        DateTime.UtcNow;
                    existingItem.UpdatedByUserId =
                        request.UserId;
                }
                else
                {
                    _repo.RemoveItem(existingItem);
                }
            }

            foreach (var input in request.Items)
            {
                var existingItem =
                    input.PromotionItemId.HasValue
                        ? entity.Items.FirstOrDefault(
                            x =>
                                x.PromotionItemId
                                == input.PromotionItemId.Value)
                        : null;

                if (existingItem == null)
                {
                    if (entity.Published)
                    {
                        return Fail(
                            "New products cannot be added after publication.");
                    }

                    // Any newly-added product is Active by default.
                    entity.Items.Add(
                        await BuildNewItemAsync(
                            entity,
                            input,
                            request.UserId,
                            ct));
                }
                else
                {
                    // Existing item can be changed Active -> Inactive
                    // or Inactive -> Active from Edit UI.
                    await ApplyExistingItemAsync(
                        existingItem,
                        input,
                        request.UserId,
                        ct);
                }
            }

            await _repo.SaveChangesAsync(ct);

            return Ok(
                entity.PromotionId,
                "Promotion updated successfully.");
        }

        // -----------------------------------------------------
        // PUBLISH
        // -----------------------------------------------------
        public async Task<PromotionOperationResultDto> PublishAsync(
            Guid id,
            Guid userId,
            CancellationToken ct = default)
        {
            var entity =
                await _repo.GetByIdAsync(id, ct);

            if (entity == null)
            {
                return Fail("Promotion not found.");
            }

            var validationError =
                CanPublish(entity);

            if (validationError != null)
            {
                return Fail(validationError);
            }


            // Check Duplicate Promotion for Same Product

            // -----------------------------------------------------
            // CHECK PRODUCT ALREADY UNDER ACTIVE PUBLISHED PROMOTION
            // -----------------------------------------------------

            var activeProductIds =
                entity.Items
                    .Where(x => x.IsActive)
                    .Select(x => x.ProductId)
                    .Distinct()
                    .ToList();

            var conflicts =
                await _repo
                    .GetActivePromotionProductConflictsAsync(
                        entity.PromotionId,
                        activeProductIds,
                        DateTime.Today,
                        ct);

            if (conflicts.Count > 0)
            {
                var conflictProducts =
                    conflicts
                        .GroupBy(x => x.ProductId)
                        .Select(group =>
                        {
                            var product =
                                group.First();

                            var productDisplay =
                                !string.IsNullOrWhiteSpace(
                                    product.ProductCode)
                                && !string.IsNullOrWhiteSpace(
                                    product.ProductName)
                                    ? $"{product.ProductCode} - {product.ProductName}"
                                    : !string.IsNullOrWhiteSpace(
                                        product.ProductName)
                                        ? product.ProductName
                                        : product.ProductCode;

                            var promotions =
                                string.Join(
                                    ", ",
                                    group
                                        .Select(x =>
                                            $"{x.PromotionCode} "
                                            + $"(End: {x.PromotionEndDate:dd-MMM-yyyy})")
                                        .Distinct());

                            return
                                $"{productDisplay} [{promotions}]";
                        });

                return Fail(
                    "Cannot publish this promotion. "
                    + "The following product(s) already have "
                    + "an active published promotion: "
                    + string.Join("; ", conflictProducts)
                    + ".");
            }

            // END Duplicacte Check

            entity.Published = true;

            entity.UpdatedAt =
                DateTime.UtcNow;

            entity.UpdatedByUserId =
                userId;

            await _repo.SaveChangesAsync(ct);

            return Ok(
                id,
                "Promotion published successfully.");
        }

        public async Task<PromotionOperationResultDto>
            PublishWithEmailAsync(
                Guid id,
                Guid userId,
                CancellationToken ct = default)
        {
            var publishResult =
                await PublishAsync(
                    id,
                    userId,
                    ct);

            if (!publishResult.Success)
            {
                return publishResult;
            }

            return await SendNotificationAsync(
                id,
                userId,
                ct);
        }

        public Task<PromotionOperationResultDto>
            ResendEmailAsync(
                Guid id,
                Guid userId,
                CancellationToken ct = default)
        {
            return SendNotificationAsync(
                id,
                userId,
                ct);
        }

        // -----------------------------------------------------
        // CANCEL
        // -----------------------------------------------------
        public async Task<PromotionOperationResultDto> CancelAsync(
            Guid id,
            Guid userId,
            CancellationToken ct = default)
        {
            var entity =
                await _repo.GetByIdAsync(id, ct);

            if (entity == null)
            {
                return Fail("Promotion not found.");
            }

            if (entity.Cancelled)
            {
                return Fail(
                    "Promotion is already cancelled.");
            }

            var now = DateTime.UtcNow;

            entity.Cancelled = true;

            // Cancelled promotion must no longer participate
            // in future commission calculation.
            entity.IsActive = false;

            entity.CancelledAt = now;
            entity.CancelledByUserId = userId;

            entity.UpdatedAt = now;
            entity.UpdatedByUserId = userId;

            // Keep Published unchanged for historical/audit purpose.
            await _repo.SaveChangesAsync(ct);

            return Ok(
                id,
                $"Promotion {entity.PromotionCode} cancelled successfully.");
        }

        // -----------------------------------------------------
        // EMAIL
        // -----------------------------------------------------


        private async Task<PromotionOperationResultDto>
            SendNotificationAsync(
                Guid id,
                Guid userId,
                CancellationToken ct)
        {
            var entity =
                await _repo.GetByIdAsync(
                    id,
                    ct);

            if (entity == null)
            {
                return Fail("Promotion not found.");
            }

            if (!entity.Published)
            {
                return Fail(
                    "Promotion must be published before email notification can be sent.");
            }

            if (entity.Cancelled)
            {
                return Fail(
                    "Email notification cannot be sent for a cancelled promotion.");
            }

            if (!entity.IsActive)
            {
                return Fail(
                    "Email notification cannot be sent for an inactive promotion.");
            }

            // ---------------------------------------------------------
            // Only ACTIVE Promotion Items will be included in the email
            // ---------------------------------------------------------

            var activeItems = entity.Items
                            .Where(x => x.IsActive)
                            .ToList();

            if (activeItems.Count == 0)
            {
                return Fail(
                    "At least one active Promotion Product is required before sending email notification.");
            }

            var emails =
                await _repo.GetActiveAgentEmailsAsync(ct);

            

            if (emails.Count == 0)
            {
                return Fail(
                    "No active agent email addresses were found.");
            }

            var emailContent =
                BuildPromotionEmail(entity);

            var subject =
                emailContent.Subject;

            var body =
                emailContent.Body;

            var failedEmails =
                new List<string>();

            foreach (var emailAddress in emails)
            {
                var result =
                    await _email.SendAsync(
                        new SendEmailRequestDto
                        {
                            ToEmail = emailAddress,
                            Subject = subject,
                            Body = body,
                            IsHtml = true,
                            SourceModule =
                                "PromotionSetup",
                            CreatedByUserId =
                                userId
                        },
                        ct);

                if (!result.Success)
                {
                    failedEmails.Add(
                        emailAddress);
                }
            }

            if (failedEmails.Count > 0)
            {
                return Fail(
                    $"Promotion is published, but email failed for {failedEmails.Count} of {emails.Count} agent(s). Notification counter was not increased.");
            }

            entity.EmailNotificationToAgents =
                true;

            if (entity.NoOfEmailNotificationSend
                < byte.MaxValue)
            {
                entity.NoOfEmailNotificationSend++;
            }

            entity.LastEmailNotificationAt =
                DateTime.UtcNow;

            entity.LastEmailNotificationByUserId =
                userId;

            entity.UpdatedAt =
                DateTime.UtcNow;

            entity.UpdatedByUserId =
                userId;

            await _repo.SaveChangesAsync(ct);

            return Ok(
                id,
                $"Email notification sent successfully to {emails.Count} active agent(s).");
        }







        // -----------------------------------------------------
        // AGENT ACTIVE PROMOTIONS / NOTIFICATION
        // -----------------------------------------------------

        public Task<List<AgentActivePromotionDto>> GetAgentActivePromotionsAsync(
            Guid agentUserId,
            DateTime currentDate,
            CancellationToken ct = default)
        {
            return _repo.GetAgentActivePromotionsAsync(
                agentUserId,
                currentDate.Date,
                ct);
        }

        public Task<AgentPromotionNotificationDto> GetAgentPromotionNotificationAsync(
            Guid agentUserId,
            DateTime currentDate,
            CancellationToken ct = default)
        {
            return _repo.GetAgentPromotionNotificationAsync(
                agentUserId,
                currentDate.Date,
                ct);
        }

        public Task<AgentPromotionDetailsDto?> GetAgentPromotionItemsAsync(
            Guid promotionId,
            DateTime currentDate,
            CancellationToken ct = default)
        {
            return _repo.GetAgentPromotionItemsAsync(
                promotionId,
                currentDate.Date,
                ct);
        }

        public Task MarkAgentPromotionViewedAsync(
            Guid promotionId,
            Guid agentUserId,
            DateTime viewedAt,
            CancellationToken ct = default)
        {
            return _repo.MarkAgentPromotionViewedAsync(
                promotionId,
                agentUserId,
                viewedAt,
                ct);
        }

        // -----------------------------------------------------
        // VALIDATION
        // -----------------------------------------------------
        private async Task<string?> ValidateAsync(
            SubmitPromotionDto request,
            CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(
                request.PromotionName))
            {
                return "Promotion name is required.";
            }

            if (request.PromotionName.Trim().Length
                > 200)
            {
                return "Promotion name cannot exceed 200 characters.";
            }

            if (request.PromotionSummary?.Length
                > 500)
            {
                return "Promotion summary cannot exceed 500 characters.";
            }

            if (request.NotificationMessage?.Length
                > 2000)
            {
                return "Notification message cannot exceed 2000 characters.";
            }

            if (request.Remarks?.Length > 500)
            {
                return "Remarks cannot exceed 500 characters.";
            }

            if (request.PromotionEndDate.Date
                < request.PromotionStartDate.Date)
            {
                return "Promotion End Date must be greater than or equal to Start Date.";
            }

            if (request.Items == null
                || request.Items.Count == 0)
            {
                return "At least one promotion product is required.";
            }

            if (request.Items
                .GroupBy(x => x.ProductId)
                .Any(g => g.Count() > 1))
            {
                return "The same product cannot be added more than once.";
            }

            foreach (var item in request.Items)
            {
                if (item.ProductId == Guid.Empty
                    || item.ProductBaseCommissionId
                        == Guid.Empty)
                {
                    return "A valid product and base commission are required.";
                }

                var baseCommission =
                    await _repo.GetProductBaseCommissionAsync(
                        item.ProductBaseCommissionId,
                        ct);

                if (baseCommission == null
                    || baseCommission.ProductId
                        != item.ProductId
                    || baseCommission.FixedAmount
                        == null)
                {
                    return "Selected product base commission is invalid or does not contain a FixedAmount.";
                }

                if (item.PromotionType
                        == PromotionType.Percentage
                    && (!item.PromotionPercentage.HasValue
                        || item.PromotionPercentage < 0))
                {
                    return "Promotion percentage is required and cannot be negative.";
                }

                if (item.PromotionType
                        == PromotionType.FixedAmount
                    && (!item.PromotionAmount.HasValue
                        || item.PromotionAmount < 0))
                {
                    return "Promotion amount is required and cannot be negative.";
                }
            }

            return null;
        }

        // -----------------------------------------------------
        // NEW ITEM
        // -----------------------------------------------------
        private async Task<PromotionItem> BuildNewItemAsync(
            PromotionSetup promotion,
            PromotionItemInputDto input,
            Guid userId,
            CancellationToken ct)
        {
            var item = new PromotionItem
            {
                PromotionItemId = Guid.NewGuid(),
                PromotionId = promotion.PromotionId,
                PromotionCode = promotion.PromotionCode,

                // Business rule:
                // every newly-created PromotionItem is Active.
                IsActive = true
            };

            await ApplyCommissionValuesAsync(
                item,
                input,
                userId,
                ct);

            // ApplyCommissionValuesAsync does not override
            // IsActive for a new item.
            return item;
        }

        // -----------------------------------------------------
        // EXISTING ITEM
        // -----------------------------------------------------
        private async Task ApplyExistingItemAsync(
            PromotionItem item,
            PromotionItemInputDto input,
            Guid userId,
            CancellationToken ct)
        {
            await ApplyCommissionValuesAsync(
                item,
                input,
                userId,
                ct);

            // Existing item Active status comes from Edit UI.
            item.IsActive = input.IsActive;
        }

        private async Task ApplyCommissionValuesAsync(
            PromotionItem item,
            PromotionItemInputDto input,
            Guid userId,
            CancellationToken ct)
        {
            var baseCommission =
                await _repo.GetProductBaseCommissionAsync(
                    input.ProductBaseCommissionId,
                    ct)
                ?? throw new InvalidOperationException(
                    "Product base commission not found.");

            var standardCommission =
                baseCommission.FixedAmount
                ?? 0m;

            var promotionAmount =
                input.PromotionType
                    == PromotionType.Percentage
                    ? decimal.Round(
                        standardCommission
                        * (input.PromotionPercentage ?? 0m)
                        / 100m,
                        2,
                        MidpointRounding.AwayFromZero)
                    : input.PromotionAmount
                        ?? 0m;

            item.ProductBaseCommissionId =
                baseCommission.ProductBaseCommissionId;

            item.ProductId =
                baseCommission.ProductId;

            item.StandardCommission =
                standardCommission;

            item.PromotionType =
                input.PromotionType;

            item.PromotionPercentage =
                input.PromotionType
                    == PromotionType.Percentage
                    ? input.PromotionPercentage
                    : null;

            item.PromotionAmount =
                promotionAmount;

            item.FinalCommissionAmount =
                standardCommission
                + promotionAmount;

            item.UpdatedAt =
                DateTime.UtcNow;

            item.UpdatedByUserId =
                userId;
        }

        private async Task<string> GeneratePromotionCodeAsync(
            DateTime now,
            CancellationToken ct)
        {
            for (var attempt = 0;
                 attempt < 5;
                 attempt++)
            {
                var sequence =
                    await _repo.GetNextMonthlySequenceAsync(
                        now.Year,
                        now.Month,
                        ct);

                var code =
                    $"PRO{now:yyMM}{sequence:000}";

                if (!await _repo.PromotionCodeExistsAsync(
                    code,
                    ct))
                {
                    return code;
                }
            }

            return string.Empty;
        }

        private static string? CanPublish(
            PromotionSetup entity)
        {
            if (!entity.Submitted)
            {
                return "Promotion must be submitted before publication.";
            }

            if (entity.Cancelled)
            {
                return "Cancelled promotion cannot be published.";
            }

            if (entity.Published)
            {
                return "Promotion is already published.";
            }

            if (!entity.IsActive)
            {
                return "Inactive promotion cannot be published.";
            }

            if (!entity.Items.Any(x => x.IsActive))
            {
                return "At least one active promotion item is required.";
            }

            return null;
        }

        private static string? Clean(
            string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }

        private static PromotionOperationResultDto Fail(
            string message)
        {
            return new PromotionOperationResultDto
            {
                Success = false,
                Message = message
            };
        }

        private static PromotionOperationResultDto Ok(
            Guid promotionId,
            string message)
        {
            return new PromotionOperationResultDto
            {
                Success = true,
                Message = message,
                PromotionId = promotionId
            };
        }


        private PromotionEmailContentDto BuildPromotionEmail(
        PromotionSetup entity)
            {
                var activeItems = entity.Items
                    .Where(x => x.IsActive)
                    .ToList();

                var productRows =
                    new StringBuilder();

                foreach (var item in activeItems)
                {
                    var productCode =
                        item.Product?.ProductCode
                        ?? string.Empty;

                    var productName =
                        item.Product?.ProductName
                        ?? string.Empty;

                    var productDisplay =
                        string.IsNullOrWhiteSpace(productCode)
                            ? productName
                            : string.IsNullOrWhiteSpace(productName)
                                ? productCode
                                : $"{productCode} - {productName}";

                    productRows.Append($@"
                <tr>
                    <td style=""padding:10px;
                               border:1px solid #d9d9d9;"">
                        {WebUtility.HtmlEncode(productDisplay)}
                    </td>

                    <td style=""padding:10px;
                               border:1px solid #d9d9d9;
                               text-align:right;
                               white-space:nowrap;"">
                        BDT {item.StandardCommission:N2}
                    </td>

                    <td style=""padding:10px;
                               border:1px solid #d9d9d9;
                               text-align:right;
                               white-space:nowrap;"">
                        BDT {item.PromotionAmount:N2}
                    </td>

                    <td style=""padding:10px;
                               border:1px solid #d9d9d9;
                               text-align:right;
                               white-space:nowrap;"">
                        BDT {item.FinalCommissionAmount:N2}
                    </td>
                </tr>");
                }

                var encodedMessage =
                    WebUtility.HtmlEncode(
                        entity.NotificationMessage
                        ?? string.Empty)
                    .Replace(
                        "\r\n",
                        "<br />")
                    .Replace(
                        "\n",
                        "<br />");

                var notificationMessageHtml =
                    string.IsNullOrWhiteSpace(
                        encodedMessage)
                        ? string.Empty
                        : $"<p>{encodedMessage}</p>";

                var subject =
                    $"New Agent Promotion - {entity.PromotionName}";

                var body = $@"
                <div style=""font-family:Arial,Helvetica,sans-serif;
                            font-size:14px;
                            line-height:1.6;
                            color:#222;"">

                    <p>Dear Agent,</p>

                    <p>
                        We are pleased to inform you that a new sales promotion
                        has been introduced for eligible products.
                    </p>

                    <p>
                        <strong>Promotion:</strong>
                        {WebUtility.HtmlEncode(entity.PromotionName)}
                        <br />

                        <strong>Promotion Code:</strong>
                        {WebUtility.HtmlEncode(entity.PromotionCode)}
                        <br />

                        <strong>Promotion Period:</strong>
                        {entity.PromotionStartDate:dd-MMM-yyyy}
                        to
                        {entity.PromotionEndDate:dd-MMM-yyyy}
                    </p>

                    {notificationMessageHtml}

                    <p>
                        Promotional commission will apply to below eligible products
                        according to the applicable promotion terms and product criteria.
                    </p>

                    <p>
                        <strong>Promotion Products:</strong>
                    </p>

                    <table style=""border-collapse:collapse;
                                  width:100%;
                                  max-width:900px;
                                  margin-bottom:20px;"">

                        <thead>
                            <tr style=""background-color:#f2f2f2;"">

                                <th style=""padding:10px;
                                           border:1px solid #d9d9d9;
                                           text-align:left;"">
                                    Product
                                </th>

                                <th style=""padding:10px;
                                           border:1px solid #d9d9d9;
                                           text-align:right;"">
                                    Standard Commission
                                </th>

                                <th style=""padding:10px;
                                           border:1px solid #d9d9d9;
                                           text-align:right;"">
                                    Promotional Amount
                                </th>

                                <th style=""padding:10px;
                                           border:1px solid #d9d9d9;
                                           text-align:right;"">
                                    Final Commission
                                </th>

                            </tr>
                        </thead>

                        <tbody>
                            {productRows}
                        </tbody>

                    </table>

                    <p>
                        Please contact the SuperCRM support team if you require
                        any clarification.
                    </p>

                    <p>
                        Regards,<br />
                        <strong>SuperCRM Team</strong>
                    </p>

                </div>";

                return new PromotionEmailContentDto
                {
                    Subject = subject,
                    Body = body
                };
            }

        public async Task<PromotionOperationResultDto> SendTestEmailAsync(
    SendPromotionTestEmailDto request,
    CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(
                request.TestEmail))
            {
                return Fail(
                    "Test email address is required.");
            }

            var emailValidator =
                new System.ComponentModel.DataAnnotations
                    .EmailAddressAttribute();

            if (!emailValidator.IsValid(
                request.TestEmail))
            {
                return Fail(
                    "Please enter a valid test email address.");
            }

            var entity =
                await _repo.GetByIdAsync(
                    request.PromotionId,
                    ct);

            if (entity == null)
            {
                return Fail(
                    "Promotion not found.");
            }

            if (entity.Cancelled)
            {
                return Fail(
                    "Test email cannot be sent for a cancelled promotion.");
            }

            if (!entity.IsActive)
            {
                return Fail(
                    "Promotion must be Active before sending a test email.");
            }

            var activeItems =
                entity.Items
                    .Where(x => x.IsActive)
                    .ToList();

            if (activeItems.Count == 0)
            {
                return Fail(
                    "At least one active Promotion Product is required before sending a test email.");
            }

            var emailContent =
                BuildPromotionEmail(entity);

            var result =
                await _email.SendAsync(
                    new SendEmailRequestDto
                    {
                        ToEmail =
                            request.TestEmail.Trim(),

                        Subject =
                            "[TEST] "
                            + emailContent.Subject,

                        Body =
                            emailContent.Body,

                        IsHtml =
                            true,

                        SourceModule =
                            "PromotionSetup-Test",

                        CreatedByUserId =
                            request.UserId
                    },
                    ct);

            if (!result.Success)
            {
                return Fail(
                    "Test email could not be sent.");
            }

            // IMPORTANT:
            // No promotion notification audit fields are changed here.

            return Ok(
                entity.PromotionId,
                $"Test promotion email sent successfully to {request.TestEmail.Trim()}.");
        }

        // END
    }
}
