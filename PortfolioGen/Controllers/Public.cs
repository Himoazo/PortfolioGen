using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioGen.Data;
using PortfolioGen.DTOs;

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


    public async Task<IActionResult> Profile(string? id)
    {
        
        if (string.IsNullOrEmpty(id))
        {
            id = HttpContext.Request.Path.Value?.TrimStart('/'); // Capture the original path
        }

        Console.WriteLine($"ID value: '{id}'"); // This gets printed

        if (!string.IsNullOrEmpty(id))
        {
            Console.WriteLine($"{id} ID is null");
            var porftolio = await _context.Portfolios
            .Include(p => p.Projects)
            .Include(p => p.SocialLinks)
            .FirstOrDefaultAsync(p => p.PortfolioSlug == id);

            if (porftolio is null)
            {
                Console.WriteLine($"{id} Portfolio is null");
                return NotFound("Potfolio was not found");
            }

            PortfolioDto dto = new()
            {
                Title = porftolio.Title,
                Bio = porftolio.Bio,
                ProfileImg = porftolio.ProfileImg
            };

            return View(dto);
        }
        Console.WriteLine($"Nothing Works"); // This gets printed
        return View("Index");
    }
}
