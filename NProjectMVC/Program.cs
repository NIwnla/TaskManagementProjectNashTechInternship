using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NProjectMVC.Data;
using NProjectMVC.Extension;
using NProjectMVC.Interface;
using NProjectMVC.Models;
using NProjectMVC.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDbContext<NProjectContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDbContext"));
});
builder.Services.AddDefaultIdentity<User>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    // Password settings.
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;

    // Lockout settings.
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;

    // User settings.
    options.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<NProjectContext>();

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = IdentityConstants.ApplicationScheme;
//    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
//}).AddIdentityCookies(options => { });


builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.SlidingExpiration = true;
});

builder.Services.AddScoped<IRepository<Project>, Repository<Project>>();
builder.Services.AddScoped<IRepository<ProjectTask>, Repository<ProjectTask>>();
builder.Services.AddScoped<IRepository<WorkedTask>, Repository<WorkedTask>>();
builder.Services.AddScoped<IUserRepository,UserRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    //app.UseMigrationsEndPoint();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "MyArea",
	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Manager", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    string emailAdmin = "admin@admin.com";
    string emailManager = "manager@manager.com";
    string emailUser = "user@user.com";
    string password = "Son123456789";
    if (await userManager.FindByEmailAsync(emailAdmin) == null)
    {
        var admin = new User
        {
            Email = emailAdmin,
            UserName = emailAdmin,
            EmailConfirmed = true
        };
        var manager = new User
        {
            Email = emailManager,
            UserName = emailManager,
            EmailConfirmed = true
        };
        var user = new User
        {
            Email = emailUser,
            UserName = emailUser,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(admin, password);
        await userManager.AddToRoleAsync(admin, "Admin");
        await userManager.CreateAsync(manager, password);
        await userManager.AddToRoleAsync(manager, "Manager");
        await userManager.CreateAsync(user, password);
        await userManager.AddToRoleAsync(user, "User");
    }


}

app.SeedData();

app.Run();
