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

namespace PortfolioGen.Controllers;


public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly UserManager<AppUser> _userManager;
    public HomeController(IHttpClientFactory httpClientFactory, UserManager<AppUser> UserManager)
    {
       _httpClientFactory = httpClientFactory;
       _userManager = UserManager;
    }


    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        var githubUsername = user?.UserName;

        if (string.IsNullOrEmpty(githubUsername)) 
        {
            return View();
        }

  

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

            var repos = await JsonSerializer.DeserializeAsync<IEnumerable<GitHubRepoDto>>(contentStream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Console.WriteLine(repos);

            return View(repos);
        }

        return View();
    }

    

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
