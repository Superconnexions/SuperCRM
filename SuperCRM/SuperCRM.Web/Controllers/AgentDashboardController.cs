using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Shared;
using SuperCRM.Web.ViewModels.Agents;
using System.Security.Claims;

namespace SuperCRM.Web.Controllers
{
    /// <summary>
    /// Dashboard controller for logged-in Agents.
    /// </summary>
    [Authorize(Roles = AppRoles.Agent)]
    public class AgentDashboardController : Controller
    {
        private readonly IAgentRegistrationService _agentRegistrationService;
        private readonly ISalesOrderCreationService _salesOrderCreationService;

        public AgentDashboardController(IAgentRegistrationService agentRegistrationService, ISalesOrderCreationService salesOrderCreationService)
        {
            _agentRegistrationService = agentRegistrationService;
            _salesOrderCreationService = salesOrderCreationService;
        }

        /// <summary>
        /// Loads the dashboard for the current Agent.
        /// If the agent record is missing, the request is rejected.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var currentUserId = GetCurrentUserId();

            //if (string.IsNullOrWhiteSpace(currentUserId))
            //    return Challenge();

            //var dashboard = await _agentRegistrationService.GetDashboardAsync(Guid.Parse(currentUserId), cancellationToken);

            //if (dashboard == null)
            //    return Forbid();

            //var model = new AgentDashboardViewModel
            //{
            //    AgentCode = dashboard.AgentCode,
            //    IsApproved = dashboard.IsApproved,
            //    IsCommissionEligible = dashboard.IsCommissionEligible,
            //    RegistrationStatusText = dashboard.RegistrationStatusText,
            //    JoinedAt = dashboard.JoinedAt
            //};

            if (currentUserId == Guid.Empty)
            {
                return RedirectToAction("Login", "Account");
            }

            var model = await _salesOrderCreationService.GetAgentDashboardAsync(
                currentUserId,
                cancellationToken);

            
            return View(model);
        }

        private Guid GetCurrentUserId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
        }

        // END
    }
}
