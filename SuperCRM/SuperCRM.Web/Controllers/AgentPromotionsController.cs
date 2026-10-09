using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Shared;
using System.Security.Claims;

namespace SuperCRM.Web.Controllers
{
    /// <summary>
    /// Agent-facing active promotion list and AJAX promotion-item endpoint.
    /// </summary>
    [Authorize(Roles = AppRoles.Agent)]
    public class AgentPromotionsController : Controller
    {
        private readonly IPromotionSetupService _promotionSetupService;

        public AgentPromotionsController(
            IPromotionSetupService promotionSetupService)
        {
            _promotionSetupService = promotionSetupService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            CancellationToken cancellationToken,
            bool modal = false)
        {
            var userId = GetCurrentUserId();

            if (userId == Guid.Empty)
            {
                return Challenge();
            }

            var model = await _promotionSetupService
                .GetAgentActivePromotionsAsync(
                    userId,
                    DateTime.Today,
                    cancellationToken);

            return modal
                ? PartialView("_ActivePromotions", model)
                : View(model);
        }

        /// <summary>
        /// AJAX endpoint. Returns the active Promotion Items for one active promotion.
        /// Opening the items marks that promotion as viewed for this Agent.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromotionItems(
            Guid promotionId,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }

            var details = await _promotionSetupService
                .GetAgentPromotionItemsAsync(
                    promotionId,
                    DateTime.Today,
                    cancellationToken);

            if (details == null || details.Items.Count == 0)
            {
                return NotFound(
                    "The promotion is no longer active or has no active promotion items.");
            }

            await _promotionSetupService.MarkAgentPromotionViewedAsync(
                promotionId,
                userId,
                DateTime.UtcNow,
                cancellationToken);

            var notification = await _promotionSetupService
                .GetAgentPromotionNotificationAsync(
                    userId,
                    DateTime.Today,
                    cancellationToken);

            // Allows JavaScript to refresh the header notification without reloading the page.
            Response.Headers["X-Active-Promotion-Count"] =
                notification.ActivePromotionCount.ToString();

            Response.Headers["X-New-Promotion-Count"] =
                notification.NewPromotionCount.ToString();

            return PartialView("_PromotionItems", details);
        }

        private Guid GetCurrentUserId()
        {
            var raw = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return Guid.TryParse(raw, out var id)
                ? id
                : Guid.Empty;
        }
    }
}
