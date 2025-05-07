using EquidCMS.Common;
using EquidCMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EquidCMS.Controllers
{
    public class Infographics : Controller
    {
        private readonly EquiDbContext _context;
        private readonly string _connectionString;

        public Infographics(EquiDbContext context, IConfiguration configuration)
        {
            _context = context;
            _connectionString = configuration.GetConnectionString("DbConnection");
        }

        public IActionResult Index()
        {
            var existingRecords = _context.Tblinfographics.Where(x => x.Isdeleted == null || x.Isdeleted == false).ToList();
            var model = new InfographicViewModel
            {
                Records = existingRecords,
                NewEntry = new Tblinfographic() // Initialize the model for the new entry form
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Create(InfographicViewModel model, IFormFile FileToUpload1)
        {
            if (FileToUpload1 != null && FileToUpload1.Length > 0)
            {
                var imageFileName = Path.GetFileNameWithoutExtension(FileToUpload1.FileName)
                                    + "_" + Guid.NewGuid()
                                    + Path.GetExtension(FileToUpload1.FileName);

                var imagePath = Path.Combine("wwwroot/assets/Infographic", imageFileName);
                var directoryPath = Path.GetDirectoryName(imagePath);

                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                try
                {
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await FileToUpload1.CopyToAsync(stream);
                    }

                    model.NewEntry.Infoimage = "/assets/Infographic/" + imageFileName;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("FileUploadError", "An error occurred while uploading the image: " + ex.Message);
                    return View(model.NewEntry);
                }
            }

            _context.Tblinfographics.Add(model.NewEntry);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        [HttpPost]
        public async Task<IActionResult> Edit(InfographicViewModel model, IFormFile FileToUpload1)
        {
            var record = await _context.Tblinfographics.FindAsync(model.NewEntry.Infogid);
            if (record != null)
            {
                record.Infoheading = model.NewEntry.Infoheading;
                record.Infoimage = model.NewEntry.Infoimage; // This might be overwritten below if a file is uploaded
                record.Infodesc=model.NewEntry.Infodesc;
                if (FileToUpload1 != null && FileToUpload1.Length > 0)
                {
                    var imageFileName = Path.GetFileNameWithoutExtension(FileToUpload1.FileName)
                                        + "_" + Guid.NewGuid().ToString()
                                        + Path.GetExtension(FileToUpload1.FileName);

                    var imagePath = Path.Combine("wwwroot/assets/Infographic", imageFileName);

                    // Ensure directory exists
                    var directory = Path.GetDirectoryName(imagePath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await FileToUpload1.CopyToAsync(stream);
                    }

                    record.Infoimage = "/assets/Infographic/" + imageFileName; // Front-end relative path
                }

                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(model);
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var record = _context.Tblinfographics.Find(id);
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
