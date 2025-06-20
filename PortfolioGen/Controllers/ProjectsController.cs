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


    public ProjectsController(ApplicationDbContext context, UserManager<AppUser> UserManager,
        IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _userManager = UserManager;
        _webHostEnvironment = webHostEnvironment;
    }

    // GET: Projects
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var portfolio = await _context.Portfolios
            .FirstOrDefaultAsync(p => p.AppUserId == userId);

        if (portfolio == null)
        {
            return RedirectToAction("Create", "Portfolios");
        }

        var projects = await _context.Projects
            .Where(p => p.PortfolioId == portfolio.Id)
            .Include(p => p.Portfolio)
            .ToListAsync();

        var projectDtos = projects.Select(p => new ProjectDto
        {
            Id = p.Id,
            Title = p.Title,
            Description = p.Description,
            ProjectUrl = p.ProjectUrl,
            GithubUrl = p.GithubUrl,
            PortfolioId = p.PortfolioId
        }).ToList();

        return View(projectDtos);
    }

    // GET: Projects/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Projects/Create
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
                ProjectUrl = projectDto.ProjectUrl,
                GithubUrl = projectDto.GithubUrl,
                PortfolioId = portfolio.Id,
                Portfolio = portfolio,
                CreatedAt = DateTime.Now
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

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

        return View(project);
    }

    // POST: Projects/Edit/5
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
            project.ProjectUrl = editProjectDto.ProjectUrl;
            project.GithubUrl = editProjectDto.GithubUrl;
            
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
