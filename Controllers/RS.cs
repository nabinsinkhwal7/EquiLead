using EquidCMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Npgsql;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EquidCMS.Controllers
{
    
    public class RS : Controller
    {
        private readonly EquiDbContext _context;
        private readonly string _connectionString;
        public RS(EquiDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DbConnection");
        }
        public IActionResult Index()
        {
            var RSList = _context.Tblresources.Where(x=>x.Isdeleted==false).ToList();
            return View(RSList);
        }

        // GET: Tblresources/Create
        public IActionResult Create()
        {
            ViewData["Categories"] = new SelectList(_context.MstLookups.Where(p=>p.Lookupflag == 15), "Lookupcode", "Description");
            ViewData["ThemeId"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 16), "Lookupcode", "Description");
            ViewData["Rsdocumenttypeid"] = new SelectList(_context.MstRsdocumenttypes, "Rsdocumenttypeid", "Rsdocumenttype");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tblresource tblresource, IFormFile? FileToUpload1, IFormFile? FileToUpload2)
        {
            if (ModelState.IsValid)
            {
                // Handle the first file upload (Image)
                if (FileToUpload1 != null && FileToUpload1.Length > 0)
                {
                    // Generate a unique file name to avoid conflicts
                    var imageFileName = Path.GetFileNameWithoutExtension(FileToUpload1.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(FileToUpload1.FileName);

                    // Define the full file path
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "RSImage", imageFileName);
                    var imageRelativePath = Path.Combine("/assets/RSImage", imageFileName);

                    // Ensure the directory exists
                    var directoryPath = Path.GetDirectoryName(imagePath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    try
                    {
                        // Save the file to the specified path
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await FileToUpload1.CopyToAsync(stream);
                        }

                        // Save the full file path in the model (not just the file name)
                        tblresource.Rsimage = imageRelativePath;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("FileUploadError", "An error occurred while uploading the image: " + ex.Message);
                        return View(tblresource);
                    }
                }

                // Handle the second file upload (Document)
                if (FileToUpload2 != null && FileToUpload2.Length > 0)
                {
                    // Generate a unique file name to avoid conflicts
                    var documentFileName = Path.GetFileNameWithoutExtension(FileToUpload2.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(FileToUpload2.FileName);

                    // Define the full file path
                    var documentPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "RSDocument", documentFileName);
                    var documentRelativePath = Path.Combine("/assets/RSDocument", documentFileName);

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
                        tblresource.Rsdocument = documentRelativePath;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("FileUploadError", "An error occurred while uploading the document: " + ex.Message);
                        return View(tblresource);
                    }
                }
                if (tblresource.Resourceid > 0)
                {
                    _context.Update(tblresource);
                }
                else { 
                // Save the tblresource to the database
                _context.Add(tblresource);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Populate the ViewBag for the Theme dropdown
            ViewData["Rsdocumenttypeid"] = new SelectList(_context.MstRsdocumenttypes, "Rsdocumenttypeid", "Rsdocumenttype");
            ViewData["ThemeId"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 16), "Lookupcode", "Description");
            return View(tblresource);
        }


        // GET: Tblresources/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblresource = await _context.Tblresources.FirstOrDefaultAsync(e => e.Resourceid == id);
            if (tblresource == null)
            {
                return NotFound();
            }
            ViewData["Categories"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 15), "Lookupcode", "Description");
            ViewData["ThemeId"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 16), "Lookupcode", "Description");
            ViewData["Rsdocumenttypeid"] = new SelectList(_context.MstRsdocumenttypes, "Rsdocumenttypeid", "Rsdocumenttype");
            return View(tblresource);
        }
        public IActionResult ExportToExcel()
        {
             var resources = _context.ResourceExports.FromSqlRaw("SELECT * FROM GetResourcesForExport()").ToList();
            if (!resources.Any())
                return BadRequest("No records found for export");
            byte[] fileContent = ExcelExportHelper.ExportToExcel(resources, "Resources");
            return File(fileContent, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Resources.xlsx");
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var record = _context.Tblresources.Find(id);
            if (record != null)
            {
                record.Isdeleted = true;  // Set the flag to true
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Record not found." });
        }

    }
}
