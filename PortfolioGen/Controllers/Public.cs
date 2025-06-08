using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using PortfolioGen.Data;
using PortfolioGen.DTOs;
using PortfolioGen.Models;
using System.Net.Http;
using System.Text.Json;

namespace PortfolioGen.Controllers;

public class PublicController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    public PublicController(ApplicationDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Profile(string? id)
    {
        
        if (string.IsNullOrEmpty(id))
        {
            id = HttpContext.Request.Path.Value?.TrimStart('/'); 
        }


        if (!string.IsNullOrEmpty(id))
        {
            var portfolio = await _context.Portfolios
            .Include(p => p.Projects)
            .Include(p => p.SocialLinks)
            .Include(p => p.AppUser)
            .FirstOrDefaultAsync(p => p.PortfolioSlug == id);

            if (portfolio is null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (portfolio.Published == false)
            { 
                PublicPortfolioDto UnPublishedPortfolio = new();
                
                ViewBag.Unpublished = true;
                return View(UnPublishedPortfolio);

            }

            var githubUsername = portfolio.AppUser.UserName;

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
                Name = portfolio.AppUser.Name,
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

        return View();
    }
}
