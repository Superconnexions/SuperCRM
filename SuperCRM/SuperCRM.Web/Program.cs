using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SuperCRM.Infrastructure.Services;

//using SuperCRM.Infrastructure.Services;
using SuperCRM.Persistence.Dapper;
using SuperCRM.Persistence.DbContexts;
using SuperCRM.Persistence.Identity;
using SuperCRM.Shared;
//using SuperCRM.Shared;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<SuperCrmDbContext>(options =>
    options.UseSqlServer(connectionString));

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
