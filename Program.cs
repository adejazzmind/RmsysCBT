using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RmsysCBT.Data;
using RmsysCBT.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Setup In-Memory Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("RmsysCBT_Db"));

// 2. Identity with Roles (Updated)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4; // Easy password for testing
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Required for Login/Register pages

var app = builder.Build();

// 3. Seed Data (Admin User & Questions)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Pass user/role managers to seeder
    DbInitializer.Initialize(context, userManager, roleManager).Wait();
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Verify Identity
app.UseAuthorization();  // Check Access

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Activate Login Pages

app.Run();