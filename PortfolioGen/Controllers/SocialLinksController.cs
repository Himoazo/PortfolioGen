using System;
using System.Collections.Generic;
using System.Linq;
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
public class SocialLinksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public SocialLinksController(ApplicationDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: SocialLinks
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var portfolio = await _context.Portfolios
            .FirstOrDefaultAsync(p => p.AppUserId == userId);

        if (portfolio == null)
        {
            return RedirectToAction("Create", "Portfolios");
        }

        var socialLinks = await _context.SocialLinks
            .Where(s => s.PortfolioId == portfolio.Id)
            .Include(s => s.Portfolio)
            .ToListAsync();

        return View(socialLinks);
    }

    // GET: SocialLinks/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var portfolio = await _context.Portfolios
            .FirstOrDefaultAsync(p => p.AppUserId == userId);

        if (portfolio == null)
        {
            return RedirectToAction("Create", "Portfolios");
        }

        var socialLink = await _context.SocialLinks
            .Include(s => s.Portfolio)
            .FirstOrDefaultAsync(m => m.Id == id && m.PortfolioId == portfolio.Id);

        if (socialLink == null)
        {
            return NotFound();
        }

        return View(socialLink);
    }

    // GET: SocialLinks/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: SocialLinks/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSocialLinkDto socialLinkDto)
    {
        if (ModelState.IsValid)
        {
            var userId = _userManager.GetUserId(User);
            var portfolio = await _context.Portfolios
                .FirstOrDefaultAsync(p => p.AppUserId == userId);

            if (portfolio == null)
            {
                return RedirectToAction("Create", "Portfolios");
            }

            SocialLink socialLink = new()
            {
                Platform = socialLinkDto.Platform,
                Url = socialLinkDto.Url,
                PortfolioId = portfolio.Id,
                Portfolio = portfolio
            };

            _context.Add(socialLink);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(socialLinkDto);
    }

    // GET: SocialLinks/Edit/5
    /*public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var portfolio = await _context.Portfolios
            .FirstOrDefaultAsync(p => p.AppUserId == userId);

        if (portfolio == null)
        {
            return RedirectToAction("Create", "Portfolios");
        }

        var socialLink = await _context.SocialLinks
            .FirstOrDefaultAsync(s => s.Id == id && s.PortfolioId == portfolio.Id);

        if (socialLink == null)
        {
            return NotFound();
        }

        var updateDto = new UpdateSocialLinkDto
        {
            Id = socialLink.Id,
            Platform = socialLink.Platform,
            Url = socialLink.Url
        };

        return View(updateDto);
    }*/
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var socialLink = await _context.SocialLinks.FindAsync(id);
        if (socialLink == null)
        {
            return NotFound();
        }
        ViewData["PortfolioId"] = new SelectList(_context.Portfolios, "Id", "Bio", socialLink.PortfolioId);
        return View(socialLink);
    }

    // POST: SocialLinks/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateSocialLinkDto updateSocialLinkDto)
    {
        if (id != updateSocialLinkDto.Id)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var portfolio = await _context.Portfolios
            .FirstOrDefaultAsync(p => p.AppUserId == userId);

        if (portfolio == null)
        {
            return NotFound("Portfolio not found.");
        }

        var socialLink = await _context.SocialLinks
            .FirstOrDefaultAsync(s => s.Id == updateSocialLinkDto.Id && s.PortfolioId == portfolio.Id);

        if (socialLink == null)
        {
            return NotFound("Social link not found.");
        }

        if (ModelState.IsValid)
        {
            socialLink.Platform = updateSocialLinkDto.Platform;
            socialLink.Url = updateSocialLinkDto.Url;

            try
            {
                _context.Update(socialLink);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SocialLinkExists(socialLink.Id))
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

        return View(updateSocialLinkDto);
    }

    // GET: SocialLinks/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var userId = _userManager.GetUserId(User);
        var portfolio = await _context.Portfolios
            .FirstOrDefaultAsync(p => p.AppUserId == userId);

        if (portfolio == null)
        {
            return RedirectToAction("Create", "Portfolios");
        }

        var socialLink = await _context.SocialLinks
            .Include(s => s.Portfolio)
            .FirstOrDefaultAsync(m => m.Id == id && m.PortfolioId == portfolio.Id);

        if (socialLink == null)
        {
            return NotFound();
        }

        return View(socialLink);
    }

    // POST: SocialLinks/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = _userManager.GetUserId(User);
        var portfolio = await _context.Portfolios
            .FirstOrDefaultAsync(p => p.AppUserId == userId);

        if (portfolio == null)
        {
            return RedirectToAction("Create", "Portfolios");
        }

        var socialLink = await _context.SocialLinks
            .FirstOrDefaultAsync(s => s.Id == id && s.PortfolioId == portfolio.Id);

        if (socialLink != null)
        {
            _context.SocialLinks.Remove(socialLink);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private bool SocialLinkExists(int id)
    {
        return _context.SocialLinks.Any(e => e.Id == id);
    }
}