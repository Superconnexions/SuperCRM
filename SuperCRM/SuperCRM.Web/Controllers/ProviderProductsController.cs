using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperCRM.Application.DTOs.ProviderProducts;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Shared;
using SuperCRM.Web.ViewModels.ProviderProducts;
using System.Security.Claims;

namespace SuperCRM.Web.Controllers
{
    /// <summary>
    /// MVC controller for ProviderProducts setup.
    /// Includes list search by provider/product and delete action.
    /// </summary>
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public class ProviderProductsController : Controller
    {
        private readonly IProviderProductService _service;

        public ProviderProductsController(IProviderProductService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string? searchProviderName, string? searchProductName, CancellationToken cancellationToken)
        {
            var items = await _service.SearchAsync(new ProviderProductSearchDto
            {
                ProviderName = searchProviderName,
                ProductName = searchProductName
            }, cancellationToken);

            var model = new ProviderProductIndexViewModel
            {
                SearchProviderName = searchProviderName,
                SearchProductName = searchProductName,
                Items = items.Select(x => new ProviderProductListItemViewModel
                {
                    ProviderProductId = x.ProviderProductId,
                    ProviderName = x.ProviderName,
                    ProductCode = x.ProductCode,
                    ProductName = x.ProductName
                }).ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var model = new ProviderProductCreateEditViewModel { IsEditMode = false };
            await LoadLookupsAsync(model, cancellationToken);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProviderProductCreateEditViewModel model, CancellationToken cancellationToken)
        {
            await LoadLookupsAsync(model, cancellationToken);

            if (!ModelState.IsValid)
                return View(model);

            var result = await _service.CreateAsync(new CreateProviderProductDto
            {
                ProviderId = model.ProviderId,
                ProductId = model.ProductId,
                UpdatedByUserId = GetCurrentUserId()
            }, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(model);
            }

            TempData["SuccessMessage"] = "Provider Product created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _service.GetByIdAsync(id, cancellationToken);
            if (dto == null)
                return NotFound();

            var model = new ProviderProductCreateEditViewModel
            {
                ProviderProductId = dto.ProviderProductId,
                ProviderId = dto.ProviderId,
                ProductId = dto.ProductId,
                ProductCode = dto.ProductCode,
                ProductName = dto.ProductName,
                IsEditMode = true
            };

            await LoadLookupsAsync(model, cancellationToken);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProviderProductCreateEditViewModel model, CancellationToken cancellationToken)
        {
            await LoadLookupsAsync(model, cancellationToken);

            if (!ModelState.IsValid)
            {
                model.IsEditMode = true;
                return View(model);
            }

            if (!model.ProviderProductId.HasValue)
                return BadRequest();

            var result = await _service.UpdateAsync(new UpdateProviderProductDto
            {
                ProviderProductId = model.ProviderProductId.Value,
                ProviderId = model.ProviderId,
                ProductId = model.ProductId,
                UpdatedByUserId = GetCurrentUserId()
            }, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                model.IsEditMode = true;
                return View(model);
            }

            TempData["SuccessMessage"] = "Provider Product updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var result = await _service.DeleteAsync(id, cancellationToken);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Success
                ? "Provider Product deleted successfully."
                : result.ErrorMessage;

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadLookupsAsync(ProviderProductCreateEditViewModel model, CancellationToken cancellationToken)
        {
            var lookup = await _service.GetFormLookupAsync(cancellationToken);
            model.ProviderOptions = lookup.Providers.Select(x => new SelectListItem(x.Text, x.Value)).ToList();
            model.ProductOptions = lookup.Products.Select(x => new SelectListItem(x.Text, x.Value)).ToList();
        }

        private Guid? GetCurrentUserId()
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return string.IsNullOrWhiteSpace(currentUserId) ? null : Guid.Parse(currentUserId);
        }
    }
}
