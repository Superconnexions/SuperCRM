using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuperCRM.Domain.Entities;
using SuperCRM.Persistence.DbContexts;
using SuperCRM.Persistence.Identity;
using SuperCRM.Shared;
using SuperCRM.Web.ViewModels.Providers;
using System.Security.Claims;

namespace SuperCRM.Web.Controllers;

[Authorize(Roles = AppRoles.SuperAdmin)]
public class ProvidersController : Controller
{
    private readonly SuperCrmDbContext _dbContext;

    public ProvidersController(SuperCrmDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _dbContext.Providers
            .AsNoTracking()
            .OrderBy(x => x.ProviderName)
            .Select(x => new ProviderListItemViewModel
            {
                ProviderId = x.ProviderId,
                ProviderName = x.ProviderName,
                ContactEmail = x.ContactEmail,
                ContactPhone = x.ContactPhone,
                ProviderUrl = x.ProviderUrl,
                ProviderAddress = x.ProviderAddress,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return View(items);
    }

    public IActionResult Create()
    {
        return View(new ProviderCreateEditViewModel
        {
            IsActive = true
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProviderCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var entity = new Provider
        {
            ProviderId = Guid.NewGuid(),
            ProviderName = model.ProviderName.Trim(),
            ContactEmail = string.IsNullOrWhiteSpace(model.ContactEmail) ? null : model.ContactEmail.Trim(),
            ContactPhone = string.IsNullOrWhiteSpace(model.ContactPhone) ? null : model.ContactPhone.Trim(),

            ProviderUrl = string.IsNullOrWhiteSpace(model.ProviderUrl) ? null : model.ProviderUrl.Trim(),
            ProviderAddress = string.IsNullOrWhiteSpace(model.ProviderAddress) ? null : model.ProviderAddress.Trim(),

            IsActive = model.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = null,
            UpdatedByUserId = string.IsNullOrWhiteSpace(userId) ? null : Guid.Parse(userId)
        };

        _dbContext.Providers.Add(entity);
        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Provider created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(Guid id)
    {
        var entity = await _dbContext.Providers.FindAsync(id);
        if (entity == null)
            return NotFound();

        var model = new ProviderCreateEditViewModel
        {
            ProviderId = entity.ProviderId,
            ProviderName = entity.ProviderName,
            ContactEmail = entity.ContactEmail,
            ContactPhone = entity.ContactPhone,
            ProviderUrl = entity.ProviderUrl,
            ProviderAddress = entity.ProviderAddress,
            IsActive = entity.IsActive
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ProviderCreateEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.ProviderId == null)
            return BadRequest();

        var entity = await _dbContext.Providers.FindAsync(model.ProviderId.Value);
        if (entity == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        entity.ProviderName = model.ProviderName.Trim();
        entity.ContactEmail = string.IsNullOrWhiteSpace(model.ContactEmail) ? null : model.ContactEmail.Trim();
        entity.ContactPhone = string.IsNullOrWhiteSpace(model.ContactPhone) ? null : model.ContactPhone.Trim();


        entity.ProviderUrl = string.IsNullOrWhiteSpace(model.ProviderUrl) ? null : model.ProviderUrl.Trim();
        entity.ProviderAddress = string.IsNullOrWhiteSpace(model.ProviderAddress) ? null : model.ProviderAddress.Trim();

        entity.IsActive = model.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedByUserId = string.IsNullOrWhiteSpace(userId) ? null : Guid.Parse(userId);

        await _dbContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Provider updated successfully.";
        return RedirectToAction(nameof(Index));
    }
}