
using EquidCMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EquidCMS.Controllers
{
    public class OurTeam : Controller
    {
        private readonly EquiDbContext _context;
        public OurTeam(EquiDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var existingRecords = _context.Tblourteams.Where(x => x.IsDeleted == null || x.IsDeleted == false).ToList();
            var model = new OurTeamViewModel
            {
                Records = existingRecords,
                NewEntry = new Tblourteam() // Initialize the model for the new entry form
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OurTeamViewModel model, IFormFile FileToUpload1)
        {
             // Handle the first file upload (Image)
                if (FileToUpload1 != null && FileToUpload1.Length > 0)
                {
                // Generate a unique file name to avoid conflicts
                var imageFileName = Path.GetFileNameWithoutExtension(FileToUpload1.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(FileToUpload1.FileName);
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "OurTeam", imageFileName);

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
                           await  FileToUpload1.CopyToAsync(stream);
                        }

                        // Save the full file path in the model (not just the file name)
                        model.NewEntry.Photo = "/assets/OurTeam/" + imageFileName; // Relative path for front-end usage
                }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("FileUploadError", "An error occurred while uploading the image: " + ex.Message);
                        return View(model.NewEntry);
                    }
                }
                // Save the new entry
                _context.Tblourteams.Add(model.NewEntry);
                _context.SaveChanges();

            
            // If the model is not valid, return to the Index view with the current model
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(OurTeamViewModel model, IFormFile FileToUpload1)
        {
            
                var record = _context.Tblourteams.Find(model.NewEntry.Id);
                if (record != null)
                {
                    record.Name = model.NewEntry.Name;
                    record.Linkedin = model.NewEntry.Linkedin;
                    record.Description = model.NewEntry.Description;
                    record.Organization = model.NewEntry.Organization;

                    if (FileToUpload1 != null && FileToUpload1.Length > 0)
                    {
                        var imageFileName = Path.GetFileNameWithoutExtension(FileToUpload1.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(FileToUpload1.FileName);
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "OurTeam", imageFileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                           await FileToUpload1.CopyToAsync(stream);
                        }
                        record.Photo = "/assets/OurTeam/" + imageFileName; // Relative path for front-end usage
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                return View(model);
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var record = _context.Tblourteams.Find(id);
            if (record != null)
            {
                record.IsDeleted = true;  // Set the flag to true
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Record not found." });
        }



    }
}
