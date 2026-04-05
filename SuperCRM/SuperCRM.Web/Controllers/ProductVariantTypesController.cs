using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperCRM.Application.DTOs.ProductVariantTypes;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Shared;
using SuperCRM.Web.ViewModels.ProductVariantTypes;
using System.Security.Claims;

namespace SuperCRM.Web.Controllers
{
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public class ProductVariantTypesController : Controller
    {
        private readonly IProductVariantTypeService _service;

        public ProductVariantTypesController(IProductVariantTypeService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var items = await _service.GetAllAsync(cancellationToken);

            var model = items.Select(x => new ProductVariantTypeListItemViewModel
            {
                TypeCode = x.TypeCode,
                TypeValue = x.TypeValue,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            }).ToList();

            return View(model);
        }

        public IActionResult Create()
        {
            return View(new ProductVariantTypeCreateEditViewModel
            {
                IsActive = true,
                IsEditMode = false
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVariantTypeCreateEditViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var dto = new CreateProductVariantTypeDto
            {
                TypeCode = model.TypeCode,
                TypeValue = model.TypeValue,
                DisplayOrder = model.DisplayOrder,
                IsActive = model.IsActive,
                UpdatedByUserId = string.IsNullOrWhiteSpace(currentUserId) ? null : Guid.Parse(currentUserId)
            };

            var result = await _service.CreateAsync(dto, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(model);
            }

            TempData["SuccessMessage"] = "Product Variant Type created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest();

            var dto = await _service.GetByTypeCodeAsync(id, cancellationToken);
            if (dto == null)
                return NotFound();

            var model = new ProductVariantTypeCreateEditViewModel
            {
                TypeCode = dto.TypeCode,
                TypeValue = dto.TypeValue,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                IsEditMode = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductVariantTypeCreateEditViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                model.IsEditMode = true;
                return View(model);
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var dto = new UpdateProductVariantTypeDto
            {
                TypeCode = model.TypeCode,
                TypeValue = model.TypeValue,
                DisplayOrder = model.DisplayOrder,
                IsActive = model.IsActive,
                UpdatedByUserId = string.IsNullOrWhiteSpace(currentUserId) ? null : Guid.Parse(currentUserId)
            };

            var result = await _service.UpdateAsync(dto, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                model.IsEditMode = true;
                return View(model);
            }

            TempData["SuccessMessage"] = "Product Variant Type updated successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}