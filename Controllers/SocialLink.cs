
using EquidCMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EquidCMS.Controllers
{
    public class SocialLink : Controller
    {
        private readonly EquiDbContext _context;
        private readonly string _connectionString;

        public SocialLink(EquiDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DbConnection");
        }
        public IActionResult Index()
        {
            var existingRecords = _context.TblSocialLinkdins.Where(x=>x.IsDeleted==null || x.IsDeleted == false).ToList();
            var model = new SociallinkViewModel
            {
                Records = existingRecords,
                NewEntry = new TblSocialLinkdin() // Initialize the model for the new entry form
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Create(SociallinkViewModel model, IFormFile FileToUpload1)
        {
            // Handle the first file upload (Image)
            if (FileToUpload1 != null && FileToUpload1.Length > 0)
            {

                // Generate unique filename
                var imageFileName = $"{Path.GetFileNameWithoutExtension(FileToUpload1.FileName)}_{Guid.NewGuid()}{Path.GetExtension(FileToUpload1.FileName)}";
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "SocialPhotos", imageFileName);

                // Ensure directory exists
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
                        FileToUpload1.CopyToAsync(stream);
                    }

                    // Save the full file path in the model (not just the file name)
                    model.NewEntry.Photo = $"/assets/SocialPhotos/{imageFileName}";
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("FileUploadError", "An error occurred while uploading the image: " + ex.Message);
                    return View(model.NewEntry);
                }
            }
            // Save the new entry
            _context.TblSocialLinkdins.Add(model.NewEntry);
            _context.SaveChanges();

            
            // If the model is not valid, return to the Index view with the current model
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SociallinkViewModel model, IFormFile FileToUpload1)
        {
            if (model?.NewEntry == null)
            {
                return Json(new { success = false, message = "Invalid data." });
            }

            var record = await _context.TblSocialLinkdins.FindAsync(model.NewEntry.ScLinkdinId);
            if (record == null)
            {
                return Json(new { success = false, message = "Record not found." });
            }

            // Update text fields
            record.Name = model.NewEntry.Name;
            record.Email = model.NewEntry.Email;
            record.Post = model.NewEntry.Post;
            record.Organization = model.NewEntry.Organization;
            record.Linkdinlink = model.NewEntry.Linkdinlink;

            // Handle file upload (if a new file is uploaded)
            if (FileToUpload1 != null && FileToUpload1.Length > 0)
            {
                // Generate unique filename
                var imageFileName = $"{Path.GetFileNameWithoutExtension(FileToUpload1.FileName)}_{Guid.NewGuid()}{Path.GetExtension(FileToUpload1.FileName)}";
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "SocialPhotos", imageFileName);

                // Ensure directory exists
                var directoryPath = Path.GetDirectoryName(imagePath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                try
                {
                    // Save new file asynchronously
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await FileToUpload1.CopyToAsync(stream);
                    }

                    // Optionally delete the old image (to save storage)
                    if (!string.IsNullOrEmpty(record.Photo))
                    {
                        var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", record.Photo.TrimStart('/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // Update record with new image path
                    record.Photo = $"/assets/SocialPhotos/{imageFileName}";
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = "Error uploading file: " + ex.Message });
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var record = await _context.TblSocialLinkdins.FindAsync(id);
            if (record == null)
            {
                return Json(new { success = false, message = "Record not found." });
            }

            // Soft delete by setting IsDeleted = true
            record.IsDeleted = true;
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


    }
}
