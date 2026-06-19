using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Core.Types;
using SuperCRM.Application.DTOs.SalesOrders;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Domain.Enums;
using SuperCRM.Web.Helpers;
using SuperCRM.Web.ViewModels.SalesOrders;
using System.Security.Claims;

namespace SuperCRM.Web.Controllers
{
    [Authorize]
    public class SalesOrderController : Controller
    {
        private readonly ISalesOrderService _salesOrderService;
        private readonly ISalesOrderDraftService _salesOrderDraftService;
        private readonly ISalesOrderCustomerService _salesOrderCustomerService;
        private readonly ISalesOrderCreationService _salesOrderCreationService;
        private readonly IEmailHelper _emailHelper;
        private readonly IAppSettingsHelper _appSettingsHelper;
        private readonly ILogger<SalesOrderController> _logger;

        public SalesOrderController(
            ISalesOrderService salesOrderService,
            ISalesOrderDraftService salesOrderDraftService,
            ISalesOrderCustomerService salesOrderCustomerService,
            ISalesOrderCreationService salesOrderCreationService,
            IEmailHelper emailHelper,
            IAppSettingsHelper appSettingsHelper, 
            ILogger<SalesOrderController> logger)
        {
            _salesOrderService = salesOrderService;
            _salesOrderDraftService = salesOrderDraftService;
            _salesOrderCustomerService = salesOrderCustomerService;
            _salesOrderCreationService = salesOrderCreationService;
            _emailHelper = emailHelper;
            _appSettingsHelper = appSettingsHelper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerForSalesOrder(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var dto = await _salesOrderCustomerService.GetCustomerForSalesOrderAsync(customerId, cancellationToken);
            if (dto == null || !dto.Success)
            {
                return Json(new
                {
                    success = false,
                    message = dto?.Message ?? "Customer was not found."
                });
            }

            return Json(dto);
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


                Lines = (model.Lines ?? new List<SalesOrderProductSelectionLinePostViewModel>())
                .Select(x => new SaveSalesOrderDraftLineDto
                {
                    ProductId = x.ProductId,
                    ProductVariantId = x.ProductVariantId,
                    ProviderProductId = x.ProviderProductId,
                    Quantity = x.Quantity,
                    SalePrice = x.SalePrice,
                    IsInstallmentSelected = x.IsInstallmentSelected,
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

            TempData.Remove("SuccessMessage");
            TempData.Remove("ErrorMessage");

            var currentUserId = GetCurrentUserId();

            if (User.IsInRole("Agent"))
            {
                var canCreateOrder = await _salesOrderCreationService.CanCreateSalesOrderAsync(
                    currentUserId,
                    isAgent: true,
                    cancellationToken);

                if (!canCreateOrder)
                {
                    //TempData["ErrorMessage"] =
                    //    "Your agent account is not approved yet. Please wait for approval before creating a sales order.";

                    return RedirectToAction("Index", "AgentDashboard");
                }
            }

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

        //[HttpGet]
        //public IActionResult SalesOrderCustomerCreation(Guid draftId)
        //{
        //    ViewBag.DraftId = draftId;

        //    return View();
        //}

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
                    ProviderLinks = p.ProviderLinks.Select(link => new SalesOrderProviderLinkDto
                    {
                        ProviderId = link.ProviderId,
                        ProviderName = link.ProviderName,
                        WebsiteUrl = link.WebsiteUrl
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


        // Start Customer Creation and Sales Order

        [HttpGet]
        public async Task<IActionResult> SalesOrderCustomerCreation(Guid draftId, CancellationToken cancellationToken = default)
        {
            if (draftId == Guid.Empty)
            {
                TempData["ErrorMessage"] = "Draft was not found.";
                return RedirectToAction(nameof(SalesOrderProductList));
            }

            var dto = await _salesOrderCustomerService.GetCustomerCreationPageAsync(draftId, cancellationToken);
            if (dto == null)
            {
                TempData["ErrorMessage"] = "Draft was not found.";
                return RedirectToAction(nameof(SalesOrderProductList));
            }

            var model = MapCustomerCreationViewModel(dto);

            var ukCountry = model.CountryOptions
                            .FirstOrDefault(x =>
                                x.Text.Equals("United Kingdom", StringComparison.OrdinalIgnoreCase)
                                || x.Text.Equals("UK", StringComparison.OrdinalIgnoreCase));

            if (ukCountry != null
                && int.TryParse(ukCountry.Value, out var ukCountryId))
            {
                if (!model.PersonalAddress.CountryId.HasValue)
                {
                    model.PersonalAddress.CountryId = ukCountryId;
                }

                if (!model.BusinessAddress.CountryId.HasValue)
                {
                    model.BusinessAddress.CountryId = ukCountryId;
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SearchCustomers(string keyword, CancellationToken cancellationToken = default)
        {
            var items = await _salesOrderCustomerService.SearchCustomersAsync(keyword ?? string.Empty, cancellationToken);
            return Json(items);
        }

        [HttpGet]
        public async Task<IActionResult> GetMyCustomers( CancellationToken cancellationToken = default)
        {
            var currentUserId = GetCurrentUserId();

            if (currentUserId == Guid.Empty)
            {
                return Json(new List<CustomerSearchResultDto>());
            }

            var items = await _salesOrderCustomerService
                .GetCustomersCreatedByUserAsync(
                    currentUserId,
                    cancellationToken);

            return Json(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCustomerAndPreview(SalesOrderCustomerCreationViewModel model, CancellationToken cancellationToken = default)
        {
            var result = await SaveCustomerInternalAsync(model, cancellationToken);

            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;

            return RedirectToAction(nameof(SalesOrderCustomerCreation),
                new { draftId = model.SalesOrderDraftId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveCustomer( SalesOrderCustomerCreationViewModel model, CancellationToken cancellationToken = default)
        {
            var result = await SaveCustomerInternalAsync(model, cancellationToken);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(SalesOrderCustomerCreation),
                    new { draftId = model.SalesOrderDraftId });
            }


            //if (!result.Success)
            //{
            //    ModelState.AddModelError("", result.Message);

            //    var dto = await _salesOrderCustomerService.GetCustomerCreationPageAsync(
            //        model.SalesOrderDraftId,
            //        cancellationToken);

            //    var vm = MapCustomerCreationViewModel(dto);

            //    // Preserve user entered values
            //    vm.Customer = model.Customer;
            //    vm.PersonalAddress = model.PersonalAddress;
            //    vm.Business = model.Business;
            //    vm.BusinessAddress = model.BusinessAddress;
            //    vm.BankAccount = model.BankAccount;

            //    vm.BusinessType = model.BusinessType;
            //    vm.IsBusinessAddressSameAsPersonal = model.IsBusinessAddressSameAsPersonal;
            //    vm.ExistingCustomerId = model.ExistingCustomerId;

            //    return View(nameof(SalesOrderCustomerCreation), vm);
            //}


            TempData["SuccessMessage"] = result.Message;
            return RedirectToAction(nameof(SalesOrderCustomerCreation),
                new { draftId = model.SalesOrderDraftId });
        }

        private async Task<SalesOrderCustomerSaveResultDto> SaveCustomerInternalAsync(
            SalesOrderCustomerCreationViewModel model,
            CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return new SalesOrderCustomerSaveResultDto
                {
                    Success = false,
                    Message = "Invalid login session.",
                    SalesOrderDraftId = model.SalesOrderDraftId
                };
            }

            return await _salesOrderCustomerService.SaveCustomerAsync(new SaveSalesOrderCustomerDto
            {
                SalesOrderDraftId = model.SalesOrderDraftId,
                CurrentUserId = userId,
                ExistingCustomerId = model.ExistingCustomerId,
                BusinessType = model.BusinessType,
                IsBusinessFlow = model.IsBusinessFlow,
                RequiresBankInformation = model.RequiresBankInformation,
                IsBusinessAddressSameAsPersonal = model.IsBusinessAddressSameAsPersonal,
                FirstName = model.Customer.FirstName,
                LastName = model.Customer.LastName,
                DisplayName = model.Customer.DisplayName,
                Email = model.Customer.Email,
                AlternativeEmail = model.Customer.AlternativeEmail,
                Phone = model.Customer.Phone,
                Mobile = model.Customer.Mobile,
                
                RegistrationSource = GetRegistrationSource(),

                //PersonalAddress = model.ShowPersonalAddress
                //    ? new SaveSalesOrderAddressDto
                //    {
                //        HouseNo = model.PersonalAddress.HouseNo,
                //        RoadName = model.PersonalAddress.RoadName,
                //        PostCode = model.PersonalAddress.PostCode,
                //        City = model.PersonalAddress.City,
                //        CountryId = model.PersonalAddress.CountryId,
                //        RegionId = model.PersonalAddress.RegionId,
                //        AddressLine = model.PersonalAddress.AddressLine
                //    }
                //    : null,

                PersonalAddress = new SaveSalesOrderAddressDto
                {
                    HouseNo = model.PersonalAddress.HouseNo,
                    RoadName = model.PersonalAddress.RoadName,
                    PostCode = model.PersonalAddress.PostCode,
                    City = model.PersonalAddress.City,
                    CityId = model.PersonalAddress.CityId,
                    CountryId = model.PersonalAddress.CountryId,
                    RegionId = model.PersonalAddress.RegionId,
                    AddressLine = model.PersonalAddress.AddressLine
                },

                //PersonalAddress = new SaveSalesOrderAddressDto
                //{
                //    HouseNo = model.PersonalAddress.HouseNo,
                //    RoadName = model.PersonalAddress.RoadName,
                //    PostCode = model.PersonalAddress.PostCode,
                //    City = model.PersonalAddress.City,
                //    CountryId = model.PersonalAddress.CountryId,
                //    RegionId = model.PersonalAddress.RegionId,
                //    AddressLine = model.PersonalAddress.AddressLine
                //},
                Business = new SaveSalesOrderBusinessDto
                {
                    BusinessName = model.Business.BusinessName,
                    BusinessEmail = model.Business.BusinessEmail,
                    TradingName = model.Business.TradingName,
                    RegistrationNo = model.Business.RegistrationNo,
                    ContactPersonName = model.Business.ContactPersonName,
                    ContactPersonPhone = model.Business.ContactPersonPhone
                },
                BusinessAddress = new SaveSalesOrderAddressDto
                {
                    HouseNo = model.BusinessAddress.HouseNo,
                    RoadName = model.BusinessAddress.RoadName,
                    PostCode = model.BusinessAddress.PostCode,
                    City = model.BusinessAddress.City,
                    CityId = model.BusinessAddress.CityId,
                    CountryId = model.BusinessAddress.CountryId,
                    RegionId = model.BusinessAddress.RegionId,
                    AddressLine = model.BusinessAddress.AddressLine
                },
                BankAccount = new SaveSalesOrderBankAccountDto
                {
                    SelectedCustomerBankAccountId = model.SelectedCustomerBankAccountId,
                    BankName = model.BankAccount.BankName,
                    AccountName = model.BankAccount.AccountName,
                    AccountNumber = model.BankAccount.AccountNumber,
                    SortCode = model.BankAccount.SortCode
                }
            }, cancellationToken);
        }

        // Start Update Customer
        private async Task<SalesOrderCustomerSaveResultDto> UpdateCustomerInternalAsync(
        SalesOrderCustomerCreationViewModel model,
        CancellationToken cancellationToken)
        {
            var userId = GetCurrentUserId();

            if (userId == Guid.Empty)
            {
                return new SalesOrderCustomerSaveResultDto
                {
                    Success = false,
                    Message = "Invalid login session.",
                    SalesOrderDraftId = model.SalesOrderDraftId
                };
            }

            return await _salesOrderCustomerService.UpdateCustomerAsync(new SaveSalesOrderCustomerDto
            {
                SalesOrderDraftId = model.SalesOrderDraftId,
                CurrentUserId = userId,
                ExistingCustomerId = model.ExistingCustomerId,

                BusinessType = model.BusinessType,
                IsBusinessFlow = model.IsBusinessFlow,
                RequiresBankInformation = model.RequiresBankInformation,
                IsBusinessAddressSameAsPersonal = model.IsBusinessAddressSameAsPersonal,

                FirstName = model.Customer.FirstName,
                LastName = model.Customer.LastName,
                DisplayName = model.Customer.DisplayName,
                Email = model.Customer.Email,
                AlternativeEmail = model.Customer.AlternativeEmail,
                Phone = model.Customer.Phone,
                Mobile = model.Customer.Mobile,
                RegistrationSource = GetRegistrationSource(),

                PersonalAddress = new SaveSalesOrderAddressDto
                {
                    HouseNo = model.PersonalAddress.HouseNo,
                    RoadName = model.PersonalAddress.RoadName,
                    PostCode = model.PersonalAddress.PostCode,
                    City = model.PersonalAddress.City,
                    CityId = model.PersonalAddress.CityId,
                    CountryId = model.PersonalAddress.CountryId,
                    RegionId = model.PersonalAddress.RegionId,
                    AddressLine = model.PersonalAddress.AddressLine
                },

                Business = new SaveSalesOrderBusinessDto
                {
                    BusinessName = model.Business.BusinessName,
                    BusinessEmail = model.Business.BusinessEmail,
                    TradingName = model.Business.TradingName,
                    RegistrationNo = model.Business.RegistrationNo,
                    ContactPersonName = model.Business.ContactPersonName,
                    ContactPersonPhone = model.Business.ContactPersonPhone
                },

                BusinessAddress = new SaveSalesOrderAddressDto
                {
                    HouseNo = model.BusinessAddress.HouseNo,
                    RoadName = model.BusinessAddress.RoadName,
                    PostCode = model.BusinessAddress.PostCode,
                    City = model.BusinessAddress.City,
                    CityId = model.BusinessAddress.CityId,
                    CountryId = model.BusinessAddress.CountryId,
                    RegionId = model.BusinessAddress.RegionId,
                    AddressLine = model.BusinessAddress.AddressLine
                },

                BankAccount = new SaveSalesOrderBankAccountDto
                {
                    BankName = model.BankAccount.BankName,
                    AccountName = model.BankAccount.AccountName,
                    AccountNumber = model.BankAccount.AccountNumber,
                    SortCode = model.BankAccount.SortCode
                }
            }, cancellationToken);
        }

        // END Update Customer

        [HttpGet]
        public IActionResult SalesOrderPreview(Guid draftId)
        {
            ViewBag.DraftId = draftId;
            return View(); // Step-2 package will replace this placeholder.
        }

        private static SalesOrderCustomerCreationViewModel MapCustomerCreationViewModel(SalesOrderCustomerCreationPageDto dto)
        {
            var model = new SalesOrderCustomerCreationViewModel
            {
                SalesOrderDraftId = dto.SalesOrderDraftId,
                DraftNo = dto.DraftNo,
                HasResidentialProduct = dto.Requirement.HasResidentialProduct,
                HasBusinessProduct = dto.Requirement.HasBusinessProduct,
                HasMixedBusinessResidential = dto.Requirement.HasMixedBusinessResidential,
                ShowPersonalAddress = dto.Requirement.IsResidentialOnly || dto.Requirement.HasMixedBusinessResidential,
                ShowBusinessAddressSameCheckbox = dto.Requirement.HasMixedBusinessResidential,
                IsResidentialOnly = dto.Requirement.IsResidentialOnly,
                IsBusinessFlow = dto.Requirement.IsBusinessFlow,
                RequiresBankInformation = dto.Requirement.RequiresBankInformation,
                ScenarioName = dto.Requirement.ScenarioName,
                ExistingCustomerId = dto.SelectedCustomerId,
                SelectedCustomerBankAccountId = dto.SelectedCustomerBankAccountId,
                IsCustomerSavedForOrder = dto.SelectedCustomerId.HasValue,
                BusinessType = (byte)dto.BusinessType,
                CountryOptions = dto.Countries.Select(x => new SelectListItem { Value = x.Value, Text = x.Text }).ToList(),
                RegionOptions = dto.Regions.Select(x => new SelectListItem { Value = x.Value, Text = x.Text }).ToList(),
                Products = dto.Products.Select(x => new SalesOrderSelectedProductSummaryViewModel
                {
                    ProductName = x.ProductName,
                    VariantName = x.VariantName,
                    ProviderName = x.ProviderName,
                    Quantity = x.Quantity,
                    SalePrice = x.SalePrice,
                    LineTotalAmount = x.LineTotalAmount,
                    CurrencyCode = x.CurrencyCode,
                    IsInstallmentSelected = x.IsInstallmentSelected,
                    SalesUnitCode = x.SalesUnitCode,
                    InstallmentSummary = x.InstallmentApplicable
                    ? (x.IsInstallmentSelected
                        ? $"Installment: Down {x.DownPaymentAmount:0.00}, {x.NoOfInstallment} x {x.MonthlyInstallmentAmount:0.00} monthly"
                        : "One-off")
                    : ""
                }).ToList()
            };

            // Added for Load Page with Existing Customer

            if (dto.Customer != null)
            {
                model.Customer.FirstName = dto.Customer.FirstName;
                model.Customer.LastName = dto.Customer.LastName;
                model.Customer.DisplayName = dto.Customer.DisplayName;
                model.Customer.Email = dto.Customer.Email;
                model.Customer.AlternativeEmail = dto.Customer.AlternativeEmail;
                model.Customer.Phone = dto.Customer.Phone;
                model.Customer.Mobile = dto.Customer.Mobile;
            }

            if (dto.PersonalAddress != null)
            {
                model.PersonalAddress.HouseNo = dto.PersonalAddress.HouseNo;
                model.PersonalAddress.RoadName = dto.PersonalAddress.RoadName;
                model.PersonalAddress.PostCode = dto.PersonalAddress.PostCode;
                model.PersonalAddress.City = dto.PersonalAddress.City;
                model.PersonalAddress.CityId = dto.PersonalAddress.CityId;
                model.PersonalAddress.CountryId = dto.PersonalAddress.CountryId;
                model.PersonalAddress.RegionId = dto.PersonalAddress.RegionId;
                model.PersonalAddress.AddressLine = dto.PersonalAddress.AddressLine;
            }

            if (dto.Business != null)
            {
                model.Business.BusinessName = dto.Business.BusinessName;
                model.Business.BusinessEmail = dto.Business.BusinessEmail;
                model.Business.TradingName = dto.Business.TradingName;
                model.Business.RegistrationNo = dto.Business.RegistrationNo;
                model.Business.ContactPersonName = dto.Business.ContactPersonName;
                model.Business.ContactPersonPhone = dto.Business.ContactPersonPhone;
            }

            if (dto.BusinessAddress != null)
            {
                model.BusinessAddress.HouseNo = dto.BusinessAddress.HouseNo;
                model.BusinessAddress.RoadName = dto.BusinessAddress.RoadName;
                model.BusinessAddress.PostCode = dto.BusinessAddress.PostCode;
                model.BusinessAddress.City = dto.BusinessAddress.City;
                model.BusinessAddress.CityId = dto.BusinessAddress.CityId;
                model.BusinessAddress.CountryId = dto.BusinessAddress.CountryId;
                model.BusinessAddress.RegionId = dto.BusinessAddress.RegionId;
                model.BusinessAddress.AddressLine = dto.BusinessAddress.AddressLine;
            }

            if (dto.BankAccount != null)
            {
                model.BankAccount.BankName = dto.BankAccount.BankName;
                model.BankAccount.AccountName = dto.BankAccount.AccountName;
                model.BankAccount.AccountNumber = dto.BankAccount.AccountNumber;
                model.BankAccount.SortCode = dto.BankAccount.SortCode;
            }
            // END

            model.PersonalAddress.CountryOptions = model.CountryOptions;
            model.PersonalAddress.RegionOptions = model.RegionOptions;

            model.BusinessAddress.CountryOptions = model.CountryOptions;
            model.BusinessAddress.RegionOptions = model.RegionOptions;

            model.PersonalAddress.CityOptions = new List<SelectListItem>();
            model.BusinessAddress.CityOptions = new List<SelectListItem>();

            return model;


        }
        // END Customer Creation


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSalesOrder(Guid draftId, CancellationToken cancellationToken = default)
        {

            TempData.Remove("SuccessMessage");
            TempData.Remove("ErrorMessage");

            var userId = GetCurrentUserId();

            var result = await _salesOrderCreationService.CreateSalesOrderFromDraftAsync(
                new CreateSalesOrderFromDraftRequestDto
                {
                    SalesOrderDraftId = draftId,
                    CurrentUserId = userId,
                    OrderSourceType = GetOrderSourceType()
                },
                cancellationToken);

            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message;
                return RedirectToAction(nameof(SalesOrderCustomerCreation), new { draftId });
            }

            var saleIds = string.Join(",", result.SaleIds);

            var emailResult = await SendCustomerSalesOrderEmailAsync( result.SaleIds, userId, cancellationToken);


            //TempData["SuccessMessage"] = result.Message;
            TempData["SuccessMessage"] = emailResult? result.Message + " Customer email also sent successfully." : result.Message + " However, customer email could not be sent.";

            return RedirectToAction(nameof(SalesOrderCreatedSummary), new { saleIds });
        }

        [HttpGet]
        public async Task<IActionResult> SalesOrderCreatedSummary(string saleIds, CancellationToken cancellationToken = default)
        {
            var ids = (saleIds ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(x => Guid.TryParse(x, out var id) ? id : Guid.Empty)
                .Where(x => x != Guid.Empty)
                .ToList();

            var dto = await _salesOrderCreationService.GetCreatedSalesOrderSummaryAsync(ids, cancellationToken);
            if (dto == null)
            {
                TempData["ErrorMessage"] = "Created sales order was not found.";
                return RedirectToAction(nameof(SalesOrderProductList));
            }

            return View(MapSalesOrderCreatedSummaryViewModel(dto));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendSalesOrderEmail(Guid saleId)
        {
            // Demo placeholder. Later this action can call EmailSettings module and email Provider.ContactEmail.
            TempData["SuccessMessage"] = "Email sending placeholder executed. Provider email integration will be added next.";
            return RedirectToAction(nameof(SalesOrderCreatedSummary), new { saleIds = saleId.ToString() });
        }

        private static SalesOrderCreatedSummaryViewModel MapSalesOrderCreatedSummaryViewModel(SalesOrderCreatedSummaryDto dto)
        {
            return new SalesOrderCreatedSummaryViewModel
            {
                Customer = dto.Customer,
                Business = dto.Business,
                HomeAddress = dto.HomeAddress,
                BusinessAddress = dto.BusinessAddress,
                BankAccount = dto.BankAccount,
                Orders = dto.Orders
            };
        }

        [HttpGet]
        public async Task<IActionResult> GetAnyRegionByCountry(int countryId, CancellationToken cancellationToken)
        {
            var regionId = await _salesOrderCustomerService.GetAnyRegionIdByCountryIdAsync(countryId, cancellationToken);
            return Json(new { regionId });
        }

        [HttpGet]
        public async Task<IActionResult> GetCitiesByRegion(int regionId, CancellationToken cancellationToken)
        {
            var cities = await _salesOrderCustomerService.GetCityOptionsByRegionIdAsync(regionId, cancellationToken);

            return Json(cities.Select(x => new
            {
                id = x.Value,
                name = x.Text
            }));
        }

        [HttpGet]
        public async Task<IActionResult> CheckCustomerDuplicate(
        string? email,
        string? mobile,
        Guid? excludeCustomerId,
        CancellationToken cancellationToken = default)
        {
            var result = await _salesOrderCustomerService.CheckCustomerDuplicateAsync(
                email,
                mobile,
                excludeCustomerId,
                cancellationToken);

            return Json(new
            {
                emailExists = result.EmailExists,
                mobileExists = result.MobileExists
            });
        }

        private RegistrationSource GetRegistrationSource()
        {
            if (User.IsInRole("SuperAdmin") ||
                User.IsInRole("SuperCRMAdmin"))
            {
                return RegistrationSource.AdminCreated;
            }

            if (User.IsInRole("Agent"))
            {
                return RegistrationSource.AgentCreated;
            }

            return RegistrationSource.SelfRegistration;
        }

        private OrderSourceType GetOrderSourceType()
        {
            if (User.IsInRole("SuperAdmin") ||
                User.IsInRole("SuperCRMAdmin"))
            {
                return OrderSourceType.Admin;
            }

            if (User.IsInRole("Agent"))
            {
                return OrderSourceType.Agent;
            }

            return OrderSourceType.Unknown;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCustomer( SalesOrderCustomerCreationViewModel model, CancellationToken cancellationToken = default)
        {
            var result = await UpdateCustomerInternalAsync(model, cancellationToken);

            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] = result.Message;

            return RedirectToAction(nameof(SalesOrderCustomerCreation),
                new { draftId = model.SalesOrderDraftId });
        }

        [HttpGet]
        public async Task<IActionResult> CheckCustomerDuplicateForOrder(
        string? email,
        string? mobile,
        
        string? accountNumber,
        Guid? excludeCustomerId,
        Guid? excludeBankAccountId,
        CancellationToken cancellationToken = default)
        {
            var result = await _salesOrderCustomerService.CheckCustomerDuplicateForOrderAsync(
                email,
                mobile,
                
                accountNumber,
                excludeCustomerId,
                excludeBankAccountId,
                cancellationToken);

            return Json(new
            {
                emailExists = result.EmailExists,
                mobileExists = result.MobileExists,
                bankAccountExists = result.BankAccountExists
            });
        }

        // END


        // Start Agents Activities

        [HttpGet]
        public async Task<IActionResult> SalesOrderHistory(
        DateTime? orderDateFrom,
        DateTime? orderDateTo,
        byte? salesOrderStatus,
        string? salesOrderNo,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
        {
            var currentUserId = GetCurrentUserId();

            Guid? soldByUserId = null;
            var isAgentView = false;

            if (User.IsInRole("Agent"))
            {
                soldByUserId = currentUserId;
                isAgentView = true;
            }

            if (!orderDateFrom.HasValue && !orderDateTo.HasValue)
            {
                orderDateFrom = DateTime.UtcNow.Date.AddDays(-30);
                orderDateTo = DateTime.UtcNow.Date;
            }

            var result = await _salesOrderCreationService.GetSalesOrderHistoryAsync(
                soldByUserId,
                orderDateFrom,
                orderDateTo,
                salesOrderStatus,
                salesOrderNo,
                page,
                pageSize,
                cancellationToken);

            var model = new SalesOrderHistoryViewModel
            {
                IsAgentView = isAgentView,
                OrderDateFrom = orderDateFrom,
                OrderDateTo = orderDateTo,
                SalesOrderStatus = salesOrderStatus,
                SalesOrderNo = salesOrderNo,
                Orders = result.Items,
                TotalRecords = result.TotalRecords,
                Page = page,
                PageSize = pageSize,
                StatusOptions = Enum.GetValues(typeof(SalesOrderStatus))
                    .Cast<SalesOrderStatus>()
                    .Where(x => x != SalesOrderStatus.Unknown)
                    .Select(x => new SelectListItem
                    {
                        Value = ((byte)x).ToString(),
                        Text = x.ToString()
                    })
                    .ToList()
            };

            return View(model);
        }

        // END Agents Activities

        // Start Sales Management

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SuperCRMAdmin")]
        public async Task<IActionResult> SalesOrderManagement(
        DateTime? orderDateFrom,
        DateTime? orderDateTo,
        byte? salesOrderStatus,
        string? salesOrderNo,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
        {

            if (!string.IsNullOrWhiteSpace(salesOrderNo))
            {
                salesOrderNo = salesOrderNo.Trim();
            }

            if (!orderDateFrom.HasValue && !orderDateTo.HasValue)
            {
                orderDateFrom = DateTime.UtcNow.Date.AddDays(-30);
                orderDateTo = DateTime.UtcNow.Date;
            }


            var result = await _salesOrderCreationService.GetSalesOrderHistoryAsync(
                soldByUserId: null,
                orderDateFrom,
                orderDateTo,
                salesOrderStatus,
                salesOrderNo,
                page,
                pageSize,
                cancellationToken);

            var model = new SalesOrderHistoryViewModel
            {
                IsAgentView = false,
                OrderDateFrom = orderDateFrom,
                OrderDateTo = orderDateTo,
                SalesOrderStatus = salesOrderStatus,
                SalesOrderNo = salesOrderNo,
                Orders = result.Items,
                TotalRecords = result.TotalRecords,
                Page = page,
                PageSize = pageSize,
                StatusOptions = Enum.GetValues(typeof(SalesOrderStatus))
                    .Cast<SalesOrderStatus>()
                    .Where(x => x != SalesOrderStatus.Unknown)
                    .Select(x => new SelectListItem
                    {
                        Value = ((byte)x).ToString(),
                        Text = x.ToString()
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesOrderManagementDetail(
        Guid saleId,
        CancellationToken cancellationToken = default)
        {

            TempData.Remove("SuccessMessage");
            TempData.Remove("ErrorMessage");

            var dto = await _salesOrderCreationService.GetSalesOrderManagementDetailAsync(
                saleId,
                cancellationToken);

            if (dto == null)
            {
                return Json(new
                {
                    success = false,
                    message = "Sales order was not found."
                });
            }

            return Json(new
            {
                success = true,
                data = dto
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSalesInformation(
            UpdateSalesInformationDto model,
            CancellationToken cancellationToken = default)
        {
            model.UpdatedByUserId = GetCurrentUserId();

            var success = await _salesOrderCreationService.UpdateSalesInformationAsync(
                model,
                cancellationToken);

            if (IsAjaxRequest())
            {

                if (success)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Sales information updated successfully"
                    });
                }
                else {
                    return Json(new
                    {
                        success = false,
                        message = "Update failed."
                    });
                }
                    
            }


            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Sales information updated successfully." : "Sales order was not found.";

            return RedirectToAction(nameof(SalesOrderManagement));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSalesCommission(
            UpdateSalesCommissionDto model,
            CancellationToken cancellationToken = default)
        {
            model.UpdatedByUserId = GetCurrentUserId();

            var success = await _salesOrderCreationService.UpdateSalesCommissionAsync(
                model,
                cancellationToken);

            if (IsAjaxRequest())
            {

                if (success)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Agent commission updated successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Update failed."
                    });
                }

            }



            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Agent commission updated successfully." : "Sales order was not found.";

            return RedirectToAction(nameof(SalesOrderManagement));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSalesOrderStatus(
            UpdateSalesOrderStatusDto model,
            CancellationToken cancellationToken = default)
        {
            model.UpdatedByUserId = GetCurrentUserId();

            var success = await _salesOrderCreationService.UpdateSalesOrderStatusAsync(
                model,
                cancellationToken);

            if (IsAjaxRequest())
            {

                if (success)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Sales order status updated successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Update failed."
                    });
                }

            }


            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success ? "Sales order status updated successfully." : "Sales order was not found.";

            return RedirectToAction(nameof(SalesOrderManagement));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin,SuperCRMAdmin")]
        public async Task<IActionResult> UpdateSuperCRMCommission(
        UpdateSuperCRMCommissionDto model,
        CancellationToken cancellationToken = default)
        {
            model.UpdatedByUserId = GetCurrentUserId();

            var success = await _salesOrderCreationService.UpdateSuperCRMCommissionAsync(
                model,
                cancellationToken);

            if (IsAjaxRequest())
            {

                if (success)
                {
                    return Json(new
                    {
                        success = true,
                        message = "SuperCRM commission updated successfully"
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Update failed."
                    });
                }

            }


            TempData[success ? "SuccessMessage" : "ErrorMessage"] =
                success
                    ? "SuperCRM commission updated successfully."
                    : "Sales order was not found.";

            return RedirectToAction(nameof(SalesOrderManagement));
        }


        // Customer Managment
        [HttpGet]
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,SuperCRMAdmin,Agent")]
        public async Task<IActionResult> CustomerManagement(
        CustomerManagementViewModel model,
        CancellationToken cancellationToken = default)
        {
            var currentUserId = GetCurrentUserId();

            Guid? createdByUserId = null;
            var isAgentView = false;

            if (User.IsInRole("Agent"))
            {
                createdByUserId = currentUserId;
                isAgentView = true;
            }

            model.Page = model.Page <= 0 ? 1 : model.Page;
            model.PageSize = model.PageSize <= 0 ? 20 : model.PageSize;

            if (string.IsNullOrWhiteSpace(model.EmailOrMobile))
            {
                if (!model.CreatedDateFrom.HasValue &&
                    !model.CreatedDateTo.HasValue)
                {
                    model.CreatedDateFrom = DateTime.UtcNow.Date.AddDays(-30);
                    model.CreatedDateTo = DateTime.UtcNow.Date;
                }
            }

            var result = await _salesOrderCustomerService.GetCustomerManagementListAsync(
                createdByUserId,
                model.CreatedDateFrom,
                model.CreatedDateTo,
                model.CustomerCode,
                model.EmailOrMobile,
                model.Page,
                model.PageSize,
                cancellationToken);

            model.IsAgentView = isAgentView;
            model.Customers = result.Items;
            model.TotalRecords = result.TotalRecords;

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SuperCRMAdmin,Agent")]
        public async Task<IActionResult> CustomerSalesOrders(
            Guid customerId,
            string customerName = "",
            CancellationToken cancellationToken = default)
        {
            var orders = await _salesOrderCustomerService.GetCustomerSalesOrdersAsync(
                customerId,
                cancellationToken);

            var model = new CustomerSalesOrdersViewModel
            {
                CustomerId = customerId,
                CustomerName = customerName,
                Orders = orders
            };

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SuperCRMAdmin,Agent")]
        public async Task<IActionResult> GetCustomerAddresses(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var items = await _salesOrderCustomerService.GetCustomerAddressesAsync(
                customerId,
                cancellationToken);

            return Json(items);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SuperCRMAdmin,Agent")]
        public async Task<IActionResult> GetCustomerBusiness(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var item = await _salesOrderCustomerService.GetCustomerBusinessViewAsync(
                customerId,
                cancellationToken);

            return Json(item);
        }

        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SuperCRMAdmin,Agent")]
        public async Task<IActionResult> GetCustomerBankAccounts(
            Guid customerId,
            CancellationToken cancellationToken = default)
        {
            var items = await _salesOrderCustomerService.GetCustomerBankAccountsViewAsync(
                customerId,
                cancellationToken);

            return Json(items);
        }

        private async Task<bool> SendCustomerSalesOrderEmailAsync(
        List<Guid> saleIds,
        Guid currentUserId,
        CancellationToken cancellationToken)
        {
            if (saleIds == null || !saleIds.Any())
                return false;

            var dto = await _salesOrderCreationService.GetCreatedSalesOrderSummaryAsync(
                saleIds,
                cancellationToken);

            if (dto == null || dto.Customer == null)
                return false;

            if (string.IsNullOrWhiteSpace(dto.Customer.Email))
                return false;

            var subject = BuildCustomerSalesOrderEmailSubject(dto);
            var body = BuildCustomerSalesOrderEmailBody(dto);

            var ccEmails = _appSettingsHelper.GetSalesOrderCCEmail();
            var bccEmails = _appSettingsHelper.GetSalesOrderBCCEmail();

            try
            {
                var result = await _emailHelper.EmailSendWithCCBccAsync(
                    dto.Customer.Email,
                    ccEmails,
                    bccEmails,
                    subject,
                    body,
                    true,
                    "SalesOrder",
                    currentUserId,
                    cancellationToken);

                if (result.Success)
                {
                    await _salesOrderCreationService.MarkCustomerEmailSentAsync(
                        saleIds,
                        cancellationToken);
                }

                return result.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send Sales Order email. SaleIds: {SaleIds}",
                    string.Join(",", saleIds));

                return false;
            }

        }

        private static string BuildCustomerSalesOrderEmailSubject( SalesOrderCreatedSummaryDto dto)
        {
            var orderNos = dto.Orders == null || !dto.Orders.Any()
                ? "Sales Order"
                : string.Join(", ", dto.Orders.Select(x => x.OrderNo));

            return $"SuperCRM Sales Order Confirmation - {orderNos}";
        }

        private static string BuildCustomerSalesOrderEmailBody( SalesOrderCreatedSummaryDto dto)
        {
            var customerName = dto.Customer.DisplayName;
            var generatedAt = DateTime.UtcNow.ToString("dd-MMM-yyyy hh:mm tt");

            var html = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='utf-8' />
            </head>
            <body style='font-family: Arial, Helvetica, sans-serif; background:#f5f7fb; padding:20px; color:#1f2937;'>

                <div style='max-width:900px; margin:auto; background:#ffffff; border-radius:10px; overflow:hidden; border:1px solid #e5e7eb;'>

                    <div style='background:#1d4ed8; color:#ffffff; padding:18px 24px;'>
                        <h2 style='margin:0;'>Sales Order Confirmation</h2>
                        <p style='margin:5px 0 0 0;'>Thank you for your order with SuperCRM.</p>
                    </div>

                    <div style='padding:24px;'>

                        <p>Dear <strong>{customerName}</strong>,</p>

                        <p>
                            Your sales order has been created successfully. Please find the order details below.
                        </p>

                        <table style='width:100%; border-collapse:collapse; margin-bottom:20px;'>
                            <tr>
                                <td style='padding:6px 0;'><strong>Customer Code:</strong></td>
                                <td style='padding:6px 0;'>{dto.Customer.CustomerCode}</td>
                            </tr>
                            <tr>
                                <td style='padding:6px 0;'><strong>Email:</strong></td>
                                <td style='padding:6px 0;'>{dto.Customer.Email}</td>
                            </tr>
                            <tr>
                                <td style='padding:6px 0;'><strong>Mobile:</strong></td>
                                <td style='padding:6px 0;'>{dto.Customer.Mobile}</td>
                            </tr>
                            <tr>
                                <td style='padding:6px 0;'><strong>Generated At:</strong></td>
                                <td style='padding:6px 0;'>{generatedAt}</td>
                            </tr>
                        </table>";

                        foreach (var order in dto.Orders)
                        {
                            html += $@"
                        <div style='border:1px solid #dbeafe; border-radius:8px; margin-bottom:22px; overflow:hidden;'>

                            <div style='background:#eff6ff; padding:12px 16px; border-bottom:1px solid #dbeafe;'>
                                <strong>Order No:</strong> {order.OrderNo}
                                &nbsp; | &nbsp;
                                <strong>Provider:</strong> {order.ProviderName}
                                &nbsp; | &nbsp;
                                <strong>Order Date:</strong> {order.OrderDate:dd-MMM-yyyy}
                            </div>

                            <table style='width:100%; border-collapse:collapse;'>
                                <thead>
                                    <tr style='background:#f8fafc;'>
                                        <th style='border:1px solid #e5e7eb; padding:8px; text-align:left;'>Product</th>
                                        <th style='border:1px solid #e5e7eb; padding:8px; text-align:left;'>Variant</th>
                                        <th style='border:1px solid #e5e7eb; padding:8px; text-align:right;'>Qty</th>
                                        <th style='border:1px solid #e5e7eb; padding:8px; text-align:right;'>Unit Price</th>
                                        <th style='border:1px solid #e5e7eb; padding:8px; text-align:right;'>Total</th>
                                    </tr>
                                </thead>
                                <tbody>";

                            foreach (var line in order.Lines)
                            {
                                html += $@"
                                    <tr>
                                        <td style='border:1px solid #e5e7eb; padding:8px;'>{line.ProductName}</td>
                                        <td style='border:1px solid #e5e7eb; padding:8px;'>{line.VariantName}</td>
                                        <td style='border:1px solid #e5e7eb; padding:8px; text-align:right;'>{line.Quantity}</td>
                                        <td style='border:1px solid #e5e7eb; padding:8px; text-align:right;'>{line.UnitPrice:N2}</td>
                                        <td style='border:1px solid #e5e7eb; padding:8px; text-align:right;'>{line.LineTotalAmount:N2}</td>
                                    </tr>";
                            }

                            html += $@"
                                </tbody>
                                <tfoot>
                                    <tr>
                                        <td colspan='4' style='border:1px solid #e5e7eb; padding:8px; text-align:right;'><strong>Order Total</strong></td>
                                        <td style='border:1px solid #e5e7eb; padding:8px; text-align:right;'><strong>{order.OrderTotal:N2}</strong></td>
                                    </tr>
                                </tfoot>
                            </table>";

                            if (order.Installments != null && order.Installments.Any())
                            {
                                html += @"
                            <div style='padding:12px 16px;'>
                                <strong>Installment Schedule</strong>
                                <table style='width:100%; border-collapse:collapse; margin-top:8px;'>
                                    <thead>
                                        <tr style='background:#f8fafc;'>
                                            <th style='border:1px solid #e5e7eb; padding:8px;'>Product</th>
                                            <th style='border:1px solid #e5e7eb; padding:8px;'>Installment No</th>
                                            <th style='border:1px solid #e5e7eb; padding:8px;'>Due Date</th>
                                            <th style='border:1px solid #e5e7eb; padding:8px; text-align:right;'>Amount</th>
                                        </tr>
                                    </thead>
                                    <tbody>";

                                foreach (var ins in order.Installments)
                                {
                                    html += $@"
                                        <tr>
                                            <td style='border:1px solid #e5e7eb; padding:8px;'>{ins.ProductName}</td>
                                            <td style='border:1px solid #e5e7eb; padding:8px;'>{ins.InstallmentNo}</td>
                                            <td style='border:1px solid #e5e7eb; padding:8px;'>{ins.DueDate:dd-MMM-yyyy}</td>
                                            <td style='border:1px solid #e5e7eb; padding:8px; text-align:right;'>{ins.InstallmentAmount:N2}</td>
                                        </tr>";
                                }

                                html += @"
                                    </tbody>
                                </table>
                            </div>";
                            }

                            html += @"
                        </div>";
                        }

                        html += @"
                        <p>
                            If you have any questions regarding this order, please contact SuperCRM support.
                        </p>

                        <p style='margin-top:24px;'>
                            Regards,<br/>
                            <strong>SuperCRM Sales Team</strong>
                        </p>
                    </div>

                    <div style='background:#f8fafc; color:#64748b; padding:12px 24px; font-size:12px; text-align:center;'>
                        This is an automated email from SuperCRM. Please do not reply directly to this message.
                    </div>
                </div>

            </body>
            </html>";

            return html;
        }


        [HttpGet]
        [Authorize(Roles = "SuperAdmin,SuperCRMAdmin")]
        public async Task<IActionResult> AgentsKPI(
        DateTime? orderDateFrom,
        DateTime? orderDateTo,
        Guid? agentId,
        byte? salesOrderStatus,
        int page = 1,
        int pageSize = 20,
        CancellationToken cancellationToken = default)
        {
            var toDate = orderDateTo?.Date ?? DateTime.Today;
            var fromDate = orderDateFrom?.Date ?? toDate.AddDays(-90);

            if (fromDate > toDate)
            {
                TempData["ErrorMessage"] =
                    "Order Date From cannot be greater than Order Date To.";

                return RedirectToAction(nameof(AgentsKPI));
            }

            var dto = await _salesOrderCreationService.GetAgentsKpiAsync(
                fromDate,
                toDate,
                agentId,
                salesOrderStatus,
                page,
                pageSize,
                cancellationToken);

            var model = new AgentsKpiViewModel
            {
                OrderDateFrom = dto.OrderDateFrom,
                OrderDateTo = dto.OrderDateTo,
                AgentId = dto.AgentId,
                SalesOrderStatus = dto.SalesOrderStatus,
                Page = dto.Page,
                PageSize = dto.PageSize,
                TotalRecords = dto.TotalRecords,

                AgentOptions = dto.AgentOptions.Select(x => new SelectListItem
                {
                    Value = x.AgentId.ToString(),
                    Text = $"{x.AgentCode} - {x.FullName}"
                }).ToList(),

                StatusOptions = Enum.GetValues(typeof(SalesOrderStatus))
                    .Cast<SalesOrderStatus>()
                    .Select(x => new SelectListItem
                    {
                        Value = ((byte)x).ToString(),
                        Text = x.ToString()
                    })
                    .ToList(),

                Items = dto.Items.Select(x => new AgentsKpiRowViewModel
                {
                    AgentId = x.AgentId,
                    AgentCode = x.AgentCode,
                    FullName = x.FullName,
                    Email = x.Email,
                    Mobile = x.Mobile,
                    TotalCustomer = x.TotalCustomer,
                    TotalSalesOrder = x.TotalSalesOrder,
                    CommissionUnsettled = x.CommissionUnsettled,
                    CommissionSettled = x.CommissionSettled,
                    CommissionDistributed = x.CommissionDistributed,
                    StatusCounts = x.StatusCounts.Select(s => new AgentsKpiStatusCountViewModel
                    {
                        Status = s.Status,
                        StatusText = s.StatusText,
                        Count = s.Count
                    }).ToList()
                }).ToList()
            };

            return View(model);
        }

        private bool IsAjaxRequest()
        {
            return Request.Headers["X-Requested-With"] == "XMLHttpRequest";
        }

        // END

    }
}
