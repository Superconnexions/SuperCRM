using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using SuperCRM.Application.DTOs.ProductBaseCommissions;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Web.ViewModels.ProductBaseCommissions;

namespace SuperCRM.Web.Controllers
{
    [Authorize]
    public class ProductBaseCommissionsController : Controller
    {
        private readonly IProductBaseCommissionService _service;

        public ProductBaseCommissionsController(IProductBaseCommissionService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string? productKeyword, DateTime? effectiveFrom, DateTime? effectiveTo, bool includeInactive = false, CancellationToken cancellationToken = default)
        {
            var items = await _service.SearchAsync(new ProductBaseCommissionSearchDto
            {
                ProductKeyword = productKeyword,
                EffectiveFrom = effectiveFrom,
                EffectiveTo = effectiveTo,
                IncludeInactive = includeInactive
            }, cancellationToken);

            return View(new ProductBaseCommissionIndexViewModel
            {
                ProductKeyword = productKeyword,
                EffectiveFrom = effectiveFrom,
                EffectiveTo = effectiveTo,
                
                IncludeInactive = includeInactive,
                Items = items
            });
        }

        public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
        {
            var vm = new ProductBaseCommissionCreateEditViewModel();
            await BindProductsAsync(vm, cancellationToken);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductBaseCommissionCreateEditViewModel vm, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                await BindProductsAsync(vm, cancellationToken);
                return View(vm);
            }

            var userId = GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _service.CreateAsync(new CreateProductBaseCommissionDto
            {
                ProductId = vm.ProductId,
                CommissionType = vm.CommissionType,
                FixedAmount = vm.FixedAmount,
                Percentage = vm.Percentage,
                EffectiveFrom = vm.EffectiveFrom,
                EffectiveTo = vm.EffectiveTo,
                CreatedByUserId = userId,
                CurrencyCode = vm.CurrencyCode,
                Note = vm.Note
            }, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                await BindProductsAsync(vm, cancellationToken);
                return View(vm);
            }

            TempData["SuccessMessage"] = "Product base commission created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken = default)
        {
            var dto = await _service.GetByIdAsync(id, cancellationToken);
            if (dto == null) return NotFound();

            var vm = new ProductBaseCommissionCreateEditViewModel
            {
                ProductBaseCommissionId = dto.ProductBaseCommissionId,
                ProductId = dto.ProductId,
                CommissionType = dto.CommissionType,
                FixedAmount = dto.FixedAmount,
                Percentage = dto.Percentage,
                Note = dto.Note,
                CurrencyCode = dto.CurrencyCode,
                EffectiveFrom = dto.EffectiveFrom,
                EffectiveTo = dto.EffectiveTo
            };

            await BindProductsAsync(vm, cancellationToken);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductBaseCommissionCreateEditViewModel vm, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid || !vm.ProductBaseCommissionId.HasValue)
            {
                await BindProductsAsync(vm, cancellationToken);
                return View(vm);
            }

            var userId = GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _service.UpdateAsync(new UpdateProductBaseCommissionDto
            {
                ProductBaseCommissionId = vm.ProductBaseCommissionId.Value,
                ProductId = vm.ProductId,
                CommissionType = vm.CommissionType,
                FixedAmount = vm.FixedAmount,
                Percentage = vm.Percentage,
                EffectiveFrom = vm.EffectiveFrom,
                EffectiveTo = vm.EffectiveTo,
                CurrencyCode = vm.CurrencyCode,               
                UpdatedByUserId = userId,
                Note = vm.Note
            }, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                await BindProductsAsync(vm, cancellationToken);
                return View(vm);
            }

            TempData["SuccessMessage"] = "Product base commission updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
        {
            var dto = await _service.GetByIdAsync(id, cancellationToken);
            if (dto == null) return NotFound();

            return View(new ProductBaseCommissionDeleteViewModel
            {
                ProductBaseCommissionId = dto.ProductBaseCommissionId,
                ProductCode = dto.ProductCode,
                ProductName = dto.ProductName,
                CommissionTypeText = dto.CommissionType.ToString(),
                FixedAmount = dto.FixedAmount,
                Percentage = dto.Percentage,
                EffectiveFrom = dto.EffectiveFrom,
                EffectiveTo = dto.EffectiveTo
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ProductBaseCommissionDeleteViewModel vm, CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _service.SoftDeleteAsync(vm.ProductBaseCommissionId, userId, vm.Note, cancellationToken);
            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(vm);
            }

            TempData["SuccessMessage"] = "Product base commission deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> History(Guid id, CancellationToken cancellationToken = default)
        {
            ViewBag.ProductBaseCommissionId = id;
            var items = await _service.GetHistoryAsync(id, cancellationToken);
            return View(items);
        }

        [HttpGet]
        public async Task<IActionResult> GetSmartCommission(Guid productId, DateTime orderDate, CancellationToken cancellationToken = default)
        {
            var dto = await _service.GetSmartCommissionAsync(productId, orderDate, cancellationToken);
            if (dto == null)
                return Json(new { found = false, message = "No active commission found." });

            return Json(new
            {
                found = true,
                dto.ProductBaseCommissionId,
                commissionType = dto.CommissionType.ToString(),
                dto.FixedAmount,
                dto.Percentage,
                dto.EffectiveFrom,
                dto.EffectiveTo
            });
        }

        [HttpGet]
        public async Task<IActionResult> Calculate(Guid productId, DateTime orderDate, decimal orderAmount, CancellationToken cancellationToken = default)
        {
            var result = await _service.CalculateCommissionAsync(productId, orderDate, orderAmount, cancellationToken);
            return Json(result);
        }

        private async Task BindProductsAsync(ProductBaseCommissionCreateEditViewModel vm, CancellationToken cancellationToken)
        {
            var products = await _service.GetActiveProductsAsync(cancellationToken);
            vm.ProductOptions = products
                .Select(x => new SelectListItem
                {
                    Value = x.ProductId.ToString(),
                    Text = $"{x.ProductName} ({x.ProductCode})"
                })
                .ToList();

            vm.CurrencyOptions = GetCurrencyOptions();
        }

        private Guid GetUserId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
        }

        private static List<SelectListItem> GetCurrencyOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Tk.", Text = "BDT" },
                new SelectListItem { Value = "£", Text = "GBP" }
            };
        }
    }
}
