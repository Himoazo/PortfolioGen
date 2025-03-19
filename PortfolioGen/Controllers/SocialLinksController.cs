using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PortfolioGen.Data;
using PortfolioGen.Models;

namespace PortfolioGen.Controllers
{
    public class SocialLinksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SocialLinksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SocialLinks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SocialLinks.Include(s => s.Portfolio);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SocialLinks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var socialLink = await _context.SocialLinks
                .Include(s => s.Portfolio)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (socialLink == null)
            {
                return NotFound();
            }

            return View(socialLink);
        }

        // GET: SocialLinks/Create
        public IActionResult Create()
        {
            ViewData["PortfolioId"] = new SelectList(_context.Portfolios, "Id", "Bio");
            return View();
        }

        // POST: SocialLinks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Platform,Url,PortfolioId")] SocialLink socialLink)
        {
            if (ModelState.IsValid)
            {
                _context.Add(socialLink);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PortfolioId"] = new SelectList(_context.Portfolios, "Id", "Bio", socialLink.PortfolioId);
            return View(socialLink);
        }

        // GET: SocialLinks/Edit/5
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Platform,Url,PortfolioId")] SocialLink socialLink)
        {
            if (id != socialLink.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
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
            ViewData["PortfolioId"] = new SelectList(_context.Portfolios, "Id", "Bio", socialLink.PortfolioId);
            return View(socialLink);
        }

        // GET: SocialLinks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var socialLink = await _context.SocialLinks
                .Include(s => s.Portfolio)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var socialLink = await _context.SocialLinks.FindAsync(id);
            if (socialLink != null)
            {
                _context.SocialLinks.Remove(socialLink);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SocialLinkExists(int id)
        {
            return _context.SocialLinks.Any(e => e.Id == id);
        }
    }
}
