using EquidCMS.Dto;
using EquidCMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EquidCMS.Controllers
{
    public class MstLookupController : Controller
    {
        private readonly EquiDbContext _context;

        public MstLookupController(EquiDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string selectedHintDetails)
        {
            // Fetch distinct HintDetails for the dropdown
            var hintDetails = _context.MstLookups
                .Where(l => l.Active) // Filtering only active records
                .Select(l => l.Hintdetails)
                .Distinct()
                .OrderBy(h => h)
                .ToList();

            // If no category is selected, return an empty list
            var lookupData = new List<MstLookup>();

            if (!string.IsNullOrEmpty(selectedHintDetails))
            {
                // Fetch data based on the selected HintDetails
                lookupData = _context.MstLookups
                    .Where(l => l.Hintdetails == selectedHintDetails )
                    .ToList();
            }

            // Pass the data to the view
            ViewBag.HintDetails = hintDetails;
            ViewBag.SelectedHintDetails = selectedHintDetails;  // Pass the selected filter value to the view
            return View(lookupData);
        }


        [HttpPost]
        public IActionResult Create([FromBody] LookupCreateDto model)
        {
            if (model == null || string.IsNullOrEmpty(model.selectedHintDetails) || string.IsNullOrEmpty(model.Description))
            {
                TempData["Message"] = "Invalid data received.";
                TempData["MessageType"] = "error";
                return RedirectToAction(nameof(Index));
            }

            var latestLookup = _context.MstLookups
                                       .Where(l => l.Hintdetails == model.selectedHintDetails)
                                       .OrderByDescending(l => l.Lookupcode)
                                       .ThenByDescending(l => l.Seqno)
                                       .FirstOrDefault();

            if (latestLookup != null)
            {
                var newMstLookup = new MstLookup
                {
                    Lookupflag = latestLookup.Lookupflag,
                    Lookupcode = (latestLookup.Lookupcode ) + 1,
                    Languageid = latestLookup.Languageid,
                    Hintdetails = model.selectedHintDetails,
                    Seqno = (latestLookup.Seqno ?? 0) + 1,
                    Active = true,
                    Description = model.Description
                };

                _context.Add(newMstLookup);
                _context.SaveChanges();

                TempData["Message"] = "Lookup added successfully!";
                TempData["MessageType"] = "success";
            }
            else
            {
                TempData["Message"] = "Error adding lookup. Please try again.";
                TempData["MessageType"] = "error";
            }

            return Json(new { success = true });
        }
        [HttpPost]
        public IActionResult ToggleActive(LookupStatusUpdateDTO model)
        {
            var lookup = _context.MstLookups
                                 .FirstOrDefault(l => l.Lookupcode == model.lookupCode && l.Hintdetails == model.selectedHintDetails);

            if (lookup == null)
            {
                return Json(new { success = false, message = "Lookup not found!" });
            }

            lookup.Active = !model.isActive; // Toggle status
            _context.SaveChanges();

            return Json(new { success = true, newStatus = lookup.Active ? "✅ Active" : "❌ Inactive" });
        }

        [HttpPost]
        public IActionResult Update([FromBody] LookupUpdateDTO model)
        {
            if (model.lookupCode == 0)
            {
                return Json(new { success = false, message = "Invalid Lookup Code!" });
            }

            var lookup = _context.MstLookups.FirstOrDefault(l => l.Lookupcode == model.lookupCode && l.Hintdetails==model.selectedHintDetails);
            if (lookup == null)
            {
                return Json(new { success = false, message = "Lookup not found!" });
            }

            lookup.Description = model.description;
            _context.SaveChanges();

            return Json(new { success = true });
        }


    }
}
