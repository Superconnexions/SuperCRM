using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperCRM.Persistence.Identity;
using SuperCRM.Shared;
using SuperCRM.Web.ViewModels.AdminUsers;
using System.Security.Claims;

namespace SuperCRM.Web.Controllers
{
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public class AdminUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AdminUsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roleNames = new[] { AppRoles.SuperAdmin, AppRoles.SuperCRMAdmin };

            var users = await _userManager.Users
                .Where(x => x.Email != null)
                .OrderBy(x => x.Email)
                .ToListAsync();

            var result = new List<AdminUserListItemViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var matchedRole = roles.FirstOrDefault(r => roleNames.Contains(r));

                if (matchedRole == null)
                    continue;

                result.Add(new AdminUserListItemViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    RoleName = matchedRole,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                });
            }

            return View(result.OrderBy(x => x.RoleName).ThenBy(x => x.Email).ToList());
        }

        public IActionResult Create()
        {
            var model = new AdminUserCreateViewModel
            {
                IsActive = true,
                AvailableRoles = new List<string>
                {
                    AppRoles.SuperAdmin,
                    AppRoles.SuperCRMAdmin
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminUserCreateViewModel model)
        {
            model.AvailableRoles = new List<string>
            {
                AppRoles.SuperAdmin,
                AppRoles.SuperCRMAdmin
            };

            if (!ModelState.IsValid)
                return View(model);

            if (!model.AvailableRoles.Contains(model.SelectedRole))
            {
                ModelState.AddModelError(nameof(model.SelectedRole), "Invalid role selected.");
                return View(model);
            }

            var existingByEmail = await _userManager.FindByEmailAsync(model.Email.Trim());
            if (existingByEmail != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Email already exists.");
                return View(model);
            }

            var existingByUserName = await _userManager.FindByNameAsync(model.UserName.Trim());
            if (existingByUserName != null)
            {
                ModelState.AddModelError(nameof(model.UserName), "User name already exists.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                Email = model.Email.Trim(),
                UserName = model.UserName.Trim(),
                NormalizedEmail = model.Email.Trim().ToUpperInvariant(),
                NormalizedUserName = model.UserName.Trim().ToUpperInvariant(),
                EmailConfirmed = true,
                IsActive = model.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);

            if (!roleResult.Succeeded)
            {
                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                await _userManager.DeleteAsync(user);
                return View(model);
            }

            TempData["SuccessMessage"] = "Admin user created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var currentRole = roles.FirstOrDefault(r =>
                r == AppRoles.SuperAdmin || r == AppRoles.SuperCRMAdmin);

            var model = new AdminUserEditViewModel
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                SelectedRole = currentRole ?? string.Empty,
                IsActive = user.IsActive,
                AvailableRoles = new List<string>
                {
                    AppRoles.SuperAdmin,
                    AppRoles.SuperCRMAdmin
                }
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AdminUserEditViewModel model)
        {
            model.AvailableRoles = new List<string>
            {
                AppRoles.SuperAdmin,
                AppRoles.SuperCRMAdmin
            };

            if (!ModelState.IsValid)
                return View(model);

            if (!model.AvailableRoles.Contains(model.SelectedRole))
            {
                ModelState.AddModelError(nameof(model.SelectedRole), "Invalid role selected.");
                return View(model);
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Id == model.UserId);
            if (user == null)
                return NotFound();

            var duplicateEmail = await _userManager.Users
                .FirstOrDefaultAsync(x => x.Email == model.Email && x.Id != model.UserId);

            if (duplicateEmail != null)
            {
                ModelState.AddModelError(nameof(model.Email), "Email already exists.");
                return View(model);
            }

            var duplicateUserName = await _userManager.Users
                .FirstOrDefaultAsync(x => x.UserName == model.UserName && x.Id != model.UserId);

            if (duplicateUserName != null)
            {
                ModelState.AddModelError(nameof(model.UserName), "User name already exists.");
                return View(model);
            }

            user.Email = model.Email.Trim();
            user.UserName = model.UserName.Trim();
            user.NormalizedEmail = model.Email.Trim().ToUpperInvariant();
            user.NormalizedUserName = model.UserName.Trim().ToUpperInvariant();
            user.IsActive = model.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isSelfEdit = !string.IsNullOrWhiteSpace(currentUserId) && Guid.Parse(currentUserId) == model.UserId;



            if (isSelfEdit)
            {
                if (!model.IsActive)
                {
                    ModelState.AddModelError(nameof(model.IsActive), "You cannot deactivate your own SuperAdmin account.");
                    return View(model);
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                var isCurrentlySuperAdmin = currentRoles.Contains(AppRoles.SuperAdmin);

                if (isCurrentlySuperAdmin && model.SelectedRole != AppRoles.SuperAdmin)
                {
                    ModelState.AddModelError(nameof(model.SelectedRole), "You cannot remove your own SuperAdmin role.");
                    return View(model);
                }
            }

            user.UpdatedByUserId = string.IsNullOrWhiteSpace(currentUserId)
                ? null
                : Guid.Parse(currentUserId);

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            var existingRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in existingRoles)
            {
                if (role == AppRoles.SuperAdmin || role == AppRoles.SuperCRMAdmin)
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                }
            }

            var addRoleResult = await _userManager.AddToRoleAsync(user, model.SelectedRole);

            if (!addRoleResult.Succeeded)
            {
                foreach (var error in addRoleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            TempData["SuccessMessage"] = "Admin user updated successfully.";
            return RedirectToAction(nameof(Index));
        }

    }
}