using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuperCRM.Application.Interfaces.Persistence;
using SuperCRM.Application.Interfaces.Security;
using SuperCRM.Application.Interfaces.Services;
using SuperCRM.Application.Services;
using SuperCRM.Infrastructure.Email;
using SuperCRM.Infrastructure.Security;
using SuperCRM.Infrastructure.Services;
//using SuperCRM.Infrastructure.Services;
using SuperCRM.Persistence.Dapper;
using SuperCRM.Persistence.DbContexts;
using SuperCRM.Persistence.Identity;
using SuperCRM.Persistence.Repositories;
using SuperCRM.Shared;
using SuperCRM.Web.Helpers;
//using SuperCRM.Shared;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<SuperCrmDbContext>(options =>
    options.UseSqlServer(connectionString));

//// Added for Scafolding-------
//builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
//    {
//        options.SignIn.RequireConfirmedAccount = false;
//    })
//    .AddRoles<ApplicationRole>()
//    .AddEntityFrameworkStores<SuperCrmDbContext>();

//// END addde scafolding

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddIdentity<ApplicationUser, ApplicationRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddEntityFrameworkStores<SuperCrmDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<SuperCrmDbContext>();

// Register the Dapper connection factory, Siddik, 22MAR2026
builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

// Register seed settings

// Siddik: 04APR2026, Blocked SeedAdmin registrationn since it is already done. 

//builder.Services.Configure<SeedAdminSettings>(
//    builder.Configuration.GetSection("SeedAdmin"));

// Register seed service
builder.Services.AddScoped<IdentitySeedService>();

// Start Register Services for Master Setup

// Register ProductVarintType Service
builder.Services.AddScoped<IProductVariantTypeRepository, ProductVariantTypeRepository>();
builder.Services.AddScoped<IProductVariantTypeService, ProductVariantTypeService>();
// END ProductVarintType Service

// ProductVariant Service
builder.Services.AddScoped<IProductVariantRepository, ProductVariantRepository>();
builder.Services.AddScoped<IProductVariantService, ProductVariantService>();

// Register ProductCategory Service

builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddScoped<IProductCategoryService, ProductCategoryService>();

// END ProductCategory Service

// Register SalesUnit Service
builder.Services.AddScoped<ISalesUnitRepository, SalesUnitRepository>();
builder.Services.AddScoped<ISalesUnitService, SalesUnitService>();
// END SalesUnit Service


// Register Agent Registration dependencies:
builder.Services.AddScoped<IAgentRegistrationRepository, AgentRegistrationRepository>();
builder.Services.AddScoped<IApplicationUserAccountService, ApplicationUserAccountService>();
builder.Services.AddScoped<IAgentRegistrationService, AgentRegistrationService>();
// END Registration Services

// Register Agent Management dependencies:
builder.Services.AddScoped<IAgentManagementRepository, AgentManagementRepository>();
builder.Services.AddScoped<IAgentManagementService, AgentManagementService>();
// END Agent Management Services

builder.Services.AddScoped<IGeoLookupRepository, GeoLookupRepository>();
builder.Services.AddScoped<IGeoLookupService, GeoLookupService>();

// Start Register services for Product Management
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

// Start Provider Product

builder.Services.AddScoped<IProviderProductRepository, ProviderProductRepository>();
builder.Services.AddScoped<IProviderProductService, ProviderProductService>();

// END Provider Product

// Register ProductBaseCommission dependencies

builder.Services.AddScoped<IProductBaseCommissionRepository, ProductBaseCommissionRepository>();
builder.Services.AddScoped<IProductBaseCommissionService, ProductBaseCommissionService>();

// END  ProductBaseCommission dependencies

// Register ProductImage dependencies. May be not using right now
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
//.Services.AddScoped<IProductLookupService, ProductLookupService>();
// END Product Management

builder.Services.AddDataProtection();

builder.Services.AddScoped<IEmailSettingRepository, EmailSettingRepository>();
builder.Services.AddScoped<IEmailLogRepository, EmailLogRepository>();
builder.Services.AddScoped<IEmailEncryptionService, EmailEncryptionService>();
builder.Services.AddScoped<IEmailSettingService, EmailSettingService>();
builder.Services.AddScoped<IEmailLogService, EmailLogService>();
builder.Services.AddScoped<IEmailSenderService, MailKitEmailSenderService>();
builder.Services.AddScoped<IEmailHelper, EmailHelper>();

/// END Register services

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Test Configuration Access - This can be removed after verification

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SuperCRM.Persistence.DbContexts.SuperCrmDbContext>();
    var canConnect = db.Database.CanConnect();
    Console.WriteLine($"Database connection successful: {canConnect}");

    var seedService = scope.ServiceProvider.GetRequiredService<IdentitySeedService>();
    
    await seedService.SeedAsync();
    
}


// Access the application name from configuration
// Siddik, 22MAR2026
var appName = builder.Configuration["AppSettings:ApplicationName"];

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
