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
public class ProjectsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly string wwwRootPath;

    public ProjectsController(ApplicationDbContext context, UserManager<AppUser> UserManager,
        IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _userManager = UserManager;
        _webHostEnvironment = webHostEnvironment;
        wwwRootPath = _webHostEnvironment.WebRootPath;
    }

    // GET: Projects
    public async Task<IActionResult> Index()
    {
        var applicationDbContext = _context.Projects.Include(p => p.Portfolio);
        return View(await applicationDbContext.ToListAsync());
    }

    // GET: Projects/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var project = await _context.Projects
            .Include(p => p.Portfolio)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }

    // GET: Projects/Create
    public IActionResult Create()
    {
        ViewData["PortfolioId"] = new SelectList(_context.Portfolios, "Id", "Bio");
        return View();
    }

    // POST: Projects/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProjectDto projectDto)
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
            Project project = new()
            {
                Title = projectDto.Title,
                Description = projectDto.Description,
                ImageUrl = projectDto.ImageUrl,
                ProjectUrl = projectDto.ProjectUrl,
                PortfolioId = portfolio.Id,
                Portfolio = portfolio,
                CreatedAt = DateTime.Now
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /*ViewData["PortfolioId"] = new SelectList(_context.Portfolios, "Id", "Bio", projectDto.PortfolioId);*/
        return View(projectDto);
    }

    // GET: Projects/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var project = await _context.Projects.FindAsync(id);
        if (project == null)
        {
            return NotFound();
        }
        ViewData["PortfolioId"] = new SelectList(_context.Portfolios, "Id", "Bio", project.PortfolioId);
        return View(project);
    }

    // POST: Projects/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditProjectDto editProjectDto)
    {
        if (id != editProjectDto.Id)
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

        var project = await _context.Projects
            .FirstOrDefaultAsync(p => p.Id == editProjectDto.Id && p.PortfolioId == portfolio.Id);

        if (project == null)
        {
            return NotFound("Project not found.");
        }
        ;

        if (ModelState.IsValid)
        {
            if (project == null) { return NotFound(); }

            project.Title = editProjectDto.Title;
            project.Description = editProjectDto.Description;
            project.ImageUrl = editProjectDto.ImageUrl;
            project.ProjectUrl = editProjectDto.ProjectUrl;
            
            try
            {
                _context.Update(project);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(project.Id))
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
        ViewData["PortfolioId"] = new SelectList(_context.Portfolios, "Id", "Bio", project.PortfolioId);
        return View(project);
    }

    // GET: Projects/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var project = await _context.Projects
            .Include(p => p.Portfolio)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (project == null)
        {
            return NotFound();
        }

        return View(project);
    }

    // POST: Projects/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project != null)
        {
            _context.Projects.Remove(project);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ProjectExists(int id)
    {
        return _context.Projects.Any(e => e.Id == id);
    }
}
