using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperCRM.Application.DTOs.PromotionSetup;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Shared;
using SuperCRM.Web.ViewModels.PromotionSetup;
using System.Security.Claims;

namespace SuperCRM.Web.Controllers
{
    [Authorize(
        Roles = AppRoles.SuperAdmin
                + ","
                + AppRoles.SuperCRMAdmin)]
    public class PromotionSetupController : Controller
    {
        private readonly IPromotionSetupService _service;

        public PromotionSetupController(
            IPromotionSetupService service)
        {
            _service = service;
        }

        // -----------------------------------------------------
        // LIST / PROMOTION HISTORY - INITIAL LOAD FROM MENU
        // -----------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Index(
            CancellationToken ct = default)
        {
            var filter =
                new PromotionListFilterDto
                {
                    Page = 1,
                    PageSize = 20
                };

            var result =
                await _service.SearchAsync(
                    filter,
                    ct);

            return View(
                ToListViewModel(
                    result,
                    filter));
        }

        // -----------------------------------------------------
        // LIST / PROMOTION HISTORY - SEARCH + PAGING
        // -----------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(
            PromotionListViewModel vm,
            CancellationToken ct = default)
        {
            var filter =
                new PromotionListFilterDto
                {
                    PromotionName =
                        vm.PromotionName,

                    PromotionStartDate =
                        vm.PromotionStartDate,

                    PromotionEndDate =
                        vm.PromotionEndDate,

                    Published =
                        vm.Published,

                    Page =
                        vm.Page <= 0
                            ? 1
                            : vm.Page,

                    PageSize =
                        vm.PageSize <= 0
                            ? 20
                            : vm.PageSize
                };

            var result =
                await _service.SearchAsync(
                    filter,
                    ct);

            return View(
                ToListViewModel(
                    result,
                    filter));
        }

        private static PromotionListViewModel ToListViewModel(
            PromotionListResultDto result,
            PromotionListFilterDto filter)
        {
            return new PromotionListViewModel
            {
                PromotionName =
                    filter.PromotionName,

                PromotionStartDate =
                    filter.PromotionStartDate,

                PromotionEndDate =
                    filter.PromotionEndDate,

                Published =
                    filter.Published,

                Page =
                    result.Page,

                PageSize =
                    result.PageSize,

                TotalRecords =
                    result.TotalRecords,

                TotalPages =
                    result.TotalPages,

                Promotions =
                    result.Items
                        .Select(x =>
                            new PromotionListRowViewModel
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
                                    x.TotalProducts,

                                ActiveProducts =
                                    x.ActiveProducts,

                                SubmittedAt =
                                    x.SubmittedAt,

                                UpdatedAt =
                                    x.UpdatedAt
                            })
                        .ToList()
            };
        }

        // -----------------------------------------------------
        // CREATE
        // -----------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Create(
            CancellationToken ct = default)
        {
            var vm =
                new PromotionCreateEditViewModel
                {
                    PromotionStartDate =
                        DateTime.Today,

                    PromotionEndDate =
                        DateTime.Today.AddDays(7),

                    IsActive = true
                };

            await BindProductOptionsAsync(
                vm,
                ct);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            PromotionCreateEditViewModel vm,
            CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
            {
                await BindProductOptionsAsync(
                    vm,
                    ct);

                return View(vm);
            }

            var userId = GetCurrentUserId();

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var result =
                await _service.SubmitAsync(
                    ToSubmitDto(
                        vm,
                        userId),
                    ct);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                await BindProductOptionsAsync(
                    vm,
                    ct);

                return View(vm);
            }

            TempData["SuccessMessage"] =
                result.Message;

            return RedirectToAction(
                nameof(Edit),
                new
                {
                    id = result.PromotionId
                });
        }

        // -----------------------------------------------------
        // EDIT
        // -----------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Edit(
            Guid id,
            CancellationToken ct = default)
        {
            var promotion =
                await _service.GetByIdAsync(
                    id,
                    ct);

            if (promotion == null)
            {
                return NotFound();
            }

            var vm =
                ToViewModel(promotion);

            await BindProductOptionsAsync(
                vm,
                ct);

            return View(vm);
        }

        // -----------------------------------------------------
        // UPDATE
        // -----------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(
            PromotionCreateEditViewModel vm,
            CancellationToken ct = default)
        {
            if (!vm.PromotionId.HasValue)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await BindProductOptionsAsync(
                    vm,
                    ct);

                return View(
                    "Edit",
                    vm);
            }

            var userId =
                GetCurrentUserId();

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var request =
                new UpdatePromotionDto
                {
                    PromotionId =
                        vm.PromotionId.Value,

                    PromotionName =
                        vm.PromotionName,

                    PromotionSummary =
                        vm.PromotionSummary,

                    NotificationMessage =
                        vm.NotificationMessage,

                    PromotionStartDate =
                        vm.PromotionStartDate,

                    PromotionEndDate =
                        vm.PromotionEndDate,

                    Remarks =
                        vm.Remarks,

                    IsActive =
                        vm.IsActive,

                    UserId =
                        userId,

                    Items =
                        ToItemDtos(vm)
                };

            var result =
                await _service.UpdateAsync(
                    request,
                    ct);

            if (!result.Success)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.Message);

                await BindProductOptionsAsync(
                    vm,
                    ct);

                return View(
                    "Edit",
                    vm);
            }

            TempData["SuccessMessage"] =
                result.Message;

            return RedirectToAction(
                nameof(Edit),
                new
                {
                    id = vm.PromotionId.Value
                });
        }

        // -----------------------------------------------------
        // PUBLISH
        // -----------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Publish(
            Guid id,
            CancellationToken ct = default)
        {
            return ExecuteOperationAsync(
                id,
                (promotionId, userId, token) =>
                    _service.PublishAsync(
                        promotionId,
                        userId,
                        token),
                ct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> PublishWithEmail(
            Guid id,
            CancellationToken ct = default)
        {
            return ExecuteOperationAsync(
                id,
                (promotionId, userId, token) =>
                    _service.PublishWithEmailAsync(
                        promotionId,
                        userId,
                        token),
                ct);
        }

        // -----------------------------------------------------
        // SEND / RESEND EMAIL
        // -----------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> ResendEmailNotification(
            Guid id,
            CancellationToken ct = default)
        {
            return ExecuteOperationAsync(
                id,
                (promotionId, userId, token) =>
                    _service.ResendEmailAsync(
                        promotionId,
                        userId,
                        token),
                ct);
        }

        // -----------------------------------------------------
        // CANCEL
        // -----------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Cancel(
            Guid id,
            CancellationToken ct = default)
        {
            return ExecuteOperationAsync(
                id,
                (promotionId, userId, token) =>
                    _service.CancelAsync(
                        promotionId,
                        userId,
                        token),
                ct);
        }

        // -----------------------------------------------------
        // COMMON OPERATION HANDLER
        // -----------------------------------------------------
        private async Task<IActionResult> ExecuteOperationAsync(
            Guid id,
            Func<
                Guid,
                Guid,
                CancellationToken,
                Task<PromotionOperationResultDto>> operation,
            CancellationToken ct)
        {
            var userId =
                GetCurrentUserId();

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var result =
                await operation(
                    id,
                    userId,
                    ct);

            TempData[
                result.Success
                    ? "SuccessMessage"
                    : "ErrorMessage"
            ] = result.Message;

            return RedirectToAction(
                nameof(Edit),
                new
                {
                    id
                });
        }

        private async Task BindProductOptionsAsync(
            PromotionCreateEditViewModel vm,
            CancellationToken ct)
        {
            var effectiveDate =
                vm.PromotionStartDate == default
                    ? DateTime.Today
                    : vm.PromotionStartDate;

            vm.ProductOptions =
                await _service.GetProductOptionsAsync(
                    effectiveDate,
                    ct);
        }

        private static SubmitPromotionDto ToSubmitDto(
            PromotionCreateEditViewModel vm,
            Guid userId)
        {
            return new SubmitPromotionDto
            {
                PromotionName =
                    vm.PromotionName,

                PromotionSummary =
                    vm.PromotionSummary,

                NotificationMessage =
                    vm.NotificationMessage,

                PromotionStartDate =
                    vm.PromotionStartDate,

                PromotionEndDate =
                    vm.PromotionEndDate,

                Remarks =
                    vm.Remarks,

                UserId =
                    userId,

                Items =
                    ToItemDtos(vm)
            };
        }

        private static List<PromotionItemInputDto>
            ToItemDtos(
                PromotionCreateEditViewModel vm)
        {
            return vm.Items
                .Select(item =>
                    new PromotionItemInputDto
                    {
                        PromotionItemId =
                            item.PromotionItemId,

                        ProductId =
                            item.ProductId,

                        ProductBaseCommissionId =
                            item.ProductBaseCommissionId,

                        PromotionType =
                            item.PromotionType,

                        PromotionPercentage =
                            item.PromotionPercentage,

                        PromotionAmount =
                            item.PromotionAmount,

                        IsActive =
                            item.IsActive
                    })
                .ToList();
        }

        private static PromotionCreateEditViewModel
            ToViewModel(
                PromotionDto promotion)
        {
            return new PromotionCreateEditViewModel
            {
                PromotionId =
                    promotion.PromotionId,

                PromotionCode =
                    promotion.PromotionCode,

                PromotionName =
                    promotion.PromotionName,

                PromotionSummary =
                    promotion.PromotionSummary,

                NotificationMessage =
                    promotion.NotificationMessage,

                PromotionStartDate =
                    promotion.PromotionStartDate,

                PromotionEndDate =
                    promotion.PromotionEndDate,

                Remarks =
                    promotion.Remarks,

                Submitted =
                    promotion.Submitted,

                Published =
                    promotion.Published,

                Cancelled =
                    promotion.Cancelled,

                CancelledAt =
                    promotion.CancelledAt,

                CancelledByUserId =
                    promotion.CancelledByUserId,

                IsActive =
                    promotion.IsActive,

                EmailNotificationToAgents =
                    promotion.EmailNotificationToAgents,

                NoOfEmailNotificationSend =
                    promotion.NoOfEmailNotificationSend,

                Items =
                    promotion.Items
                        .Select(item =>
                            new PromotionItemViewModel
                            {
                                PromotionItemId =
                                    item.PromotionItemId,

                                ProductId =
                                    item.ProductId,

                                ProductBaseCommissionId =
                                    item.ProductBaseCommissionId,

                                ProductCode =
                                    item.ProductCode,

                                ProductName =
                                    item.ProductName,

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

        private Guid GetCurrentUserId()
        {
            var userIdText =
                User.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            return Guid.TryParse(
                userIdText,
                out var userId)
                    ? userId
                    : Guid.Empty;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendTestEmail(
    Guid id,
    string testEmail,
    CancellationToken ct = default)
        {
            var userId =
                GetCurrentUserId();

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var result =
                await _service.SendTestEmailAsync(
                    new SendPromotionTestEmailDto
                    {
                        PromotionId = id,
                        TestEmail = testEmail,
                        UserId = userId
                    },
                    ct);

            TempData[
                result.Success
                    ? "SuccessMessage"
                    : "ErrorMessage"
            ] = result.Message;

            return RedirectToAction(
                nameof(Edit),
                new
                {
                    id
                });
        }


        // END
    }
}
