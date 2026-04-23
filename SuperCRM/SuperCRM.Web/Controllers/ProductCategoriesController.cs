using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperCRM.Application.DTOs.ProductCategories;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Shared;
using SuperCRM.Web.ViewModels.ProductCategories;
using System.Security.Claims;

namespace SuperCRM.Web.Controllers
{
    /// <summary>
    /// MVC controller for Product Category master setup.
    /// Thin controller:
    /// - binds ViewModel,
    /// - calls Application service,
    /// - handles user messages/navigation.
    /// No DbContext/EF logic should be placed here.
    /// </summary>
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public class ProductCategoriesController : Controller
    {
        private readonly IProductCategoryService _service;

        public ProductCategoriesController(IProductCategoryService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var items = await _service.GetAllAsync(cancellationToken);

            var model = items.Select(x => new ProductCategoryListItemViewModel
            {
                CategoryId = x.CategoryId,
                CategoryCode = x.CategoryCode,
                CategoryName = x.CategoryName,
                CategoryImageUrl = x.CategoryImageUrl,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            }).ToList();

            return View(model);
        }

        public IActionResult Create()
        {
            return View(new ProductCategoryCreateEditViewModel
            {
                IsActive = true,
                IsEditMode = false
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCategoryCreateEditViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return View(model);

            var request = new CreateProductCategoryDto
            {
                CategoryCode = model.CategoryCode,
                CategoryName = model.CategoryName,
                CategoryImageUrl = model.CategoryImageUrl,
                DisplayNotes = model.DisplayNotes,
                DisplayOrder = model.DisplayOrder,
                IsActive = model.IsActive,
                UpdatedByUserId = GetCurrentUserId()
            };

            var result = await _service.CreateAsync(request, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(model);
            }

            TempData["SuccessMessage"] = "Product Category created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _service.GetByIdAsync(id, cancellationToken);
            if (dto == null)
                return NotFound();

            var model = new ProductCategoryCreateEditViewModel
            {
                CategoryId = dto.CategoryId,
                CategoryCode = dto.CategoryCode,
                CategoryName = dto.CategoryName,
                CategoryImageUrl = dto.CategoryImageUrl,
                DisplayNotes = dto.DisplayNotes,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                IsEditMode = true
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductCategoryCreateEditViewModel model, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                model.IsEditMode = true;
                return View(model);
            }

            if (!model.CategoryId.HasValue)
                return BadRequest();

            var request = new UpdateProductCategoryDto
            {
                CategoryId = model.CategoryId.Value,
                CategoryCode = model.CategoryCode,
                CategoryName = model.CategoryName,
                CategoryImageUrl = model.CategoryImageUrl,
                DisplayNotes = model.DisplayNotes,
                DisplayOrder = model.DisplayOrder,
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

            TempData["SuccessMessage"] = "Product Category updated successfully.";
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
