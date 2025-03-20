using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using PortfolioGen.Data;
using PortfolioGen.DTOs;
using PortfolioGen.Models;

namespace PortfolioGen.Controllers;

[Authorize]
public class PortfoliosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string wwwRootPath;
    public PortfoliosController(ApplicationDbContext context, UserManager<AppUser> UserManager, 
        IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _userManager = UserManager;
        _webHostEnvironment = webHostEnvironment;
        wwwRootPath = _webHostEnvironment.WebRootPath;
    }

    // GET: Portfolios
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var applicationDbContext = _context.Portfolios.Include(p => p.AppUser).Where(p => p.AppUserId == userId);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: Portfolios/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var portfolioDto = await _context.Portfolios
            .FirstOrDefaultAsync(m => m.Id == id);
        if (portfolioDto == null)
        {
            return NotFound();
        }

        return View(portfolioDto);
    }

    // GET: Portfolios/Create
    public IActionResult Create()
    {
        ViewData["AppUserId"] = new SelectList(_context.Set<AppUser>(), "Id", "Id");
        return View();
    }

    // POST: Portfolios/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PortfolioDto portfolioDto)
    {
        var userId = _userManager.GetUserId(User);
        if (userId is null) return Unauthorized();

        if (ModelState.IsValid)
        {
            Portfolio portfolio = new()
            {
                Title = portfolioDto.Title,
                Bio = portfolioDto.Bio,
                Published = portfolioDto.Published,
                AppUserId = userId,
                CreatedAt = DateOnly.FromDateTime(DateTime.Now)
            };

            if (portfolioDto.ProfileImg is not null)
            {
                // Generate a unique filename
                string fileName = Path.GetFileNameWithoutExtension(portfolioDto.ProfileImg.FileName);
                fileName = fileName.Replace(" ", string.Empty) + DateTime.Now.ToString("yymmssfff");
                string extension = Path.GetExtension(portfolioDto.ProfileImg.FileName);
                string finalFileName = fileName + extension;

                // Save the filename in the database
                portfolio.ProfileImage = finalFileName;

                // Define the path where the file will be saved
                string path = Path.Combine(wwwRootPath, "images", finalFileName);

                // Create the file and copy the content
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await portfolioDto.ProfileImg.CopyToAsync(fileStream);
                    fileStream.Close();
                }
            }

            _context.Add(portfolio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        return View(portfolioDto);
    }

    // GET: Portfolios/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var portfolio = await _context.Portfolios.FindAsync(id);
        if (portfolio == null) return NotFound();

        var editPortfolioDto = new EditPortfolioDto
        {
            Id = portfolio.Id,
            Title = portfolio.Title,
            Bio = portfolio.Bio,
            ProfileImg = portfolio.ProfileImg
        };

        return View(editPortfolioDto);
    }



    // POST: Portfolios/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditPortfolioDto EditportfolioDto)
    {
        var userId = _userManager.GetUserId(User);
        if (userId is null) return Unauthorized();

        var portfolio = await _context.Portfolios.FindAsync(id);
        if (portfolio is null) return NotFound();

        if (userId != portfolio.AppUserId) return Unauthorized();
        if (id != EditportfolioDto.Id) return NotFound();

        if (ModelState.IsValid)
        {
            portfolio.Title = EditportfolioDto.Title;
            portfolio.Bio = EditportfolioDto.Bio;
            portfolio.ProfileImg = EditportfolioDto.ProfileImg;

            try
            {
                _context.Update(portfolio);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PortfolioExists(portfolio.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        /*ViewData["AppUserId"] = new SelectList(_context.Set<AppUser>(), "Id", "Id", EditportfolioDto.AppUserId);*/
        return View(EditportfolioDto);
    }

    // GET: Portfolios/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var portfolio = await _context.Portfolios
            .Include(p => p.AppUser)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (portfolio == null)
        {
            return NotFound();
        }

        return View(portfolio);
    }

    // POST: Portfolios/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var portfolio = await _context.Portfolios.FindAsync(id);
        if (portfolio != null)
        {
            _context.Portfolios.Remove(portfolio);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PortfolioExists(int id)
    {
        return _context.Portfolios.Any(e => e.Id == id);
    }
}
