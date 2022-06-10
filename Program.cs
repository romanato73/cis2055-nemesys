using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using cis2055_nemesys.Models;
using cis2055_nemesys.Models.Interfaces;
using cis2055_nemesys.Models.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services
    .AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddTransient<INemesysRepository, NemesysRepository>();

/**
 * +---------------------------------------------------------------+
 * |                     IDENTITY FRAMEWORK                        |
 * +---------------------------------------------------------------+
 */

builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    // Password
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    // User settings.
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

/**
 * +---------------------------------------------------------------+
 * |                    COOKIE/SESSION POLICY                      |
 * +---------------------------------------------------------------+
 */

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    options.SlidingExpiration = true;
});

/**
 * +---------------------------------------------------------------+
 * |                   CONNECTION CONFIGURATION                    |
 * +---------------------------------------------------------------+
 */

// SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString)
);

/**
 * +---------------------------------------------------------------+
 * |                      APPLICATION BUILDER                      |
 * +---------------------------------------------------------------+
 */

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseMigrationsEndPoint();
} else {
    app.UseExceptionHandler("/Home/Error");
}

app.UseHttpsRedirection();
app.UseStatusCodePages();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
