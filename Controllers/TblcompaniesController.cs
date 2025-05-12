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
    public class TblcompaniesController : Controller
    {
        private readonly EquiDbContext _context;

        public TblcompaniesController(EquiDbContext context)
        {
            _context = context;
        }

        // GET: Tblcompanies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tblcompanies.Where(x=>x.Isdeleted==null || x.Isdeleted==false).ToListAsync());
        }

        // GET: Tblcompanies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblcompany = await _context.Tblcompanies
                .FirstOrDefaultAsync(m => m.Companyid == id);
            if (tblcompany == null)
            {
                return NotFound();
            }

            return View(tblcompany);
        }

        // GET: Tblcompanies/Create
        public IActionResult Create()
        {
            ViewData["LPid"] =  new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 17 && p.Active == true), "Lookupcode", "Description");
            ViewData["LDid"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 19 && p.Active == true), "Lookupcode", "Description");
            ViewData["HWid"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 60 && p.Active == true), "Lookupcode", "Description");
            ViewData["DWPid"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 61 && p.Active == true), "Lookupcode", "Description");
            return View();
        }

        // POST: Tblcompanies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Companyid,Name,Logo,Overview,Website,Sociallink,Wrl,Leavepolicieid,Learningdevelopment,Healthcarewellness,Dwfp")] Tblcompany tblcompany, IFormFile FileToUpload2)
        {
            if (ModelState.IsValid)
            {
                // Handle the second file upload (Document)
                if (FileToUpload2 != null && FileToUpload2.Length > 0)
                {
                    // Generate a unique file name to avoid conflicts
                    var documentFileName = Path.GetFileNameWithoutExtension(FileToUpload2.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(FileToUpload2.FileName);

                    // Define the full file path
                    //var documentPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "RSDocument", documentFileName);
                    var documentPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "Company", documentFileName);

                    // Ensure the directory exists
                    var documentDirectoryPath = Path.GetDirectoryName(documentPath);
                    if (!Directory.Exists(documentDirectoryPath))
                    {
                        Directory.CreateDirectory(documentDirectoryPath);
                    }

                    try
                    {
                        // Save the file to the specified path
                        using (var stream = new FileStream(documentPath, FileMode.Create))
                        {
                            await FileToUpload2.CopyToAsync(stream);
                        }

                        // Save the full file path in the model (not just the file name)
                            tblcompany.Logo = "assets/Company/" + documentFileName; 
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("FileUploadError", "An error occurred while uploading the document: " + ex.Message);
                        return View(tblcompany);
                    }
                }
                _context.Add(tblcompany);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblcompany);
        }

        // GET: Tblcompanies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblcompany = await _context.Tblcompanies.FindAsync(id);
            if (tblcompany == null)
            {
                return NotFound();
            }
            return View(tblcompany);
        }

        // POST: Tblcompanies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Companyid,Name,Logo,Overview,Website,Sociallink,Wrl,Leavepolicieid,Learningdevelopment,Healthcarewellness,Dwfp")] Tblcompany tblcompany, IFormFile FileToUpload2)
        {
            if (id != tblcompany.Companyid)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (FileToUpload2 != null && FileToUpload2.Length > 0)
                    {
                        // Generate a unique file name to avoid conflicts
                        var documentFileName = Path.GetFileNameWithoutExtension(FileToUpload2.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(FileToUpload2.FileName);

                        // Define the full file path
                        //var documentPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "RSDocument", documentFileName);
                        var documentPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "Company", documentFileName);

                        // Ensure the directory exists
                        var documentDirectoryPath = Path.GetDirectoryName(documentPath);
                        if (!Directory.Exists(documentDirectoryPath))
                        {
                            Directory.CreateDirectory(documentDirectoryPath);
                        }

                        try
                        {
                            // Save the file to the specified path
                            using (var stream = new FileStream(documentPath, FileMode.Create))
                            {
                                await FileToUpload2.CopyToAsync(stream);
                            }

                            // Save the full file path in the model (not just the file name)
                            tblcompany.Logo = "assets/Company/" + documentFileName; 
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("FileUploadError", "An error occurred while uploading the document: " + ex.Message);
                            return View(tblcompany);
                        }
                    }
                    _context.Update(tblcompany);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblcompanyExists(tblcompany.Companyid))
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
            return View(tblcompany);
        }

        // GET: Tblcompanies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblcompany = await _context.Tblcompanies
                .FirstOrDefaultAsync(m => m.Companyid == id);
            if (tblcompany == null)
            {
                return NotFound();
            }

            return View(tblcompany);
        }

        //// POST: Tblcompanies/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var tblcompany = await _context.Tblcompanies.FindAsync(id);
        //    if (tblcompany != null)
        //    {
        //        _context.Tblcompanies.Remove(tblcompany);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var record = _context.Tblcompanies.Find(id);
            if (record != null)
            {
                record.Isdeleted = true;  // Set the flag to true
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Record not found." });
        }
        private bool TblcompanyExists(int id)
        {
            return _context.Tblcompanies.Any(e => e.Companyid == id);
        }
    }
}
