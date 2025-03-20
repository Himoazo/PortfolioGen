using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioGen.Data;

namespace PortfolioGen.Controllers;

public class PublicController : Controller
{
    private readonly ApplicationDbContext _context;

    public PublicController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
            return View();
    }

    [Route("/")]
    [Route("/{id}")]
    public IActionResult HandleParam(string? id = null)
    {
        if (!string.IsNullOrEmpty(id))
        {
            // Log the parameter to the console
            Console.WriteLine($"URL Parameter: {id}");

            // Return the Profile view or content
            return View("Profile", id);
        }

        // If parameter doesn't exist, show index page
        return View("Index");
    }
}
