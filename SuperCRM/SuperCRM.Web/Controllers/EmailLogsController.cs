using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Web.ViewModels.EmailSettings;

namespace SuperCRM.Web.Controllers
{
    [Authorize(Roles = "SuperAdmin,SuperCRMAdmin")]
    public class EmailLogsController : Controller
    {
        private readonly IEmailLogService _emailLogService;

        public EmailLogsController(IEmailLogService emailLogService)
        {
            _emailLogService = emailLogService;
        }

        public async Task<IActionResult> Index(
            int pageNumber = 1,
            int pageSize = 20,
            bool? isSent = null,
            string? searchText = null,
            CancellationToken cancellationToken = default)
        {
            var logs = await _emailLogService.GetPagedAsync(pageNumber, pageSize, isSent, searchText, cancellationToken);

            return View(new EmailLogIndexViewModel
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                IsSent = isSent,
                SearchText = searchText,
                Logs = logs
            });
        }

        public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken = default)
        {
            var log = await _emailLogService.GetDetailsAsync(id, cancellationToken);
            if (log == null) return NotFound();
            return View(log);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resend(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await _emailLogService.ResendFailedAsync(id, GetUserId(), cancellationToken);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;
            return RedirectToAction(nameof(Index), new { isSent = false });
        }

        private Guid? GetUserId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }
}
