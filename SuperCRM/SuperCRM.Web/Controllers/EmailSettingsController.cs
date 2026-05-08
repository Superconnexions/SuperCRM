using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SuperCRM.Application.DTOs.EmailSettings;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Web.Helpers;
using SuperCRM.Web.ViewModels.EmailSettings;

namespace SuperCRM.Web.Controllers
{
    [Authorize(Roles = "SuperAdmin,SuperCRMAdmin")]
    public class EmailSettingsController : Controller
    {
        private readonly IEmailSettingService _emailSettingService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailHelper _emailHelper;

        public EmailSettingsController(
            IEmailSettingService emailSettingService,
            IEmailLogService emailLogService,
            IEmailHelper emailHelper)
        {
            _emailSettingService = emailSettingService;
            _emailLogService = emailLogService;
            _emailHelper = emailHelper;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            var settings = await _emailSettingService.GetAllAsync(cancellationToken);
            var vm = settings.Select(x => new EmailSettingListItemViewModel
            {
                EmailSettingId = x.EmailSettingId,
                SettingName = x.SettingName,
                SmtpServer = x.SmtpServer,
                Port = x.Port,
                SenderEmail = x.SenderEmail,
                Username = x.Username,
                EnableSsl = x.EnableSsl,
                IsDefault = x.IsDefault,
                IsActive = x.IsActive
            }).ToList();

            return View(vm);
        }

        public IActionResult Create()
        {
            return View(new EmailSettingCreateEditViewModel
            {
                SettingName = "Gmail SMTP",
                SmtpServer = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                IsDefault = true,
                IsActive = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmailSettingCreateEditViewModel vm, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid) return View(vm);

            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var result = await _emailSettingService.CreateAsync(new CreateEmailSettingDto
            {
                SettingName = vm.SettingName,
                SmtpServer = vm.SmtpServer,
                Port = vm.Port,
                SenderName = vm.SenderName,
                SenderEmail = vm.SenderEmail,
                Username = vm.Username,
                Password = vm.Password ?? string.Empty,
                EnableSsl = vm.EnableSsl,
                IsDefault = vm.IsDefault,
                IsActive = vm.IsActive,
                CreatedByUserId = userId
            }, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(vm);
            }

            TempData["SuccessMessage"] = "Email setting created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken = default)
        {
            var dto = await _emailSettingService.GetByIdAsync(id, cancellationToken);
            if (dto == null) return NotFound();

            return View(new EmailSettingCreateEditViewModel
            {
                EmailSettingId = dto.EmailSettingId,
                SettingName = dto.SettingName,
                SmtpServer = dto.SmtpServer,
                Port = dto.Port,
                SenderName = dto.SenderName,
                SenderEmail = dto.SenderEmail,
                Username = dto.Username,
                EnableSsl = dto.EnableSsl,
                IsDefault = dto.IsDefault,
                IsActive = dto.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmailSettingCreateEditViewModel vm, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid || !vm.EmailSettingId.HasValue) return View(vm);

            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var result = await _emailSettingService.UpdateAsync(new UpdateEmailSettingDto
            {
                EmailSettingId = vm.EmailSettingId.Value,
                SettingName = vm.SettingName,
                SmtpServer = vm.SmtpServer,
                Port = vm.Port,
                SenderName = vm.SenderName,
                SenderEmail = vm.SenderEmail,
                Username = vm.Username,
                Password = vm.Password,
                EnableSsl = vm.EnableSsl,
                IsDefault = vm.IsDefault,
                IsActive = vm.IsActive,
                UpdatedByUserId = userId
            }, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(vm);
            }

            TempData["SuccessMessage"] = "Email setting updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Test()
        {
            return View(new TestEmailViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Test(TestEmailViewModel vm, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid) return View(vm);

            var result = await _emailHelper.SendTestEmailAsync(vm.ToEmail, GetUserId(), cancellationToken);
            if (result.Success)
                TempData["SuccessMessage"] = result.Message;
            else
                ModelState.AddModelError(string.Empty, result.Message);

            return View(vm);
        }

        //public async Task<IActionResult> Logs(CancellationToken cancellationToken = default)
        //{
        //    var logs = await _emailLogService.GetRecentAsync(100, cancellationToken);
        //    var vm = logs.Select(x => new EmailLogListItemViewModel
        //    {
        //        EmailLogId = x.EmailLogId,
        //        ToEmail = x.ToEmail,
        //        Subject = x.Subject,
        //        IsSent = x.IsSent,
        //        SentAt = x.SentAt,
        //        ErrorMessage = x.ErrorMessage,
        //        SourceModule = x.SourceModule,
        //        CreatedAt = x.CreatedAt
        //    }).ToList();

        //    return View(vm);
        //}

        private Guid GetUserId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
        }
    }
}
