using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SuperCRM.Application.DTOs.Products;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Enums;
using SuperCRM.Shared;
using SuperCRM.Web.ViewModels.Products;
using System.Security.Claims;

namespace SuperCRM.Web.Controllers
{
    /// <summary>
    /// MVC controller for Product setup.
    /// Controller keeps EF Core and business rules out of the web layer.
    /// File upload is converted to URL before calling the application service.
    /// </summary>
    [Authorize(Roles = AppRoles.SuperAdmin)]
    public class ProductsController : Controller
    {
        private readonly IProductService _service;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductsController(IProductService service, IWebHostEnvironment webHostEnvironment)
        {
            _service = service;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var items = await _service.GetAllAsync(cancellationToken);

            var model = items.Select(x => new ProductListItemViewModel
            {
                ProductId = x.ProductId,
                ProductCode = x.ProductCode,
                ProductName = x.ProductName,
                ProductDisplayName = x.ProductDisplayName,
                CategoryName = x.CategoryName,
                SalesUnitCode = x.SalesUnitCode,
                BasePriceType = x.BasePriceType.ToString(),
                BasePrice = x.BasePrice,
                CurrencyCode = x.CurrencyCode,
                ImageCount = x.Images.Count,
                ProductType = x.ProductType,
                NoOfInstallment = x.NoOfInstallment,
                MonthlyInstallmentAmount = x.MonthlyInstallmentAmount,
                IsActive = x.IsActive
            }).ToList();

            return View(model);
        }

