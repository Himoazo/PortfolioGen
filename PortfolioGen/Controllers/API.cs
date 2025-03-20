using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioGen.Data;
using PortfolioGen.Models;

namespace PortfolioGen.Controllers;

[Route("[controller]")]
[ApiController]
public class API : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public API(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPortfolio(string id)
    {
        /*//CONSIDER THIS IMPORTANTTTTT
        if (string.IsNullOrEmpty(id))
        {
            id = HttpContext.Request.Path.Value?.TrimStart('/');
        }*/
        var porftolio = await _context.Portfolios
            .Include(p => p.Projects)
            .Include(p => p.SocialLinks)
            .FirstOrDefaultAsync(p => p.PortfolioSlug == id);

        if(porftolio is null)
        {
            return NotFound("Potfolio was not found");
        }

        if (porftolio.Published == false)
        {
            return NotFound("Portfolio is not yet published");
        }

        return Ok(porftolio);
    }

}
