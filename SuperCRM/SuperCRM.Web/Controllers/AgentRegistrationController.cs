using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperCRM.Application.DTOs.Agents;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Web.ViewModels.Agents;

namespace SuperCRM.Web.Controllers
{
    /// <summary>
    /// Public registration controller for new Agents.
    /// No direct database code is allowed here; everything goes through the Application service.
    /// </summary>
    [AllowAnonymous]
    public class AgentRegistrationController : Controller
    {
        private readonly IAgentRegistrationService _agentRegistrationService;

        public AgentRegistrationController(IAgentRegistrationService agentRegistrationService)
        {
            _agentRegistrationService = agentRegistrationService;
        }

        /// <summary>
        /// Shows the public Agent registration form.
        /// </summary>
        [HttpGet]
        public IActionResult Register()
        {
            return View(new AgentRegistrationViewModel());
        }

        /// <summary>
        /// Handles Agent registration form submission.
        /// All domain and data logic are delegated to the Application service.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(AgentRegistrationViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new AgentRegistrationRequestDto
            {
                Email = model.Email,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNo = model.PhoneNo,
                MobileNo = model.MobileNo,
                HouseNo = model.HouseNo,
                RoadName = model.RoadName,
                City = model.City,
                PostCode = model.PostCode,
                CountryId = model.CountryId,
                UpdatedByUserId = null
            };

            var result = await _agentRegistrationService.RegisterAsync(dto, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(model);
            }

            TempData["AgentCode"] = result.AgentCode;
            return RedirectToAction(nameof(RegistrationSuccess));
        }

        /// <summary>
        /// Shows a post-registration success page.
        /// Agent account is created, but approval is still pending in the current business flow.
        /// </summary>
        [HttpGet]
        public IActionResult RegistrationSuccess()
        {
            ViewBag.AgentCode = TempData["AgentCode"]?.ToString();
            return View();
        }
    }
}
