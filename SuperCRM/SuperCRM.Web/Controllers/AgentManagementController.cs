using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperCRM.Application.DTOs.AgentManagement;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Enums;
using SuperCRM.Shared;
using SuperCRM.Web.ViewModels.AgentManagement;
using System.Security.Claims;

namespace SuperCRM.Web.Controllers
{
    /// <summary>
    /// SuperAdmin/SuperCRMAdmin Agent management controller.
    /// Provides Agent list/search and View/Edit registration approval workflow.
    /// </summary>
    [Authorize(Roles = AppRoles.SuperAdmin + "," + AppRoles.SuperCRMAdmin)]
    public class AgentManagementController : Controller
    {
        private readonly IAgentManagementService _service;

        public AgentManagementController(IAgentManagementService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? searchText,
            bool? isApproved,
            byte? registrationStatus,
            CancellationToken cancellationToken)
        {
            var items = await _service.SearchAsync(new AgentSearchDto
            {
                SearchText = searchText,
                IsApproved = isApproved,
                RegistrationStatus = registrationStatus
            }, cancellationToken);

            var model = new AgentListViewModel
            {
                SearchText = searchText,
                IsApproved = isApproved,
                RegistrationStatus = registrationStatus,
                Items = items.Select(x => new AgentListItemViewModel
                {
                    AgentId = x.AgentId,
                    AgentCode = x.AgentCode,
                    Email = x.Email,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNo = x.PhoneNo,
                    MobileNo = x.MobileNo,
                    IsApproved = x.IsApproved,
                    RegistrationStatus = x.RegistrationStatus,
                    RegistrationStatusName = x.RegistrationStatusName
                }).ToList()
            };

            PopulateSearchOptions(model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ViewRegistration(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _service.GetDetailsAsync(id, cancellationToken);
            if (dto == null)
                return NotFound();

            var model = new AgentRegistrationEditViewModel
            {
                AgentId = dto.AgentId,
                UserId = dto.UserId,
                AgentCode = dto.AgentCode,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNo = dto.PhoneNo,
                MobileNo = dto.MobileNo,
                IsApproved = dto.IsApproved,
                IsCommissionEligible = dto.IsCommissionEligible,
                RegistrationStatus = dto.RegistrationStatus,
                ApprovedAt = dto.ApprovedAt,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };

            PopulateRegistrationStatusOptions(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRegistration(
            AgentRegistrationEditViewModel model,
            CancellationToken cancellationToken)
        {
            PopulateRegistrationStatusOptions(model);

            if (!ModelState.IsValid)
                return View("ViewRegistration", model);

            var currentUserId = GetCurrentUserId();
            if (currentUserId == Guid.Empty)
                return Unauthorized();

            var result = await _service.UpdateRegistrationAsync(new UpdateAgentRegistrationDto
            {
                AgentId = model.AgentId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNo = model.PhoneNo,
                MobileNo = model.MobileNo,
                IsCommissionEligible = model.IsCommissionEligible,
                RegistrationStatus = model.RegistrationStatus,
                UpdatedByUserId = currentUserId
            }, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View("ViewRegistration", model);
            }

            TempData["SuccessMessage"] = "Agent registration updated successfully.";
            return RedirectToAction(nameof(ViewRegistration), new { id = model.AgentId });
        }

        private void PopulateSearchOptions(AgentListViewModel model)
        {
            model.IsApprovedOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "-- All --" },
                new SelectListItem { Value = "true", Text = "Approved" },
                new SelectListItem { Value = "false", Text = "Not Approved" }
            };

            model.RegistrationStatusOptions = BuildRegistrationStatusOptions(includeAllOption: true);
        }

        private void PopulateRegistrationStatusOptions(AgentRegistrationEditViewModel model)
        {
            model.RegistrationStatusOptions = BuildRegistrationStatusOptions(includeAllOption: false);
        }

        private static List<SelectListItem> BuildRegistrationStatusOptions(bool includeAllOption)
        {
            var list = new List<SelectListItem>();

            if (includeAllOption)
                list.Add(new SelectListItem { Value = "", Text = "-- All --" });

            list.AddRange(Enum.GetValues<AgentRegistrationStatus>()
                .Select(x => new SelectListItem
                {
                    Value = ((byte)x).ToString(),
                    Text = x.ToString()
                }));

            return list;
        }

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userId, out var id) ? id : Guid.Empty;
        }
    }
}
