using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuperCRM.Application.DTOs.SalesOrders;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Web.ViewModels.SalesOrders;
using System.Security.Claims;

namespace SuperCRM.Web.Controllers
{
    [Authorize]
    public class SalesOrderController : Controller
    {
        private readonly ISalesOrderService _salesOrderService;
        private readonly ISalesOrderDraftService _salesOrderDraftService;

        public SalesOrderController(
            ISalesOrderService salesOrderService,
            ISalesOrderDraftService salesOrderDraftService)
        {
            _salesOrderService = salesOrderService;
            _salesOrderDraftService = salesOrderDraftService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProductSelection(
        SalesOrderProductSelectionPostViewModel model,
        CancellationToken cancellationToken = default)
        {
            var result = await SaveProductSelectionInternalAsync(model, cancellationToken);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;

            return RedirectToAction(nameof(SalesOrderProductList), new { draftId = result.SalesOrderDraftId ?? model.SalesOrderDraftId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveProductSelectionAndNext(
            SalesOrderProductSelectionPostViewModel model,
            CancellationToken cancellationToken = default)
        {
            var result = await SaveProductSelectionInternalAsync(model, cancellationToken);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(SalesOrderProductList), new { draftId = model.SalesOrderDraftId });
            }

            return RedirectToAction(nameof(SalesOrderCustomerCreation), new { draftId = result.SalesOrderDraftId });
        }

        private async Task<SalesOrderDraftSaveResultDto> SaveProductSelectionInternalAsync(
            SalesOrderProductSelectionPostViewModel model,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return new SalesOrderDraftSaveResultDto
                {
                    Success = false,
                    Message = "Invalid login session."
                };
            }

            if (model.Lines == null || !model.Lines.Any())
            {
                return new SalesOrderDraftSaveResultDto
                {
                    Success = false,
                    Message = "Please select at least one product."
                };
            }

            return await _salesOrderDraftService.SaveProductSelectionAsync(new SaveSalesOrderProductSelectionDto
            {
                SalesOrderDraftId = model.SalesOrderDraftId,
                CurrentUserId = userId,

                //Lines = model.Lines.Select(x => new SaveSalesOrderDraftLineDto
                //{
                //    ProductId = x.ProductId,
                //    ProductVariantId = x.ProductVariantId,
                //    ProviderProductId = x.ProviderProductId,
                //    Quantity = x.Quantity <= 0 ? 1 : x.Quantity,
                //    SalePrice = x.SalePrice,
                //    FirstInstallmentDate = x.FirstInstallmentDate
                //}).ToList()

                Lines = (model.Lines ?? new List<SalesOrderProductSelectionLinePostViewModel>())
                .Select(x => new SaveSalesOrderDraftLineDto
                {
                    ProductId = x.ProductId,
                    ProductVariantId = x.ProductVariantId,
                    ProviderProductId = x.ProviderProductId,
                    Quantity = x.Quantity,
                    SalePrice = x.SalePrice,

                    InstallmentApplicable = x.InstallmentApplicable,
                    DownPaymentAmount = x.DownPaymentAmount,
                    NoOfInstallment = x.NoOfInstallment,
                    MonthlyInstallmentAmount = x.MonthlyInstallmentAmount,

                    FirstInstallmentDate = x.FirstInstallmentDate
                })
                .ToList()


            }, cancellationToken);
        }

        private Guid GetCurrentUserId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(raw, out var id) ? id : Guid.Empty;
        }


