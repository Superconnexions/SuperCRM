using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperCRM.Application.DTOs.ProductBaseCommissions;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Shared;
using SuperCRM.Web.ViewModels.ProductBaseCommissions;
using System.Security.Claims;

namespace SuperCRM.Web.Controllers
{
    [Authorize(Roles = AppRoles.SuperAdmin + "," + AppRoles.SuperCRMAdmin)]
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
                EffectiveTo = dto.EffectiveTo,
                IsActive = dto.IsActive
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
                IsActive = vm.IsActive,
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
            
            //var products = await _service.GetActiveProductsAsync(cancellationToken);
            var products = await _service.GetProductsAsync(cancellationToken);

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


        /// Productbase commission override
        /// 

        [HttpGet]

        public async Task<IActionResult> VariantOverrides(
        CancellationToken cancellationToken = default)
        {
            var dtoItems =
                await _service
                .GetProductVariantCommissionOverridesAsync(
                    null,
                    cancellationToken);

            var model =
                new ProductVariantCommissionOverrideIndexViewModel
                {
                    ProductKeyword = null,

                    Items =
                        MapVariantOverrideList(dtoItems)
                };

            return View(model);
        }

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult>
        GetProductVariantCommissionByProductNameOrCode(

        ProductVariantCommissionOverrideIndexViewModel model,

        CancellationToken cancellationToken = default)
        {
            var dtoItems =
                await _service
                .GetProductVariantCommissionOverridesAsync(

                    model.ProductKeyword,

                    cancellationToken);

            model.Items =
                MapVariantOverrideList(dtoItems);

            return View(

                "VariantOverrides",

                model);
        }

        [HttpGet]

        public async Task<IActionResult>
        CreateVariantOverride(

        CancellationToken cancellationToken = default)
        {
            var model =
                new ProductVariantCommissionOverrideCreateEditViewModel
                {
                    IsActive = true,

                    Products =
                        await GetProductSelectListAsync(
                            cancellationToken)
                };

            return View(model);
        }


        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult>
        CreateVariantOverride(

        ProductVariantCommissionOverrideCreateEditViewModel model,

        CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                model.Products =
                    await GetProductSelectListAsync(
                        cancellationToken);

                model.Variants =

                    model.ProductId == Guid.Empty

                    ? new()

                    : await GetVariantSelectListAsync(

                        model.ProductId,

                        cancellationToken);

                return View(model);
            }


            await _service
                .CreateProductVariantCommissionOverrideAsync(

                new SaveProductVariantCommissionOverrideDto
                {
                    ProductId = model.ProductId,

                    ProductVariantId =
                        model.ProductVariantId,

                    ExtraCommissionAmount =
                        model.ExtraCommissionAmount,

                    EffectiveFrom =
                        model.EffectiveFrom,

                    EffectiveTo =
                        model.EffectiveTo,

                    IsActive =
                        model.IsActive,

                    Note =
                        model.Note,

                    CurrentUserId =
                        GetCurrentUserId()
                },

                cancellationToken);


            TempData["SuccessMessage"] =

                "Commission override created successfully.";


