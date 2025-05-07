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
    public class TblresourcesController : Controller
    {
        private readonly EquiDbContext _context;

        public TblresourcesController(EquiDbContext context)
        {
            _context = context;
        }

        // GET: Tblresources
        public async Task<IActionResult> Index()
        {
            var equiDbContext = _context.Tblresources.Include(t => t.Theme);
            return View(await equiDbContext.ToListAsync());
        }

        // GET: Tblresources/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblresource = await _context.Tblresources
                .Include(t => t.Theme)
                .FirstOrDefaultAsync(m => m.Resourceid == id);
            if (tblresource == null)
            {
                return NotFound();
            }

            return View(tblresource);
        }

        // GET: Tblresources/Create
        public IActionResult Create()
        {
            ViewData["ThemeId"] = new SelectList(_context.MstThemes, "ThemeId", "ThemeId");
            return View();
        }

        // POST: Tblresources/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Resourceid,Rshead,Rsimage,Rsshortdescription,Rsage,ThemeId,Isverified,Isrelatedrs,Isdeleted,Ispublic,Rsimagebuttonlink,Rsversion,Relatedresourceid,Createdby,Createdon,Updatedby,Updatedon")] Tblresource tblresource)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblresource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ThemeId"] = new SelectList(_context.MstThemes, "ThemeId", "ThemeId", tblresource.ThemeId);
            return View(tblresource);
        }

        // GET: Tblresources/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblresource = await _context.Tblresources.FindAsync(id);
            if (tblresource == null)
            {
                return NotFound();
            }
            ViewData["ThemeId"] = new SelectList(_context.MstThemes, "ThemeId", "ThemeId", tblresource.ThemeId);
            return View(tblresource);
        }

        // POST: Tblresources/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Resourceid,Rshead,Rsimage,Rsshortdescription,Rsage,ThemeId,Isverified,Isrelatedrs,Isdeleted,Ispublic,Rsimagebuttonlink,Rsversion,Relatedresourceid,Createdby,Createdon,Updatedby,Updatedon")] Tblresource tblresource)
        {
            if (id != tblresource.Resourceid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblresource);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblresourceExists(tblresource.Resourceid))
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
            ViewData["ThemeId"] = new SelectList(_context.MstThemes, "ThemeId", "ThemeId", tblresource.ThemeId);
            return View(tblresource);
        }

        // GET: Tblresources/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblresource = await _context.Tblresources
                .Include(t => t.Theme)
                .FirstOrDefaultAsync(m => m.Resourceid == id);
            if (tblresource == null)
            {
                return NotFound();
            }

            return View(tblresource);
        }

        // POST: Tblresources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblresource = await _context.Tblresources.FindAsync(id);
            if (tblresource != null)
            {
                _context.Tblresources.Remove(tblresource);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblresourceExists(int id)
        {
            return _context.Tblresources.Any(e => e.Resourceid == id);
        }
    }
}
