using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortfolioGen.Data;
using PortfolioGen.Models;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication()
    .AddGitHub(options =>
    {
        options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
        options.ClaimActions.MapJsonKey("urn:github:login", "login");
        options.Scope.Add("public_repo");
        options.SaveTokens = true;

        options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                var githubUsername = context.Identity.FindFirst("urn:github:login")?.Value;

                var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
                var signInManager = context.HttpContext.RequestServices.GetRequiredService<SignInManager<AppUser>>();
                var user = await userManager.GetUserAsync(context.Principal);

                if (user != null && string.IsNullOrEmpty(user.UserName))
                {
                    user.UserName = githubUsername;
                    await userManager.UpdateAsync(user);
                }
            }
        };

    });

builder.Services.AddHttpClient(); // HttpClient

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

app.MapFallbackToController("Profile", "Public"); // Public portfolio 

app.Run();
