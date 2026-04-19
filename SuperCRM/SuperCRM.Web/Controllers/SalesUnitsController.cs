using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperCRM.Application.DTOs.SalesUnits;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Shared;
using SuperCRM.Web.ViewModels.SalesUnits;
using System.Security.Claims;

namespace SuperCRM.Web.Controllers
{
    /// <summary>
    /// MVC controller for Sales Unit master setup.
    /// Thin controller with no direct DbContext/EF logic.
    /// </summary>
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public class SalesUnitsController : Controller
    {
        private readonly ISalesUnitService _service;

        public SalesUnitsController(ISalesUnitService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var items = await _service.GetAllAsync(cancellationToken);

            var model = items.Select(x => new SalesUnitListItemViewModel
            {
                SalesUnitId = x.SalesUnitId,
                UnitCode = x.UnitCode,
                UnitName = x.UnitName,
                IsActive = x.IsActive
            }).ToList();

            return View(model);
        }

        public IActionResult Create()
        {
            return View(new SalesUnitCreateEditViewModel
            {
                IsActive = true,
                IsEditMode = false
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SalesUnitCreateEditViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var request = new CreateSalesUnitDto
            {
                UnitCode = model.UnitCode,
                UnitName = model.UnitName,
                IsActive = model.IsActive,
                UpdatedByUserId = GetCurrentUserId()
            };

            var result = await _service.CreateAsync(request, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(model);
            }

            TempData["SuccessMessage"] = "Sales Unit created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
        {
            var dto = await _service.GetByIdAsync(id, cancellationToken);
            if (dto == null)
                return NotFound();

            var model = new SalesUnitCreateEditViewModel
            {
                SalesUnitId = dto.SalesUnitId,
                UnitCode = dto.UnitCode,
                UnitName = dto.UnitName,
                IsActive = dto.IsActive,
                IsEditMode = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SalesUnitCreateEditViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                model.IsEditMode = true;
                return View(model);
            }

            if (!model.SalesUnitId.HasValue)
                return BadRequest();

            var request = new UpdateSalesUnitDto
            {
                SalesUnitId = model.SalesUnitId.Value,
                UnitCode = model.UnitCode,
                UnitName = model.UnitName,
                IsActive = model.IsActive,
                UpdatedByUserId = GetCurrentUserId()
            };

            var result = await _service.UpdateAsync(request, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                model.IsEditMode = true;
                return View(model);
            }

            TempData["SuccessMessage"] = "Sales Unit updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private Guid? GetCurrentUserId()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return string.IsNullOrWhiteSpace(currentUserId)
                ? null
                : Guid.Parse(currentUserId);
        }
    }
}
