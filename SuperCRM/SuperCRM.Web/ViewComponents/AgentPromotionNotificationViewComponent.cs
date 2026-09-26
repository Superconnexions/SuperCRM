using Microsoft.AspNetCore.Mvc;
using SuperCRM.Application.DTOs.PromotionSetup;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Shared;
using System.Security.Claims;

namespace SuperCRM.Web.ViewComponents
{
    public class AgentPromotionNotificationViewComponent : ViewComponent
    {
        private readonly IPromotionSetupService _promotionSetupService;

        public AgentPromotionNotificationViewComponent(
            IPromotionSetupService promotionSetupService)
        {
            _promotionSetupService = promotionSetupService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!(HttpContext.User.Identity?.IsAuthenticated ?? false)
                || !HttpContext.User.IsInRole(AppRoles.Agent))
            {
                return Content(string.Empty);
            }

            var raw = HttpContext.User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(raw, out var userId))
            {
                return Content(string.Empty);
            }

            AgentPromotionNotificationDto model =
                await _promotionSetupService
                    .GetAgentPromotionNotificationAsync(
                        userId,
                        DateTime.Today,
                        HttpContext.RequestAborted);

            return View(model);
        }
    }
}
