using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PortfolioGen.Data;
using PortfolioGen.Models;
using System.Security.Claims;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication()
    .AddGitHub(options =>
    {
        options.ClientId = builder.Configuration["Authentication:GitHub:ClientId"];
        options.ClientSecret = builder.Configuration["Authentication:GitHub:ClientSecret"];
        options.ClaimActions.MapJsonKey("urn:github:login", "login");
        options.ClaimActions.MapJsonKey("urn:github:name", "name");
        options.Scope.Add("public_repo");
        options.SaveTokens = true;

        /*options.Events = new OAuthEvents
        {
            OnCreatingTicket = async context =>
            {
                
                var githubUsername = context.Identity.FindFirst("urn:github:login")?.Value;
                var displayName = context.Identity.FindFirst("urn:github:name")?.Value;

                var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();

                var loginProvider = context.Options.ClaimsIssuer;
                var providerKey = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var user = await userManager.FindByLoginAsync(loginProvider, providerKey);

                if (user != null)
                {
                    if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Name))
                    {
                        user.UserName ??= githubUsername;
                        user.Name ??= displayName;
                        await userManager.UpdateAsync(user);
                    }
                }
            }
        };*/

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
