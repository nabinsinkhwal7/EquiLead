
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
using Microsoft.CodeAnalysis;

namespace EquidCMS.Controllers
{
    
    public class Event : Controller
    {
        private readonly EquiDbContext _context;
        public Event(EquiDbContext context, IConfiguration configuration)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var RSList = _context.Tblevents.OrderByDescending(p => p.Enddateofevent).Include(d => d.Tbleventparticipants).Include(d => d.Tblevidences).Where(x=>x.Isdeleted==null || x.Isdeleted==false).ToList();
            return View(RSList);
        }

        // GET: Tblresources/Create
        public IActionResult Create()
        {
            ViewData["ThemeId"] = new SelectList(_context.MstThemes, "ThemeId", "Theme");
            ViewData["EPRID"] = new SelectList(_context.MstEventpricings, "Eventpricingtypeid", "Eventpricingtype");
            ViewData["EVTYID"] = new SelectList(_context.MstEventtypes, "Eventtypeid", "Eventtype");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Tblevent tblevent, List<string> eventBenefits, IFormFile FileToUpload1, IFormFile FileToUpload2)
        {
            //if (ModelState.IsValid)
            //{
            // Add benefits if available

            if (tblevent.Eventid > 0)
            {

                var existingEvent = await _context.Tblevents.FindAsync(tblevent.Eventid);
                if (existingEvent == null)
                {
                    ModelState.AddModelError("EventNotFound", "The event you are trying to edit does not exist.");
                    return View(tblevent);
                }

                // Update properties of existing event
                existingEvent.EventName = tblevent.EventName;
                existingEvent.Startdateofevent = tblevent.Startdateofevent;
                existingEvent.Enddateofevent = tblevent.Enddateofevent;
                existingEvent.Descriptionofevent = tblevent.Descriptionofevent;
                existingEvent.EventTimeStart = tblevent.EventTimeStart;
                existingEvent.EventTimeEnd = tblevent.EventTimeEnd;
                existingEvent.Numberofattended = tblevent.Numberofattended;
                existingEvent.Themeid = tblevent.Themeid;
                existingEvent.EventLink = tblevent.EventLink;
                existingEvent.EventRegistrationLink = tblevent.EventRegistrationLink;
                existingEvent.EventPricingTypeId = tblevent.EventPricingTypeId;
                existingEvent.EventSpeaker = tblevent.EventSpeaker;
                existingEvent.EventSpeakerOrg = tblevent.EventSpeakerOrg;
                existingEvent.Parking = tblevent.Parking;
                existingEvent.EventHost = tblevent.EventHost;
                existingEvent.EventHostOrg = tblevent.EventHostOrg;
                existingEvent.WheelChair = tblevent.WheelChair;
                existingEvent.Eventbenefit = tblevent.Eventbenefit;
                existingEvent.EventTypeId = tblevent.EventTypeId;
                existingEvent.EventPricing = tblevent.EventPricing;
                existingEvent.EventVenue = tblevent.EventVenue;

                // Handle the second file upload (Document)
                if (FileToUpload2 != null && FileToUpload2.Length > 0)
                {
                    // If an old file exists, delete it
                    if (!string.IsNullOrEmpty(existingEvent.EventAgendaDoc))
                    {
                        // The old file path
                        var oldFilePath = existingEvent.EventAgendaDoc;

                        // Check if the file exists on the server
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            try
                            {
                                // Delete the old file
                                System.IO.File.Delete(oldFilePath);
                            }
                            catch (Exception ex)
                            {
                                // Handle error while deleting the file
                                ModelState.AddModelError("FileDeleteError", "An error occurred while deleting the old file: " + ex.Message);
                                return View(tblevent);
                            }
                        }
                    }

                    // Generate a unique file name for the new file
                    var documentFileName = Path.GetFileNameWithoutExtension(FileToUpload2.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(FileToUpload2.FileName);

                    // Define the new file path
                    //var documentPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "EventAgendaDocument", documentFileName);
                    
                    var documentPath = Path.Combine("wwwroot/assets/EventAgendaDocument", documentFileName);
                    // Ensure the directory exists
                    var documentDirectoryPath = Path.GetDirectoryName(documentPath);
                    if (!Directory.Exists(documentDirectoryPath))
                    {
                        Directory.CreateDirectory(documentDirectoryPath);
                    }

                    try
                    {
                        // Save the new file to the specified path
                        using (var stream = new FileStream(documentPath, FileMode.Create,FileAccess.Write))
                        {
                            await FileToUpload2.CopyToAsync(stream);
                        }
                        var relativePath = Path.Combine("assets", "EventAgendaDocument", documentFileName);
                        // Save the new file path in the model
                        existingEvent.EventAgendaDoc = relativePath;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("FileUploadError", "An error occurred while uploading the document: " + ex.Message);
                        return View(tblevent);
                    }
                }
                // Handle the second file upload (Document)
                if (FileToUpload1 != null && FileToUpload1.Length > 0)
                {
                    // If an old file exists, delete it
                    if (!string.IsNullOrEmpty(existingEvent.EventBanner))
                    {
                        // The old file path
                        var oldFilePath = existingEvent.EventBanner;

                        // Check if the file exists on the server
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            try
                            {
                                // Delete the old file
                                System.IO.File.Delete(oldFilePath);
                            }
                            catch (Exception ex)
                            {
                                // Handle error while deleting the file
                                ModelState.AddModelError("FileDeleteError", "An error occurred while deleting the old file: " + ex.Message);
                                return View(tblevent);
                            }
                        }
                    }

                    // Generate a unique file name for the new file
                    var documentFileName = Path.GetFileNameWithoutExtension(FileToUpload1.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(FileToUpload1.FileName);

                    // Define the new file path
                    //var documentPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "img", documentFileName);
                    var documentPath = Path.Combine("wwwroot/assets/img", documentFileName);

                    // Ensure the directory exists
                    var documentDirectoryPath = Path.GetDirectoryName(documentPath);
                    if (!Directory.Exists(documentDirectoryPath))
                    {
                        Directory.CreateDirectory(documentDirectoryPath);
                    }

                    try
                    {
                        // Save the new file to the specified path
                        using (var stream = new FileStream(documentPath, FileMode.Create))
                        {
                            await FileToUpload1.CopyToAsync(stream);
                        }

                        var relativePath = Path.Combine("assets", "img", documentFileName);
                        existingEvent.EventBanner = relativePath;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("FileUploadError", "An error occurred while uploading the document: " + ex.Message);
                        return View(tblevent);
                    }
                }

               
                if (eventBenefits != null && eventBenefits.Any()) 
                { 
                var productToDelete = _context.Tbleventbenefits.Where(p=>p.Eventid == existingEvent.Eventid).ToList();
                _context.Tbleventbenefits.RemoveRange(productToDelete);
                    // Save changes to the database
                _context.SaveChanges();
                   
                foreach (var benefitDescription in eventBenefits)
                {
                        if (string.IsNullOrWhiteSpace(benefitDescription))
                            continue;
                        var eventBenefit = new Tbleventbenefit
                    {
                        Eventbenefit = benefitDescription,
                        Eventid = existingEvent.Eventid  // Link benefit to the event
                    };
                    existingEvent.Tbleventbenefits.Add(eventBenefit);
                }
                }
            
                _context.Update(existingEvent);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));

            }
            else 
            {
                // Handle the second file upload (Document)
                if (FileToUpload1 != null && FileToUpload1.Length > 0)
                {
                    

                    // Generate a unique file name for the new file
                    var documentFileName = Path.GetFileNameWithoutExtension(FileToUpload1.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(FileToUpload1.FileName);

                    // Define the new file path
                    //var documentPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "img", documentFileName);
                    var documentPath = Path.Combine("wwwroot/assets/img", documentFileName);
                    // Ensure the directory exists
                    var documentDirectoryPath = Path.GetDirectoryName(documentPath);
                    if (!Directory.Exists(documentDirectoryPath))
                    {
                        Directory.CreateDirectory(documentDirectoryPath);
                    }

                    try
                    {
                        // Save the new file to the specified path
                        using (var stream = new FileStream(documentPath, FileMode.Create))
                        {
                            await FileToUpload1.CopyToAsync(stream);
                        }
                        var relativePath = Path.Combine("assets", "img", documentFileName);
                        // Save the new file path in the model
                        tblevent.EventBanner = relativePath;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("FileUploadError", "An error occurred while uploading the document: " + ex.Message);
                        return View(tblevent);
                    }
                }
                // Handle the second file upload (Document)
                if (FileToUpload2 != null && FileToUpload2.Length > 0)
                {
                    // Generate a unique file name to avoid conflicts
                    var documentFileName = Path.GetFileNameWithoutExtension(FileToUpload2.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(FileToUpload2.FileName);

                    // Define the full file path
                    //var documentPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "EventAgendaDocument", documentFileName);
                    var documentPath = Path.Combine("wwwroot/assets/EventAgendaDocument", documentFileName);
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
                        var relativePath = Path.Combine("assets", "EventAgendaDocument", documentFileName);
                        // Save the full file path in the model (not just the file name)
                        tblevent.EventAgendaDoc = relativePath;
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("FileUploadError", "An error occurred while uploading the document: " + ex.Message);
                        return View(tblevent);
                    }
                }

                if (eventBenefits != null && eventBenefits.Any())
                {
                    foreach (var benefitDescription in eventBenefits)
                    {
                        if (benefitDescription != null || benefitDescription != "")
                        {
                        var eventBenefit = new Tbleventbenefit
                        {
                            Eventbenefit = benefitDescription,
                            Eventid = tblevent.Eventid  // Link the benefit to the event
                        };
                        tblevent.Tbleventbenefits.Add(eventBenefit);
                    }
                }
                }

                // Save the event and benefits to the database
                _context.Add(tblevent);
                await _context.SaveChangesAsync();

                //return RedirectToAction(nameof(Index)); // Redirect to the event list page

            }


            return RedirectToAction("Index");
        }


        //dotnet ef dbcontext scaffold "Host=localhost;Database=EquiDB;Username=postgres;Password=1234" Npgsql.EntityFrameworkCore.PostgreSQL --output-dir Models --context ApplicationDbContext --force

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEvidence(int EventID, List<IFormFile>? EvidenceFiles, string? VideoLink)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(VideoLink))
                {
                    // Save the video link as evidence
                    var evidence = new Tblevidence
                    {
                        Eventid = EventID,
                        Evidencelink = VideoLink.Trim()
                    };
                    _context.Tblevidences.Add(evidence);
                }

                foreach (var file in EvidenceFiles)
                {
                    if (file.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        var relativePath = Path.Combine("assets", "Evidence", fileName);
                        var fullFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

                        using (var stream = new FileStream(fullFilePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        var evidence = new Tblevidence
                        {
                            Eventid = EventID,
                            Evidencepath = relativePath
                        };

                        _context.Tblevidences.Add(evidence);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(EvidenceGallery), new { id = EventID });
            }

            return View();
        }

        // GET: Tblevents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblevent = await _context.Tblevents.Include(e => e.Tbleventbenefits).FirstOrDefaultAsync(e => e.Eventid == id);
            if (tblevent == null)
            {
                return NotFound();
            }
            ViewData["EPRID"] = new SelectList(_context.MstEventpricings, "Eventpricingtypeid", "Eventpricingtype", tblevent.EventPricingTypeId);
            ViewData["EVTYID"] = new SelectList(_context.MstEventtypes, "Eventtypeid", "Eventtype", tblevent.EventTypeId);
            ViewData["ThemeId"] = new SelectList(_context.MstThemes, "ThemeId", "Theme", tblevent.Themeid);
            return View(tblevent);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var record = _context.Tblevents.Find(id);
            if (record != null)
            {
                record.Isdeleted = true;  // Set the flag to true
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Record not found." });
        }
        [HttpPost]
        public IActionResult DeleteEvidence(int id)
        {
            var evidence = _context.Tblevidences.FirstOrDefault(e => e.Evidenceid == id);
            if (evidence != null)
            {
                _context.Tblevidences.Remove(evidence);
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }
        public IActionResult EvidenceGallery(int id)
        {
            var eventDetails = _context.Tblevents
                .Include(e => e.Tblevidences)
                .FirstOrDefault(e => e.Eventid == id);

            if (eventDetails == null)
            {
                return NotFound();
            }

            return View(eventDetails);
        }
    }
}
