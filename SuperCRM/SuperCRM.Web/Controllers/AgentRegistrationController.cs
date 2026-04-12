using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperCRM.Application.DTOs.AgentRegistration;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Web.ViewModels.AgentRegistration;

namespace SuperCRM.Web.Controllers
{
    /// <summary>
    /// Public controller for Agent self-registration.
    /// Responsibilities:
    /// 1. Render registration page.
    /// 2. Load Country / Region / City dropdown data.
    /// 3. Submit registration request to Application layer.
    /// 4. Return lightweight JSON for cascading dropdowns.
    /// </summary>
    [AllowAnonymous]
    public class AgentRegistrationController : Controller
    {
        private readonly IAgentRegistrationService _agentRegistrationService;
        private readonly IGeoLookupService _geoLookupService;

        public AgentRegistrationController(
            IAgentRegistrationService agentRegistrationService,
            IGeoLookupService geoLookupService)
        {
            _agentRegistrationService = agentRegistrationService;
            _geoLookupService = geoLookupService;
        }

        /// <summary>
        /// Shows public registration form.
        /// Initially loads Countries only.
        /// Regions and Cities are loaded dynamically after selection.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Register(CancellationToken cancellationToken)
        {
            var model = new AgentRegistrationViewModel();
            await PopulateCountriesAsync(model, cancellationToken);
            return View(model);
        }

        /// <summary>
        /// Handles public Agent registration form submit.
        /// Reloads dropdowns when validation fails so user does not lose selection context.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            AgentRegistrationViewModel model,
            CancellationToken cancellationToken)
        {
            await PopulateCountriesAsync(model, cancellationToken);

            if (model.CountryId.HasValue)
                await PopulateRegionsAsync(model, model.CountryId.Value, cancellationToken);

            if (model.RegionId.HasValue)
                await PopulateCitiesAsync(model, model.RegionId.Value, cancellationToken);

            if (!ModelState.IsValid)
                return View(model);

            var request = new AgentRegistrationRequestDto
            {
                Email = model.Email.Trim(),
                Password = model.Password,
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                PhoneNo = model.PhoneNo,
                MobileNo = model.MobileNo,
                HouseNo = model.HouseNo,
                RoadName = model.RoadName,
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                PostCode = model.PostCode,
                CountryId = model.CountryId,
                RegionId = model.RegionId,
                CityId = model.CityId,
                UpdatedByUserId = null
            };

            var result = await _agentRegistrationService.RegisterAsync(request, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(model);
            }

            //TempData["SuccessMessage"] = "Agent registration submitted successfully. Please wait for approval.";
            //return RedirectToAction("Index", "Home");

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

        /// <summary>
        /// AJAX endpoint for Region dropdown.
        /// Returns active Regions for selected Country.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetRegions(int countryId, CancellationToken cancellationToken)
        {
            var data = await _geoLookupService.GetRegionsByCountryIdAsync(countryId, cancellationToken);

            return Json(data.Select(x => new
            {
                id = x.Id,
                name = x.Name
            }));
        }

        /// <summary>
        /// AJAX endpoint for City dropdown.
        /// Returns active Cities for selected Region.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCities(int regionId, CancellationToken cancellationToken)
        {
            var data = await _geoLookupService.GetCitiesByRegionIdAsync(regionId, cancellationToken);

            return Json(data.Select(x => new
            {
                id = x.Id,
                name = x.Name
            }));
        }

        /// <summary>
        /// Loads Country dropdown.
        /// </summary>
        private async Task PopulateCountriesAsync(
            AgentRegistrationViewModel model,
            CancellationToken cancellationToken)
        {
            var countries = await _geoLookupService.GetCountriesAsync(cancellationToken);

            model.Countries = countries
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();
        }

        /// <summary>
        /// Loads Region dropdown for selected Country.
        /// </summary>
        private async Task PopulateRegionsAsync(
            AgentRegistrationViewModel model,
            int countryId,
            CancellationToken cancellationToken)
        {
            var regions = await _geoLookupService.GetRegionsByCountryIdAsync(countryId, cancellationToken);

            model.Regions = regions
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();
        }

        /// <summary>
        /// Loads City dropdown for selected Region.
        /// </summary>
        private async Task PopulateCitiesAsync(
            AgentRegistrationViewModel model,
            int regionId,
            CancellationToken cancellationToken)
        {
            var cities = await _geoLookupService.GetCitiesByRegionIdAsync(regionId, cancellationToken);

            model.Cities = cities
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToList();
        }
    }
}