        public async Task<IActionResult> Create(CancellationToken cancellationToken)
        {
            var model = new ProductCreateEditViewModel
            {
                IsActive = true,
                IsEditMode = false
            };

            await LoadLookupsAsync(model, cancellationToken);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateEditViewModel model, CancellationToken cancellationToken)
        {
            await LoadLookupsAsync(model, cancellationToken);

            if (!ModelState.IsValid)
                return View(model);

            var images = await SaveUploadedImagesAsync(model.Images, cancellationToken);

            var request = new CreateProductDto
            {
                CategoryId = model.CategoryId,
                SalesUnitId = model.SalesUnitId,
                ProductCode = model.ProductCode,
                ProductName = model.ProductName,
                ProductDisplayName = model.ProductDisplayName,
                ProductType = model.ProductType,
                CustomerType = model.CustomerType,
                ProductDescription = model.ProductDescription,
                IsThirdPartyProduct = model.IsThirdPartyProduct,
                InstallmentApplicable = model.InstallmentApplicable,
                DownPaymentAmount = model.DownPaymentAmount,
                NoOfInstallment = model.NoOfInstallment,
                MonthlyInstallmentAmount = model.MonthlyInstallmentAmount,
                CurrencyCode = model.CurrencyCode,
                IsRequiredBankInformation = model.IsRequiredBankInformation,
                IsProviderDeliveryProduct = model.IsProviderDeliveryProduct,
                BasePriceType = model.BasePriceType,
                BasePrice = model.BasePrice,
                IsPriceEditable = model.IsPriceEditable,
                IsPortalVisible = model.IsPortalVisible,
                IsPortalOrderEnabled = model.IsPortalOrderEnabled,
                DisplayOrder = model.DisplayOrder,
                ProductDisplayNotes = model.ProductDisplayNotes,
                PaymentNotes = model.PaymentNotes,
                Remarks = model.Remarks,
                IsActive = model.IsActive,
                UpdatedByUserId = GetCurrentUserId(),
                Images = images
            };

            var result = await _service.CreateAsync(request, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                return View(model);
            }

            TempData["SuccessMessage"] = "Product created successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _service.GetByIdAsync(id, cancellationToken);
            if (dto == null)
                return NotFound();

            var model = new ProductCreateEditViewModel
            {
                ProductId = dto.ProductId,
                CategoryId = dto.CategoryId,
                SalesUnitId = dto.SalesUnitId,
                ProductCode = dto.ProductCode,
                ProductName = dto.ProductName,
                ProductDisplayName = dto.ProductDisplayName,
                ProductType = dto.ProductType,
                CustomerType = dto.CustomerType,
                ProductDescription = dto.ProductDescription,
                IsThirdPartyProduct = dto.IsThirdPartyProduct,
                InstallmentApplicable = dto.InstallmentApplicable,
                DownPaymentAmount = dto.DownPaymentAmount,
                NoOfInstallment = dto.NoOfInstallment,
                MonthlyInstallmentAmount = dto.MonthlyInstallmentAmount ?? 0m, // Fix: provide default value if null
                CurrencyCode = dto.CurrencyCode,
                IsRequiredBankInformation = dto.IsRequiredBankInformation,
                IsProviderDeliveryProduct = dto.IsProviderDeliveryProduct,
                BasePriceType = dto.BasePriceType,
                BasePrice = dto.BasePrice,
                IsPriceEditable = dto.IsPriceEditable,
                IsPortalVisible = dto.IsPortalVisible,
                IsPortalOrderEnabled = dto.IsPortalOrderEnabled,
                DisplayOrder = dto.DisplayOrder,
                ProductDisplayNotes = dto.ProductDisplayNotes,
                PaymentNotes = dto.PaymentNotes,
                Remarks = dto.Remarks,
                IsActive = dto.IsActive,
                IsEditMode = true,
                Images = dto.Images.Count == 0
                    ? new List<ProductImageInputViewModel>
                    {
                        new ProductImageInputViewModel { DisplayOrder = 1, IsPrimary = true }
                    }
                    : dto.Images.Select(x => new ProductImageInputViewModel
                    {
                        ProductImageId = x.ProductImageId,
                        ExistingImageUrl = x.ImageUrl,
                        DisplayOrder = x.DisplayOrder,
                        IsPrimary = x.IsPrimary
                    }).ToList()
            };

            await LoadLookupsAsync(model, cancellationToken);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductCreateEditViewModel model, CancellationToken cancellationToken)
        {
            await LoadLookupsAsync(model, cancellationToken);

            if (!ModelState.IsValid)
            {
                model.IsEditMode = true;
                return View(model);
            }

            if (!model.ProductId.HasValue)
                return BadRequest();

            var images = await SaveUploadedImagesAsync(model.Images, cancellationToken);

            var request = new UpdateProductDto
            {
                ProductId = model.ProductId.Value,
                CategoryId = model.CategoryId,
                SalesUnitId = model.SalesUnitId,
                ProductCode = model.ProductCode,
                ProductName = model.ProductName,
                ProductDisplayName = model.ProductDisplayName,
                ProductType = model.ProductType,
                CustomerType = model.CustomerType,
                ProductDescription = model.ProductDescription,
                IsThirdPartyProduct = model.IsThirdPartyProduct,
                InstallmentApplicable = model.InstallmentApplicable,
                DownPaymentAmount = model.DownPaymentAmount,
                NoOfInstallment = model.NoOfInstallment,
                MonthlyInstallmentAmount = model.MonthlyInstallmentAmount,
                CurrencyCode = model.CurrencyCode,
                IsRequiredBankInformation = model.IsRequiredBankInformation,
                IsProviderDeliveryProduct = model.IsProviderDeliveryProduct,
                BasePriceType = model.BasePriceType,
                BasePrice = model.BasePrice,
                IsPriceEditable = model.IsPriceEditable,
                IsPortalVisible = model.IsPortalVisible,
                IsPortalOrderEnabled = model.IsPortalOrderEnabled,
                DisplayOrder = model.DisplayOrder,
                ProductDisplayNotes = model.ProductDisplayNotes,
                PaymentNotes = model.PaymentNotes,
                Remarks = model.Remarks,
                IsActive = model.IsActive,
                UpdatedByUserId = GetCurrentUserId(),
                Images = images
            };

            var result = await _service.UpdateAsync(request, cancellationToken);

            if (!result.Success)
            {
                ModelState.AddModelError(string.Empty, result.ErrorMessage);
                model.IsEditMode = true;
                return View(model);
            }

            TempData["SuccessMessage"] = "Product updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadLookupsAsync(ProductCreateEditViewModel model, CancellationToken cancellationToken)
        {
            var lookups = await _service.GetFormLookupAsync(cancellationToken);

            model.CategoryOptions = lookups.Categories
                .Select(x => new SelectListItem(x.Text, x.Value))
                .ToList();

            model.SalesUnitOptions = lookups.SalesUnits
                .Select(x => new SelectListItem(x.Text, x.Value))
                .ToList();

            model.ProductTypeOptions = Enum.GetValues<ProductType>()
                .Select(x => new SelectListItem(x.ToString(), ((byte)x).ToString()))
                .ToList();

            model.CustomerTypeOptions = Enum.GetValues<ProductCustomerType>()
                .Select(x => new SelectListItem(x.ToString(), ((byte)x).ToString()))
                .ToList();

            model.BasePriceTypeOptions = Enum.GetValues<ProductBasePriceType>()
                .Select(x => new SelectListItem(x.ToString(), ((byte)x).ToString()))
                .ToList();
        }

        private async Task<List<ProductImageDto>> SaveUploadedImagesAsync(List<ProductImageInputViewModel> imageRows, CancellationToken cancellationToken)
        {
            var results = new List<ProductImageDto>();

            if (imageRows == null || imageRows.Count == 0)
                return results;

            var uploadFolderPhysical = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "products");
            Directory.CreateDirectory(uploadFolderPhysical);

            foreach (var row in imageRows)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (row.RemoveImage)
                    continue;

                string? imageUrl = string.IsNullOrWhiteSpace(row.ExistingImageUrl)
                    ? null
                    : row.ExistingImageUrl.Trim();

                if (row.UploadFile != null && row.UploadFile.Length > 0)
                {
                    var extension = Path.GetExtension(row.UploadFile.FileName);
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

                    if (!allowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
                        throw new InvalidOperationException("Only image files are allowed for Product Images.");

                    var safeFileName = $"{Guid.NewGuid():N}{extension}";
                    var physicalPath = Path.Combine(uploadFolderPhysical, safeFileName);

                    await using var stream = new FileStream(physicalPath, FileMode.Create);
                    await row.UploadFile.CopyToAsync(stream, cancellationToken);

                    imageUrl = $"/uploads/products/{safeFileName}";
                }

                if (string.IsNullOrWhiteSpace(imageUrl))
                    continue;

                results.Add(new ProductImageDto
                {
                    ProductImageId = row.ProductImageId,
                    ImageUrl = imageUrl,
                    DisplayOrder = row.DisplayOrder,
                    IsPrimary = row.IsPrimary
                });
            }

            return results;
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
