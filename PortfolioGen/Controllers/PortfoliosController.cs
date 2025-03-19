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
using PortfolioGen.Data;
using PortfolioGen.DTOs;
using PortfolioGen.Models;

namespace PortfolioGen.Controllers;

[Authorize]
public class PortfoliosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    public PortfoliosController(ApplicationDbContext context, UserManager<AppUser> UserManager)
    {
        _context = context;
        _userManager = UserManager;
    }

    // GET: Portfolios
    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _context.Portfolios.Include(p => p.AppUser);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: Portfolios/Details/5
    public async Task<IActionResult> Details(int? id)
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

            _context.Add(portfolio);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        return View(portfolioDto);
    }

    // GET: Portfolios/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var portfolio = await _context.Portfolios.FindAsync(id);
        if (portfolio == null)
        {
            return NotFound();
        }
        ViewData["AppUserId"] = new SelectList(_context.Set<AppUser>(), "Id", "Id", portfolio.AppUserId);
        return View(portfolio);
    }

    // POST: Portfolios/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,PortfolioSlug,Title,Bio,ProfileImage,Published,CreatedAt,AppUserId")] Portfolio portfolio)
    {
        if (id != portfolio.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
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
        ViewData["AppUserId"] = new SelectList(_context.Set<AppUser>(), "Id", "Id", portfolio.AppUserId);
        return View(portfolio);
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