        [HttpGet]
        public async Task<IActionResult> SalesOrderProductList(
        Guid? draftId,
        CancellationToken cancellationToken = default)
        {
            if (!draftId.HasValue)
            {
                TempData.Remove("ErrorMessage");
            }

            var dto = await _salesOrderService.GetProductListForOrderAsync(cancellationToken);
            var model = MapToViewModel(dto);

            if (draftId.HasValue && draftId.Value != Guid.Empty)
            {
                var draft = await _salesOrderDraftService.GetDraftAsync(draftId.Value, cancellationToken);

                if (draft != null)
                {
                    //model.SalesOrderDraftId = draft.SalesOrderDraftId;
                    //model.DraftNo = draft.DraftNo;

                    model.SalesOrderDraftId = draft.SalesOrderDraftId;
                    model.DraftNo = draft.DraftNo;

                    model.DraftLines = draft.Lines.Select(x => new SalesOrderDraftLineViewModel
                    {
                        ProductId = x.ProductId,
                        ProductVariantId = x.ProductVariantId,
                        ProviderProductId = x.ProviderProductId,
                        Quantity = x.Quantity,
                        SalePrice = x.SalePrice,
                        
                        InstallmentApplicable = x.InstallmentApplicable,
                        IsInstallmentSelected = x.IsInstallmentSelected,
                        DownPaymentAmount = x.DownPaymentAmount,
                        NoOfInstallment = x.NoOfInstallment,
                        MonthlyInstallmentAmount = x.MonthlyInstallmentAmount,
                        FirstInstallmentDate = x.FirstInstallmentDate
                    }).ToList();

                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SalesOrderProductList(IFormCollection form)
        {
            // This first version preserves selected product data and moves to customer selection/creation.
            // Full order submit logic will be added in SalesOrderConfirmation phase.
            TempData["OrderProductSelectionPosted"] = "true";
            return RedirectToAction(nameof(SalesOrderCustomerCreation));
        }

        [HttpGet]
        public IActionResult SalesOrderCustomerCreation(Guid draftId)
        {
            ViewBag.DraftId = draftId;

            return View();
        }

        [HttpGet]
        public IActionResult SalesOrderConfirmation()
        {
            return View();
        }

        private static SalesOrderProductListViewModel MapToViewModel(SalesOrderProductListDto dto)
        {
            return new SalesOrderProductListViewModel
            {
                BusinessCategories = dto.BusinessCategories.Select(MapCategory).ToList(),
                ResidentialCategories = dto.ResidentialCategories.Select(MapCategory).ToList()
            };
        }

        private static SalesOrderProductCategoryViewModel MapCategory(SalesOrderProductCategoryDto x)
        {
            return new SalesOrderProductCategoryViewModel
            {
                CategoryId = x.CategoryId,
                CategoryCode = x.CategoryCode,
                CategoryName = x.CategoryName,
                CategoryImageUrl = x.CategoryImageUrl,
                DisplayOrder = x.DisplayOrder,
                Products = x.Products.Select(p => new SalesOrderProductItemViewModel
                {
                    ProductId = p.ProductId,
                    CategoryId = p.CategoryId,
                    ProductCode = p.ProductCode,
                    ProductName = p.ProductName,
                    ProductDisplayName = p.ProductDisplayName,
                    ProductType = p.ProductType,
                    BasePriceType = p.BasePriceType,
                    BasePrice = p.BasePrice,
                    IsPriceEditable = p.IsPriceEditable,
                    InstallmentApplicable = p.InstallmentApplicable,
                    DownPaymentAmount = p.DownPaymentAmount,
                    NoOfInstallment = p.NoOfInstallment,
                    MonthlyInstallmentAmount = p.MonthlyInstallmentAmount,
                    CurrencyCode = p.CurrencyCode,
                    ProductDescription = p.ProductDescription,
                    ProductDisplayNotes = p.ProductDisplayNotes,
                    PaymentNotes = p.PaymentNotes,
                    DisplayOrder = p.DisplayOrder,
                    PrimaryImageUrl = p.PrimaryImageUrl,
                    Providers = p.Providers.Select(pr => new SalesOrderProviderOptionViewModel
                    {
                        ProviderProductId = pr.ProviderProductId,
                        ProviderId = pr.ProviderId,
                        ProviderName = pr.ProviderName,
                        ProductCode = pr.ProductCode,
                        ProductName = pr.ProductName
                    }).ToList(),
                    Variants = p.Variants.Select(v => new SalesOrderProductVariantOptionViewModel
                    {
                        ProductVariantId = v.ProductVariantId,
                        VariantCode = v.VariantCode,
                        VariantTypeCode = v.VariantTypeCode,
                        VariantTypeName = v.VariantTypeName,
                        VariantName = v.VariantName,
                        DisplayStyle = v.DisplayStyle,
                        BasePrice = v.BasePrice,
                        DisplayOrder = v.DisplayOrder
                    }).ToList()
                }).ToList()
            };
        }
    }
}
