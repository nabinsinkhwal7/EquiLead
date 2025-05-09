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

        [HttpGet]
        public JsonResult GetEmails()
        {
            var people = _context.Applicants
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
