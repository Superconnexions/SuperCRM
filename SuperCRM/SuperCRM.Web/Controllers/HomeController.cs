using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuperCRM.Persistence.Identity;
using SuperCRM.Shared;
using SuperCRM.Web.Models;
using SuperCRM.Web.ViewModels.Account;
using System.Diagnostics;
using System.Text.Encodings.Web;
using SuperCRM.Application.DTOs.EmailSettings;
using SuperCRM.Application.Interfaces.Services;

namespace SuperCRM.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSenderService _emailSender;

        public HomeController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IEmailSenderService emailSender,
            ILogger<HomeController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole(AppRoles.SuperAdmin))
                    return RedirectToAction("Index", "Admin");

                if (User.IsInRole(AppRoles.SuperCRMAdmin))
                    return RedirectToAction("Index", "Admin");

                if (User.IsInRole(AppRoles.Agent))
                    return RedirectToAction("Index", "AgentDashboard");
            }

            return View(new LoginViewModel());
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Index", model);

            var user = await _userManager.FindByNameAsync(model.UserName)
                       ?? await _userManager.FindByEmailAsync(model.UserName);

            if (user == null || !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View("Index", model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user, AppRoles.SuperAdmin))
                    return RedirectToAction("Index", "Admin");

                if (await _userManager.IsInRoleAsync(user, AppRoles.SuperCRMAdmin))
                    return RedirectToAction("Index", "Admin");

                if (await _userManager.IsInRoleAsync(user, AppRoles.Agent))
                    return RedirectToAction("Index", "AgentDashboard");

                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View("Index", model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel
            {
                Email = TempData["PasswordResetEmail"] as string ?? string.Empty
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            const string confirmation = "Please check your email and click the Reset Password link to reset your password. If you do not see the email, please check your spam or junk folder.";
            var user = await _userManager.FindByEmailAsync(model.Email.Trim());

            if (user == null || !user.IsActive || string.IsNullOrWhiteSpace(user.Email)
                || !await CanResetPasswordAsync(user))
            {
                ModelState.AddModelError(string.Empty,
                    "This email address is not linked to an active account eligible for password reset. Please check the email address or contact support.");
                return View(model);
            }

            if (user != null)
            {
                try
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetUrl = Url.Action(
                        nameof(ResetPassword),
                        "Home",
                        new { email = user.Email, token },
                        protocol: Request.Scheme);

                    if (string.IsNullOrWhiteSpace(resetUrl))
                        throw new InvalidOperationException("Unable to generate password reset link.");

                    // Do not store the reset token in the email log.
                    var result = await _emailSender.SendWithoutCreatingLogAsync(new SendEmailRequestDto
                    {
                        ToEmail = user.Email,
                        Subject = "SuperCRM - Reset your password",
                        Body = "<p>A password reset was requested for your SuperCRM account.</p>"
                            + "<p><a href=\"" + HtmlEncoder.Default.Encode(resetUrl)
                            + "\">Reset Password</a></p>"
                            + "<p>Click the link to choose and confirm your new password.</p>"
                            + "<p>If you did not request this, you can ignore this email.</p>",
                        IsHtml = true,
                        SourceModule = "PasswordReset"
                    }, HttpContext.RequestAborted);

                    if (!result.Success)
                    {
                        _logger.LogWarning("Password reset email delivery failed.");
                        ModelState.AddModelError(string.Empty,
                            "Your account is active, but we could not send the password reset email. Please try again later or contact support.");
                        return View(model);
                    }
                }
                catch (Exception)
                {
                    // Do not log the reset URL or token.
                    _logger.LogWarning("Unable to complete password reset email delivery.");
                    ModelState.AddModelError(string.Empty,
                        "Your account is active, but we could not send the password reset email. Please try again later or contact support.");
                    return View(model);
                }
            }

            TempData["PasswordResetLinkMessage"] = confirmation;
            TempData["PasswordResetEmail"] = model.Email.Trim();
            return RedirectToAction(nameof(ForgotPassword));
        }

        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token))
                return RedirectToAction(nameof(Index));

            var model = new ResetPasswordViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.IsActive || !await CanResetPasswordAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Invalid reset request.");
                return View(model);
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            TempData["SuccessMessage"] = "Password has been reset successfully. Please login.";
            return RedirectToAction(nameof(Index));
        }
        private async Task<bool> CanResetPasswordAsync(ApplicationUser user)
        {
            return await _userManager.IsInRoleAsync(user, AppRoles.SuperAdmin)
                || await _userManager.IsInRoleAsync(user, AppRoles.SuperCRMAdmin)
                || await _userManager.IsInRoleAsync(user, AppRoles.Agent);
        }
    }
}