            return RedirectToAction(

                nameof(VariantOverrides));
        }

        [HttpGet]

        public async Task<IActionResult>
        EditVariantOverride(

        Guid id,

        CancellationToken cancellationToken = default)
        {
            var dto =
                await _service
                .GetProductVariantCommissionOverrideByIdAsync(
                    id,
                    cancellationToken);

            if (dto == null)
            {
                return NotFound();
            }

            var model =
                new ProductVariantCommissionOverrideCreateEditViewModel
                {
                    ProductVariantCommissionOverrideId =
                        dto.ProductVariantCommissionOverrideId,

                    ProductId =
                        dto.ProductId,

                    ProductVariantId =
                        dto.ProductVariantId,

                    ExtraCommissionAmount =
                        dto.ExtraCommissionAmount,

                    EffectiveFrom =
                        dto.EffectiveFrom,

                    EffectiveTo =
                        dto.EffectiveTo,

                    IsActive =
                        dto.IsActive,

                    Note =
                        dto.Note,

                    Products =
                        await GetProductSelectListAsync(
                            cancellationToken),

                    Variants =
                        await GetVariantSelectListAsync(
                            dto.ProductId,
                            cancellationToken)
                };

            return View(model);
        }

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult>
        EditVariantOverride(

        ProductVariantCommissionOverrideCreateEditViewModel model,

        CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                model.Products =
                    await GetProductSelectListAsync(
                        cancellationToken);

                model.Variants =

                    model.ProductId == Guid.Empty

                    ? new()

                    : await GetVariantSelectListAsync(

                        model.ProductId,

                        cancellationToken);

                return View(model);
            }


            await _service
                .UpdateProductVariantCommissionOverrideAsync(

                new SaveProductVariantCommissionOverrideDto
                {
                    ProductVariantCommissionOverrideId =
                        model.ProductVariantCommissionOverrideId,

                    ProductId =
                        model.ProductId,

                    ProductVariantId =
                        model.ProductVariantId,

                    ExtraCommissionAmount =
                        model.ExtraCommissionAmount,

                    EffectiveFrom =
                        model.EffectiveFrom,

                    EffectiveTo =
                        model.EffectiveTo,

                    IsActive =
                        model.IsActive,

                    Note =
                        model.Note,

                    CurrentUserId =
                        GetCurrentUserId()
                },

                cancellationToken);


            TempData["SuccessMessage"] =

                "Commission override updated successfully.";


            return RedirectToAction(

                nameof(VariantOverrides));
        }

        [HttpGet]

        public async Task<IActionResult>
        GetVariantsByProductId(

        Guid productId,

        CancellationToken cancellationToken = default)
        {
            var variants =
                await _service
                .GetVariantOptionsByProductIdAsync(

                    productId,

                    cancellationToken);

            return Json(

                variants.Select(x => new
                {
                    value = x.Id,

                    text = x.Text
                }));
        }

        // Helper VariantOverride

        private List<ProductVariantCommissionOverrideListItemViewModel>
        MapVariantOverrideList(

        List<ProductVariantCommissionOverrideDto> dtoItems)
        {
            return dtoItems

                .Select(x =>

                new ProductVariantCommissionOverrideListItemViewModel
                {
                    ProductVariantCommissionOverrideId =
                        x.ProductVariantCommissionOverrideId,

                    ProductCode =
                        x.ProductCode,

                    ProductName =
                        x.ProductName,

                    VariantCode =
                        x.VariantCode,

                    VariantName =
                        x.VariantName,

                    ExtraCommissionAmount =
                        x.ExtraCommissionAmount,

                    EffectiveFrom =
                        x.EffectiveFrom,

                    EffectiveTo =
                        x.EffectiveTo,

                    IsActive =
                        x.IsActive,

                    Note =
                        x.Note
                })

                .ToList();
        }


        private async Task<List<SelectListItem>>
        GetProductSelectListAsync(
        CancellationToken cancellationToken)
        {
            var items =
                await _service
                .GetProductOptionsAsync(
                    cancellationToken);

            return items

                .Select(x =>

                new SelectListItem
                {
                    Value = x.Id.ToString(),

                    Text = x.Text
                })

                .ToList();
        }

        private async Task<List<SelectListItem>>
        GetVariantSelectListAsync(

        Guid productId,

        CancellationToken cancellationToken)
        {
            var items =
                await _service
                .GetVariantOptionsByProductIdAsync(

                    productId,

                    cancellationToken);

            return items

                .Select(x =>

                new SelectListItem
                {
                    Value = x.Id.ToString(),

                    Text = x.Text
                })

                .ToList();
        }

        private Guid GetCurrentUserId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
        }

        // END


    }
}
