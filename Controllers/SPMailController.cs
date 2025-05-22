using DocumentFormat.OpenXml.Wordprocessing;
using EquidCMS.Controllers;
using EquidCMS.Dto;
using EquidCMS.Models;
using EquidCMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Diagnostics;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FairtradePR.Controllers
{
    public class SPMailController : Controller
    {
        private readonly EquiDbContext _context;
        private readonly EmailService _service;

        private readonly ILogger<HomeController> _logger;
        public SPMailController(ILogger<SPMailController> logger, EquiDbContext context, EmailService service)
        {
            
            _context = context;
            _service = service;
        }

        public IActionResult Index()
        {

            if (Convert.ToString(HttpContext.Session.GetString("UserID")) == "")
            {
                return RedirectToAction("Login", "Logout");
            }
            return View();
        }

        public IActionResult AddGroup()
        {
            return View();
        }
        // Updated server-side controller
        [HttpGet]
        public JsonResult GetEmails(int page = 1, int pageSize = 50, string searchTerm = "")
        {
            var query = _context.Applicants
                .Where(x => !string.IsNullOrEmpty(x.Email));

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(x =>
                    x.Email.Contains(searchTerm) ||
                    x.FullName.Contains(searchTerm));
            }

            var mapped = query
                .Select(x => new SPMailModel
                {
                    Id = x.ApplicantId,
                    Name = x.FullName,
                    Email = x.Email
                })
                .ToList(); 

            var grouped = mapped
                .GroupBy(x => x.Email)
                .Select(g => g.First())
                .ToList();

            var totalCount = grouped.Count();

            var people = grouped
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Json(new
            {
                Data = people,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }


        //[HttpGet]
        //public JsonResult GetEmails()
        //{
        //    var people = _context.Applicants
        //        .Where(x => !string.IsNullOrEmpty(x.Email)) 
        //        .Select(x => new SPMailModel
        //        {
        //            Id = x.ApplicantId,
        //            Name = x.FullName,
        //            Email = x.Email
        //        })
        //        .GroupBy(x => x.Email)
        //        .Select(g => g.First())
        //        .ToList();

        //    return Json(people);
        //}

        [HttpGet]
        public JsonResult GetEmailsFilter(string employeeType,string fromExp, string toExp)
        {
            //var testDT = _context.Applicants.ToList();
            bool hasFromExp = int.TryParse(fromExp, out int from);
            bool hasToExp = int.TryParse(toExp, out int to);

            var people = _context.Applicants
                .Where(x => !string.IsNullOrEmpty(x.Email))
                .Where(x =>
                    (!hasFromExp || x.YearsOfExperence >= from) &&
                    (!hasToExp || x.YearsOfExperence <= to) &&
                    (string.IsNullOrEmpty(employeeType) ||
                     (x.ApplicantCareerPreference != null &&
                      x.ApplicantCareerPreference.EmploymentTypePreference == employeeType))
                )
                .Select(x => new SPMailModel
                {
                    Id = x.ApplicantId,
                    Name = x.FullName,
                    Email = x.Email
                })
                .GroupBy(x => x.Email)
                .Select(g => g.First())
                .ToList();

            return Json(people);
        }
        [HttpGet]
        public JsonResult GetLocation()
        {
            var emails = _context.Applicants.Where(l=>l.Location!=null).Select(l=>l.Location).Distinct().OrderBy(l=>l).ToList();
            return Json(emails);
        }
        //[HttpPost]
        //public async Task<JsonResult> SendEmail([FromBody] EmailRequestModel request)
        //{
        //    // Validate the request (optional)
        //    if (request?.Recipients == null || !request.Recipients.Any())
        //    {
        //        return Json(new { success = false, errorMessage = "No recipients selected." });
        //    }

        //    // Logic to send emails
        //    try
        //    {
        //        foreach (var recipient in request.Recipients)
        //        {
        //            bool isEmailSent = await _service.SendEmailAsync(recipient.Email, request.Title, request.Body);
        //            if (!isEmailSent)
        //            {
        //                return Json(new { success = false, errorMessage = "Failed to send to one or more recipients. Please try again!" });
        //            }
        //        }

        //        return Json(new { success = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, errorMessage = ex.Message });
        //    }
        //}

        [HttpPost]
        public async Task<JsonResult> SendEmail([FromBody] EmailRequestModel request)
        {
            if (request?.Recipients == null || !request.Recipients.Any())
            {
                return Json(new { success = false, errorMessage = "No recipients selected." });
            }

            var bccEmails = request.Recipients.Select(r => r.Email).ToArray();

            try
            {
                bool isEmailSent = await _service.SendEmailWithBCCAsync("letstalk@equilead.org", bccEmails, request.Title, request.Body);
                if (!isEmailSent)
                {
                    return Json(new { success = false, errorMessage = "Failed to send email. Please try again!" });
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }





    }
}
