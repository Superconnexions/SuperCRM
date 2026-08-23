using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Shared;

namespace SuperCRM.Web.Controllers
{

    [Authorize(Roles = AppRoles.SuperAdmin + "," + AppRoles.SuperCRMAdmin)]
    public class AdminController : Controller
    {

        private readonly ISalesOrderCreationService _salesOrderCreationService;

        public AdminController(
            ISalesOrderCreationService salesOrderCreationService)
        {
            _salesOrderCreationService = salesOrderCreationService;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string searchMode, Guid? agentUserId, Guid? adminUserId, CancellationToken cancellationToken)
        {
            var model =
                await _salesOrderCreationService.GetAdminDashboardAsync(
                    searchMode,
                    agentUserId,
                    adminUserId,
                    cancellationToken);

            return View(model);
        }
    }
}
