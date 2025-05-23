using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EquidCMS.Models;
using EquidCMS.Services;

namespace EquidCMS.Controllers
{
    public class TbljobsController : Controller
    {
        private readonly EquiDbContext _context;
        private readonly JobScraperService _jobScraperService;

        public TbljobsController(EquiDbContext context, JobScraperService jobScraperService)
        {
            _context = context;
            _jobScraperService = jobScraperService;
        }

        // GET: Tbljobs
        public async Task<IActionResult> Index()
        {
            var equiDbContext = _context.Tbljobs.Where(x=>x.Isdeleted==null || x.Isdeleted==false).Include(t => t.Company);
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
            ViewData["FunctionalCategory"] = new SelectList(_context.MstLookups.Where(p=>p.Lookupflag == 14 && p.Active==true), "Lookupcode", "Description");
            ViewData["WDid"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 56 && p.Active==true), "Lookupcode", "Description");
            ViewData["ETid"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 57 && p.Active == true), "Lookupcode", "Description");
            ViewData["LeavePolicies"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 17 && p.Active == true), "Lookupcode", "Description");
            ViewData["FlexibleWorkOption"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 18 && p.Active == true), "Lookupcode", "Description");
            ViewData["LearningAndDevelopment"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 19 && p.Active == true), "Lookupcode", "Description");
            ViewData["HealthcareAndWellness"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 20 && p.Active == true), "Lookupcode", "Description");
            ViewData["DEIAndWomenFriendlyPolicies"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 21 && p.Active==true), "Lookupcode", "Description");
            return View();
        }

        public IActionResult CreateAI()
        {
            ViewData["Companyid"] = new SelectList(_context.Tblcompanies.OrderBy(p=>p.Name), "Companyid", "Name");
            ViewData["FunctionalCategory"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 14 && p.Active == true), "Lookupcode", "Description");
            ViewData["WDid"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 56 && p.Active == true), "Lookupcode", "Description");
            ViewData["ETid"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 57 && p.Active == true), "Lookupcode", "Description");
            ViewData["LeavePolicies"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 17 && p.Active == true), "Lookupcode", "Description");
            ViewData["FlexibleWorkOption"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 18 && p.Active == true), "Lookupcode", "Description");
            ViewData["LearningAndDevelopment"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 19 && p.Active == true), "Lookupcode", "Description");
            ViewData["HealthcareAndWellness"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 20 && p.Active == true), "Lookupcode", "Description");
            ViewData["DEIAndWomenFriendlyPolicies"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 21 && p.Active == true), "Lookupcode", "Description");
            return View();
        }

        // POST: Tbljobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tbljob tbljob)
        {
            if (ModelState.IsValid)
            {
                tbljob.Createdat = DateTime.Now;
                _context.Add(tbljob);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeavePolicies"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 17 && p.Active == true), "Lookupcode", "Description");
            ViewData["FlexibleWorkOption"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 18 && p.Active == true), "Lookupcode", "Description");
            ViewData["LearningAndDevelopment"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 19 && p.Active == true), "Lookupcode", "Description");
            ViewData["HealthcareAndWellness"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 20 && p.Active == true), "Lookupcode", "Description");
            ViewData["DEIAndWomenFriendlyPolicies"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 21 && p.Active == true), "Lookupcode", "Description");
            ViewData["Companyid"] = new SelectList(_context.Tblcompanies, "Companyid", "Companyid", tbljob.Companyid);
            return View(tbljob);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAI(Tbljob tbljob)
        {
            if (ModelState.IsValid)
            {
                if (tbljob.Companyid == 0 || tbljob.Companyid==null)
                {
                    TempData["ErrorMessage"] = "Company must be selected";
                    return RedirectToAction("CreateAI"); 
                }
                   
                tbljob.Createdat = DateTime.Now;
                _context.Add(tbljob);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeavePolicies"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 17 && p.Active == true), "Lookupcode", "Description");
            ViewData["FlexibleWorkOption"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 18 && p.Active == true), "Lookupcode", "Description");
            ViewData["LearningAndDevelopment"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 19 && p.Active == true), "Lookupcode", "Description");
            ViewData["HealthcareAndWellness"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 20 && p.Active == true), "Lookupcode", "Description");
            ViewData["DEIAndWomenFriendlyPolicies"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 21 && p.Active == true), "Lookupcode", "Description");
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
            ViewData["FunctionalCategory"] = new SelectList(_context.MstLookups.Where(p=>p.Lookupflag == 14 && p.Active == true), "Lookupcode", "Description");
            ViewData["Cityid"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 55 && p.Active == true), "Lookupcode", "Description");
            ViewData["WDid"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 56  && p.Active == true), "Lookupcode", "Description");
            ViewData["ETid"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 57 && p.Active == true), "Lookupcode", "Description");
            ViewData["LeavePolicies"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 17 && p.Active == true), "Lookupcode", "Description");
            ViewData["FlexibleWorkOption"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 18 && p.Active == true), "Lookupcode", "Description");
            ViewData["LearningAndDevelopment"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 19 && p.Active == true), "Lookupcode", "Description");
            ViewData["HealthcareAndWellness"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 20 && p.Active == true), "Lookupcode", "Description");
            ViewData["DEIAndWomenFriendlyPolicies"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 21 && p.Active == true), "Lookupcode", "Description");
            return View(tbljob);
        }

        // POST: Tbljobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Tbljob tbljob)
        {
            if (id != tbljob.Jobid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    tbljob.Updatedat = DateTime.Now;
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
            ViewData["LeavePolicies"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 17 && p.Active == true), "Lookupcode", "Description");
            ViewData["FlexibleWorkOption"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 18 && p.Active == true), "Lookupcode", "Description");
            ViewData["LearningAndDevelopment"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 19 && p.Active == true), "Lookupcode", "Description");
            ViewData["HealthcareAndWellness"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 20 && p.Active == true), "Lookupcode", "Description");
            ViewData["DEIAndWomenFriendlyPolicies"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 21 && p.Active == true), "Lookupcode", "Description");
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

        //// POST: Tbljobs/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var tbljob = await _context.Tbljobs.FindAsync(id);
        //    if (tbljob != null)
        //    {
        //        _context.Tbljobs.Remove(tbljob);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var record = _context.Tbljobs.Find(id);
            if (record != null)
            {
                record.Isdeleted = true;  // Set the flag to true
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Record not found." });
        }
        private bool TbljobExists(int id)
        {
            return _context.Tbljobs.Any(e => e.Jobid == id);
        }





        [HttpPost]
        public async Task<IActionResult> FetchJobDetails(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    return Json(new { success = false, message = "URL is required" });
                }

                var jobData = await _jobScraperService.ScrapeJobDetails(url);

                if (jobData == null)
                {
                    return Json(new { success = false, message = "Could not extract job details from this URL" });
                }

                // Try to find matching company in database
                if (!string.IsNullOrEmpty(jobData.CompanyName))
                {
                    var company = await _context.Tblcompanies.Where(c => c.Name.Contains(jobData.CompanyName)).FirstOrDefaultAsync();
                  //  var company = "";

                    if (company != null)
                    {
                        jobData.CompanyId = company.Companyid;
                    }
                }

                return Json(new { success = true, data = jobData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
