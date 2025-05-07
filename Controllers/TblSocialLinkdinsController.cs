using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EquidCMS.Models;

namespace EquidCMS.Controllers
{
    public class TblSocialLinkdinsController : Controller
    {
        private readonly EquiDbContext _context;

        public TblSocialLinkdinsController(EquiDbContext context)
        {
            _context = context;
        }

        // GET: TblSocialLinkdins
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblSocialLinkdins.ToListAsync());
        }

        // GET: TblSocialLinkdins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblSocialLinkdin = await _context.TblSocialLinkdins
                .FirstOrDefaultAsync(m => m.ScLinkdinId == id);
            if (tblSocialLinkdin == null)
            {
                return NotFound();
            }

            return View(tblSocialLinkdin);
        }

        // GET: TblSocialLinkdins/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblSocialLinkdins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScLinkdinId,Name,Email,Post,Organization,Linkdinlink,CreatedBy,CreatedOn")] TblSocialLinkdin tblSocialLinkdin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblSocialLinkdin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblSocialLinkdin);
        }

        // GET: TblSocialLinkdins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblSocialLinkdin = await _context.TblSocialLinkdins.FindAsync(id);
            if (tblSocialLinkdin == null)
            {
                return NotFound();
            }
            return View(tblSocialLinkdin);
        }

        // POST: TblSocialLinkdins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScLinkdinId,Name,Email,Post,Organization,Linkdinlink,CreatedBy,CreatedOn")] TblSocialLinkdin tblSocialLinkdin)
        {
            if (id != tblSocialLinkdin.ScLinkdinId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblSocialLinkdin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblSocialLinkdinExists(tblSocialLinkdin.ScLinkdinId))
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
            return View(tblSocialLinkdin);
        }

        // GET: TblSocialLinkdins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblSocialLinkdin = await _context.TblSocialLinkdins
                .FirstOrDefaultAsync(m => m.ScLinkdinId == id);
            if (tblSocialLinkdin == null)
            {
                return NotFound();
            }

            return View(tblSocialLinkdin);
        }

        // POST: TblSocialLinkdins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblSocialLinkdin = await _context.TblSocialLinkdins.FindAsync(id);
            if (tblSocialLinkdin != null)
            {
                _context.TblSocialLinkdins.Remove(tblSocialLinkdin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblSocialLinkdinExists(int id)
        {
            return _context.TblSocialLinkdins.Any(e => e.ScLinkdinId == id);
        }
    }
}
