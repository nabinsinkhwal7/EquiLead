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
    public class TbljobsController : Controller
    {
        private readonly EquiDbContext _context;

        public TbljobsController(EquiDbContext context)
        {
            _context = context;
        }

        // GET: Tbljobs
        public async Task<IActionResult> Index()
        {
            var equiDbContext = _context.Tbljobs.Include(t => t.Company);
            return View(await equiDbContext.ToListAsync());
        }

        // GET: Tbljobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbljob = await _context.Tbljobs
                .Include(t => t.Company)
                .FirstOrDefaultAsync(m => m.Jobid == id);
            if (tbljob == null)
            {
                return NotFound();
            }

            return View(tbljob);
        }

        // GET: Tbljobs/Create
        public IActionResult Create()
        {
            ViewData["Companyid"] = new SelectList(_context.Tblcompanies, "Companyid", "Name");
            ViewData["Cityid"] = new SelectList(_context.MstLookups.Where(p=>p.Lookupflag == 55), "Lookupcode", "Description");
            ViewData["WDid"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 56), "Lookupcode", "Description");
            ViewData["ETid"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 57), "Lookupcode", "Description");
            return View();
        }

        // POST: Tbljobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Jobid,Jobtitle,Createdat,Updatedat,Workmode,Cityid,Yearexperience,Salaryrange,Dateposted,Roleoverview,Keyresponsibility,Skill,Education,Experience,Jobpostinglink,Applicationdeadline,Companyid,Employeetype")] Tbljob tbljob)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tbljob);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Companyid"] = new SelectList(_context.Tblcompanies, "Companyid", "Companyid", tbljob.Companyid);
            return View(tbljob);
        }

        // GET: Tbljobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbljob = await _context.Tbljobs.FindAsync(id);
            if (tbljob == null)
            {
                return NotFound();
            }
            ViewData["Companyid"] = new SelectList(_context.Tblcompanies, "Companyid", "Name");
            ViewData["Cityid"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 55), "Lookupcode", "Description");
            ViewData["WDid"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 56), "Lookupcode", "Description");
            ViewData["ETid"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 57), "Lookupcode", "Description");
            return View(tbljob);
        }

        // POST: Tbljobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Jobid,Jobtitle,Createdat,Updatedat,Workmode,Cityid,Yearexperience,Salaryrange,Dateposted,Roleoverview,Keyresponsibility,Skill,Education,Experience,Jobpostinglink,Applicationdeadline,Companyid,Employeetype")] Tbljob tbljob)
        {
            if (id != tbljob.Jobid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tbljob);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TbljobExists(tbljob.Jobid))
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
            ViewData["Companyid"] = new SelectList(_context.Tblcompanies, "Companyid", "Companyid", tbljob.Companyid);
            return View(tbljob);
        }

        // GET: Tbljobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tbljob = await _context.Tbljobs
                .Include(t => t.Company)
                .FirstOrDefaultAsync(m => m.Jobid == id);
            if (tbljob == null)
            {
                return NotFound();
            }

            return View(tbljob);
        }

        // POST: Tbljobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tbljob = await _context.Tbljobs.FindAsync(id);
            if (tbljob != null)
            {
                _context.Tbljobs.Remove(tbljob);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TbljobExists(int id)
        {
            return _context.Tbljobs.Any(e => e.Jobid == id);
        }
    }
}
