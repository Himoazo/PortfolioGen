using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using PortfolioGen.Data;
using PortfolioGen.DTOs;
using PortfolioGen.Models;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;

namespace PortfolioGen.Controllers;


public class HomeController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly SignInManager<AppUser> _signInManager;

    public HomeController(IHttpClientFactory httpClientFactory, SignInManager<AppUser> signInManager)
    {
        _httpClientFactory = httpClientFactory;
        _signInManager = signInManager;
    }


    public async Task<IActionResult> Index()
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();
        var username = info.Principal.FindFirst("urn:github:login")?.Value;

        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"{claim.Type}: {claim.Value}");
        }

        var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Get,
            $"https://api.github.com/users/{username}/repos")
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
