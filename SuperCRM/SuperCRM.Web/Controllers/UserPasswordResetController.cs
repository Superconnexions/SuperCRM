using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperCRM.Persistence.Identity;
using SuperCRM.Shared;
using SuperCRM.Web.ViewModels.UserPasswordReset;

namespace SuperCRM.Web.Controllers
{
    [Authorize(Roles = AppRoles.SuperAdmin)]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public class UserPasswordResetController : Controller
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly ILogger<UserPasswordResetController> _logger;
        private const int PageSize = 50;

        public UserPasswordResetController(UserManager<ApplicationUser> users,
            ILogger<UserPasswordResetController> logger)
        {
            _users = users;
            _logger = logger;
        }

        // Recheck the current administrator against the database, including activity.
        private async Task<ApplicationUser?> GetAdministratorAsync()
        {
            var administrator = await _users.GetUserAsync(User);
            return administrator != null && administrator.IsActive
                && await _users.IsInRoleAsync(administrator, AppRoles.SuperAdmin)
                ? administrator : null;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            if (await GetAdministratorAsync() == null) return Forbid();
            var query = _users.Users.AsNoTracking();
            search = search?.Trim();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u => (u.UserName != null && u.UserName.Contains(search))
                    || (u.Email != null && u.Email.Contains(search))
                    || (u.PhoneNumber != null && u.PhoneNumber.Contains(search)));
            }
            var total = await query.CountAsync();
            var pages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
            page = Math.Clamp(page, 1, pages);
            var accounts = await query.OrderBy(u => u.UserName).ThenBy(u => u.Id)
                .Skip((page - 1) * PageSize).Take(PageSize).ToListAsync();
            var model = new UserPasswordResetListViewModel
            {
                Search = search, Page = page, TotalPages = pages, TotalUsers = total
            };
            foreach (var account in accounts)
            {
                var roles = await _users.GetRolesAsync(account);
                model.Users.Add(new UserPasswordResetRowViewModel
                {
                    UserId = account.Id, UserName = account.UserName ?? "-",
                    Email = account.Email ?? "-", PhoneNumber = account.PhoneNumber ?? "-",
                    UserType = roles.Count == 0 ? "No role assigned" : string.Join(", ", roles.OrderBy(x => x)),
                    IsActive = account.IsActive, EmailConfirmed = account.EmailConfirmed,
                    CreatedAt = account.CreatedAt
                });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ResetPassword(Guid id)
        {
            if (await GetAdministratorAsync() == null) return Forbid();
            var account = await _users.FindByIdAsync(id.ToString());
            if (account == null) return NotFound();
            var model = new AdminResetUserPasswordViewModel { UserId = account.Id };
            await PopulateAccountAsync(model, account);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(AdminResetUserPasswordViewModel model)
        {
            var administrator = await GetAdministratorAsync();
            if (administrator == null) return Forbid();
            var account = await _users.FindByIdAsync(model.UserId.ToString());
            if (account == null) return NotFound();
            // Read identity details from the database, never from posted display fields.
            await PopulateAccountAsync(model, account);
            if (!ModelState.IsValid) return View(model);

            var token = await _users.GeneratePasswordResetTokenAsync(account);
            account.UpdatedAt = DateTime.UtcNow;
            account.UpdatedByUserId = administrator.Id;
            var result = await _users.ResetPasswordAsync(account, token, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }
            // Identity updates the password hash and security stamp. Do not log credentials.
            _logger.LogInformation("SuperAdmin {AdministratorId} reset password for user {TargetUserId}.",
                administrator.Id, account.Id);
            TempData["AdminPasswordResetSuccess"] =
                $"Password reset successfully for {account.UserName}."
                + (account.IsActive ? "" : " The account remains inactive.");
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateAccountAsync(AdminResetUserPasswordViewModel model, ApplicationUser account)
        {
            model.UserName = account.UserName;
            model.Email = account.Email;
            model.IsActive = account.IsActive;
            var roles = await _users.GetRolesAsync(account);
            model.UserType = roles.Count == 0 ? "No role assigned" : string.Join(", ", roles.OrderBy(x => x));
        }
    }
}
