using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperCRM.Application.DTOs.ProductVariants;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Shared;
using SuperCRM.Web.ViewModels.ProductVariants;
using System.Security.Claims;

namespace SuperCRM.Web.Controllers
{
    /// <summary>
    /// MVC controller for Product Variant master setup.
    /// Thin controller:
    /// - binds ViewModel,
    /// - loads dropdowns,
    /// - calls Application service,
    /// - handles user messages/navigation.
    /// No DbContext/EF logic should be placed here.
    /// </summary>
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public class ProductVariantsController : Controller
    {
        private readonly IProductVariantService _service;

        public ProductVariantsController(IProductVariantService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var items = await _service.GetAllAsync(cancellationToken);

            var model = items.Select(x => new ProductVariantListItemViewModel
            {
                ProductVariantId = x.ProductVariantId,
                ProductCode = x.ProductCode,
                ProductName = x.ProductName,
                VariantCode = x.VariantCode,
                VariantTypeName = string.IsNullOrWhiteSpace(x.VariantTypeName) ? x.VariantTypeCode : x.VariantTypeName,
                VariantName = x.VariantName,
                DisplayStyle = x.DisplayStyle,
                DisplayOrder = x.DisplayOrder,
                BasePrice = x.BasePrice
            }).ToList();

            return View(model);
        }

        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var model = new ProductVariantCreateEditViewModel
            {
                IsEditMode = false,
                DisplayStyle = 0
            };

            await PopulateLookupAsync(model, cancellationToken);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVariantCreateEditViewModel model, CancellationToken cancellationToken)
        {
            await PopulateLookupAsync(model, cancellationToken);

            if (!ModelState.IsValid)
                return View(model);

            var request = new CreateProductVariantDto
            {
                ProductId = model.ProductId,
                VariantCode = model.VariantCode,
                VariantTypeCode = model.VariantTypeCode,
                VariantName = model.VariantName,
                //DisplayStyle = model.DisplayStyle,
                DisplayStyle = model.DisplayStyle!.Value,

                DisplayOrder = model.DisplayOrder,
                BasePrice = model.BasePrice,
                UpdatedByUserId = GetCurrentUserId()
            };

            var result = await _service.CreateAsync(request, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(model);
            }

            TempData["SuccessMessage"] = "Product Variant created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _service.GetByIdAsync(id, cancellationToken);
            if (dto == null)
                return NotFound();

            var model = new ProductVariantCreateEditViewModel
            {
                ProductVariantId = dto.ProductVariantId,
                ProductId = dto.ProductId,
                VariantCode = dto.VariantCode,
                VariantTypeCode = dto.VariantTypeCode,
                VariantName = dto.VariantName,
                DisplayStyle = dto.DisplayStyle,
                DisplayOrder = dto.DisplayOrder,
                BasePrice = dto.BasePrice,
                IsEditMode = true
            };

            await PopulateLookupAsync(model, cancellationToken);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductVariantCreateEditViewModel model, CancellationToken cancellationToken)
        {
            await PopulateLookupAsync(model, cancellationToken);

            if (!ModelState.IsValid)
            {
                model.IsEditMode = true;
                return View(model);
            }

            if (!model.ProductVariantId.HasValue)
                return BadRequest();

            var request = new UpdateProductVariantDto
            {
                ProductVariantId = model.ProductVariantId.Value,
                ProductId = model.ProductId,
                VariantCode = model.VariantCode,
                VariantTypeCode = model.VariantTypeCode,
                VariantName = model.VariantName,
                //DisplayStyle = model.DisplayStyle,
                DisplayStyle = model.DisplayStyle!.Value,
                DisplayOrder = model.DisplayOrder,
                BasePrice = model.BasePrice,
                UpdatedByUserId = GetCurrentUserId()
            };

            var result = await _service.UpdateAsync(request, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                model.IsEditMode = true;
                return View(model);
            }

            TempData["SuccessMessage"] = "Product Variant updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateLookupAsync(ProductVariantCreateEditViewModel model, CancellationToken cancellationToken)
        {
            var lookup = await _service.GetFormLookupAsync(cancellationToken);

            model.ProductOptions = lookup.ProductOptions
                .Select(x => new SelectListItem { Value = x.Value, Text = x.Text })
                .ToList();

            model.VariantTypeOptions = lookup.VariantTypeOptions
                .Select(x => new SelectListItem { Value = x.Value, Text = x.Text })
                .ToList();

            model.DisplayStyleOptions = lookup.DisplayStyleOptions
                .Select(x => new SelectListItem { Value = x.Value, Text = x.Text })
                .ToList();
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
