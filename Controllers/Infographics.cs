using ClosedXML.Excel;
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
        [HttpPost]
        public IActionResult Export()
        {

            var applist = _context.Tblinfographics.Where(x => x.Isdeleted == null || x.Isdeleted == false).ToList();


            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Applicants");

                var header = worksheet.Range("A1:H1");
                header.Style.Fill.BackgroundColor = XLColor.LightBlue;
                header.Style.Font.Bold = true;
                header.Style.Font.FontColor = XLColor.DarkBlue;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                worksheet.Cell(1, 1).Value = "Info Heading";
                worksheet.Cell(1, 2).Value = "Infodesc";

                worksheet.Column(2).Width = 89; 
                worksheet.Column(3).Width = 90;


                for (int i = 0; i < applist.Count; i++)
                {
                    var row = worksheet.Row(i + 2);

                    row.Style.Fill.BackgroundColor = i % 2 == 0
                        ? XLColor.White
                        : XLColor.LightGray;

                    var applicant = applist[i];

                    worksheet.Cell(i + 2, 1).Value = applicant.Infoheading;
                    worksheet.Cell(i + 2, 2).Value = applicant.Infodesc;
                  
                }

                // Freeze header row
                worksheet.SheetView.FreezeRows(1);

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();

                    return File(
                        content,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "infographics.xlsx"
                    );
                }
            }
        }
    }
}
