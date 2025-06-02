using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioGen.Data;
using PortfolioGen.Models;
using PortfolioGen.DTOs;
using System.Text.Json;
using Microsoft.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace PortfolioGen.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly UserManager<AppUser> _userManager;
    private readonly ApplicationDbContext _context;
    public HomeController(IHttpClientFactory httpClientFactory, UserManager<AppUser> UserManager, ApplicationDbContext context)
    {
       _httpClientFactory = httpClientFactory;
       _userManager = UserManager;
        _context = context;
    }


    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null) { return RedirectToAction("Login", "Account"); }

        var portfolio = await _context.Portfolios
            .Include(p => p.Projects)
            .Include(p => p.SocialLinks)
            .FirstOrDefaultAsync(p => p.AppUserId == user.Id);

        if (portfolio == null) { return RedirectToAction("Create", "Portfolios"); }

        var githubUsername = user.UserName;

        IEnumerable<GitHubRepoDto> repos = [];
        if (!string.IsNullOrEmpty(githubUsername))
        {
            var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            $"https://api.github.com/users/{githubUsername}/repos")
            {
                Headers =
            {
                { HeaderNames.UserAgent, "HttpRequestsSample" }
            }
            };

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream =
                    await httpResponseMessage.Content.ReadAsStreamAsync();

                var result = await JsonSerializer.DeserializeAsync<IEnumerable<GitHubRepoDto>>(contentStream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                repos = result ?? [];
            }
        }

        var publicPortfolio = new PublicPortfolioDto
        {
            Name = user.Name,
            Title = portfolio.Title,
            Bio = portfolio.Bio,
            ProfileImage = portfolio.ProfileImage,
            Projects = portfolio.Projects
        ?.Select(p => new ProjectDto
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            ProjectUrl = p.ProjectUrl,
            GithubUrl = p.GithubUrl,
            PortfolioId = p.PortfolioId
        }).ToList() ?? [],

            SocialLinks = portfolio.SocialLinks
        ?.Select(s => new SocialLinkDto
        {
            Id = s.Id,
            Platform = s.Platform,
            Url = s.Url,
            PortfolioId = s.PortfolioId
        }).ToList() ?? [],

            GitHubRepos = repos?.ToList() ?? []
        };

        return View(publicPortfolio);
    }

    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
