﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Hosting;
using PortfolioGen.Data;
using PortfolioGen.DTOs;
using PortfolioGen.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PortfolioGen.Controllers;

[Authorize]
public class PortfoliosController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly IWebHostEnvironment _webHostEnvironment;
    public PortfoliosController(ApplicationDbContext context, UserManager<AppUser> UserManager, 
        IWebHostEnvironment webHostEnvironment)
    {
        _context = context;
        _userManager = UserManager;
        _webHostEnvironment = webHostEnvironment;
    }

    // GET: Portfolios
    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var portfolio = await _context.Portfolios
            .Include(p => p.AppUser)
            .FirstOrDefaultAsync(p => p.AppUserId == userId);

        if (portfolio == null) 
        {

            return RedirectToAction("Create");
        }

        return View(portfolio);
    }
   

    // GET: Portfolios/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Portfolios/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PortfolioDto portfolioDto)
    {
        var userId = _userManager.GetUserId(User);
        if (userId is null) return Unauthorized();

        var existingPortfolio = await _context.Portfolios
            .Include(p => p.AppUser)
            .FirstOrDefaultAsync(p => p.AppUserId == userId);

        if (existingPortfolio != null) { return RedirectToAction(nameof(Index)); }

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
                portfolio.ProfileImage = await UploadImg(portfolioDto.ProfileImg);
                
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
            ProfileImg = portfolio.ProfileImg,
            ProfileImage = portfolio.ProfileImage
        };

        return View(editPortfolioDto);
    }



    // POST: Portfolios/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EditPortfolioDto EditportfolioDto, bool removeImage = false)
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

            if (removeImage)
            {
                if (portfolio.ProfileImage is not null)
                {
                    System.IO.File.Delete(Path.Combine(_webHostEnvironment.ContentRootPath, "..", "data", "images", portfolio.ProfileImage));
                    portfolio.ProfileImage = null; 
                }
            }
            else if (EditportfolioDto.ProfileImg is not null)
            {
                if (portfolio.ProfileImage is not null)
                {
                    System.IO.File.Delete(Path.Combine(_webHostEnvironment.ContentRootPath, "..", "data", "images", portfolio.ProfileImage));
                }
                portfolio.ProfileImage = await UploadImg(EditportfolioDto.ProfileImg);
            }

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

    //Publish / unpublish
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePublish(int id)
    {
        var portfolio = await _context.Portfolios.FindAsync(id);
        if (portfolio == null) return NotFound();

        portfolio.Published = !portfolio.Published;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index)); // Redirect after toggling
    }

    //Upload images
    private async Task<string> UploadImg(IFormFile imageFile)
    {
        string fileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
        fileName = fileName.Replace(" ", string.Empty) + DateTime.Now.ToString("yymmssfff");

        string finalFileName = fileName + ".jpg";

        string path = Path.Combine(_webHostEnvironment.ContentRootPath, "..", "data", "images", finalFileName);

        var directory = Path.GetDirectoryName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory!);
        }

        using (var image = await Image.LoadAsync(imageFile.OpenReadStream()))
        {
            image.Mutate(x => x
                .Resize(new ResizeOptions
                {
                    Size = new Size(300, 300),
                    Mode = ResizeMode.Crop
                }));

            await image.SaveAsync(path, new JpegEncoder { Quality = 80 });
        }

        return finalFileName;
    }

    [Route("images/{filename}")]
    public async Task<IActionResult> GetUserImage(string filename)
    {

        if(string.IsNullOrEmpty(filename)) { return NoContent(); }

        var filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "..", "data", "images", filename);

        if (!System.IO.File.Exists(filePath))
        {
            return NoContent();
        }

        var imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(imageBytes, "image/jpeg");
    }

}
