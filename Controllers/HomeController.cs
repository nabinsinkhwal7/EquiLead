using EquidCMS.Models;
using EquidCMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EquidCMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly EquiDbContext _context;

        private readonly EmailService _service;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, EquiDbContext context,EmailService service)
        {
            _logger = logger;
            _context = context;
            _service = service;
        }
        public IActionResult Index()
        {
            // Get total clicks count
            var totalClicks = _context.ResourceClickLogs.Count();

            var topicLookup = _context.MstLookups
                .Where(p => p.Lookupflag == 16)
                .ToDictionary(p => p.Lookupcode, p => p.Description);

            var categoryLookup = _context.MstLookups
                .Where(p => p.Lookupflag == 15)
                .ToDictionary(p => p.Lookupcode, p => p.Description);

            var clicksByTopic = topicLookup.Select(t => new
            {
                Id = t.Key,
                Name = t.Value,
                ClickCount = _context.ResourceClickLogs
                    .Join(_context.Tblresources, c => c.ResourceId, r => r.Resourceid, (c, r) => new { c, r })
                    .Where(x => x.r.Topic.Contains(t.Key))
                    .Count()
            }).Where(x => x.ClickCount > 0).ToList();

            var clicksByCategory = categoryLookup.Select(c => new
            {
                Id = c.Key,
                Name = c.Value,
                ClickCount = _context.ResourceClickLogs
                    .Join(_context.Tblresources, c => c.ResourceId, r => r.Resourceid, (c, r) => new { c, r })
                    .Where(x => x.r.CategoryId == c.Key)
                    .Count()
            }).Where(x => x.ClickCount > 0).ToList();

            ViewBag.TotalClicks = totalClicks;
            ViewBag.ClicksByTopic = clicksByTopic;
            ViewBag.ClicksByCategory = clicksByCategory;

            return View();
        }

        [HttpGet]
        public JsonResult GetDrilldown(string type, int id)
        {
            var query = _context.ResourceClickLogs
                .Join(_context.Tblresources, c => c.ResourceId, r => r.Resourceid, (c, r) => new { c, r });

            if (type == "topic")
            {
                var result = query
                    .Where(x => x.r.Topic.Contains(id))
                    .GroupBy(x => x.r.Rshead)
                    .Select(g => new { name = g.Key, clickCount = g.Count() })
                    .OrderByDescending(x => x.clickCount)
                    .ToList();

                return Json(result);
            }
            else if (type == "category")
            {
                var result = query
                    .Where(x => x.r.CategoryId == id)
                    .GroupBy(x => x.r.Rshead)
                    .Select(g => new { name = g.Key, clickCount = g.Count() })
                    .OrderByDescending(x => x.clickCount)
                    .ToList();

                return Json(result);
            }

            return Json(new List<object>());
        }

        public IActionResult landingpagenw()
        {
            // Get the existing landing page data
            Tbllandingpage landingPage = _context.Tbllandingpages.FirstOrDefault() ?? new Tbllandingpage();
            ViewData["Jobs"] = _context.Tbljobs.Include(p=>p.Company).Where(x=>x.Isdeleted==null || x.Isdeleted==false).OrderByDescending(x=>x.Createdat).Take(3).ToList();
            ViewData["Events"] = _context.Tblevents.OrderByDescending(p => p.Enddateofevent).Where(x => x.Isdeleted == null || x.Isdeleted == false).Where(p => p.Startdateofevent > DateOnly.FromDateTime(DateTime.Now)).Take(4).ToList();
            ViewData["Linkedinuser"] = _context.TblSocialLinkdins.Where(x => x.IsDeleted == null || x.IsDeleted == false).ToList();
            ViewData["Successsyory"] = _context.Tblsuccesstests.Where(x => x.IsDeleted == null || x.IsDeleted == false).ToList();

            ViewData["infographic"] = _context.Tblinfographics.Where(x => x.Isdeleted == null || x.Isdeleted == false).ToList();

            ViewData["Cityid"] = _context.MstLookups.Where(p => p.Lookupflag == 55 && p.Active == true).ToList();
            ViewData["WDid"] = _context.MstLookups.Where(p => p.Lookupflag == 56 && p.Active == true).ToList();
            ViewData["ETid"] = _context.MstLookups.Where(p => p.Lookupflag == 57 && p.Active == true).ToList();

            var totalRows = _context.TblSocialLinkdins.Where(p=>p.IsDeleted==false).Count();
            var rowsPerSet = totalRows / 3;
            if(rowsPerSet < 3)
            {
                ViewData["firstSet"] = _context.TblSocialLinkdins.Where(p => p.IsDeleted == false).OrderBy(x => x.ScLinkdinId).Take(1).ToList();
                ViewData["secondSet"] = _context.TblSocialLinkdins.Where(p => p.IsDeleted == false).OrderBy(x => x.ScLinkdinId)
                                            .Skip(1)
                                            .Take(1).ToList();
                ViewData["thirdSet"] = _context.TblSocialLinkdins.Where(p => p.IsDeleted == false).OrderBy(x => x.ScLinkdinId)
                                                 .Skip(2 * 1)
                                                 .Take(1).ToList();
            }
            else
            {
                ViewData["firstSet"] = _context.TblSocialLinkdins.Where(p => p.IsDeleted == false).OrderBy(x => x.ScLinkdinId).Take(rowsPerSet).ToList();
                ViewData["secondSet"] = _context.TblSocialLinkdins.Where(p => p.IsDeleted == false).OrderBy(x => x.ScLinkdinId)
                                              .Skip(rowsPerSet)
                                              .Take(rowsPerSet).ToList();
                ViewData["thirdSet"] = _context.TblSocialLinkdins.Where(p => p.IsDeleted == false).OrderBy(x => x.ScLinkdinId)
                                                 .Skip(2 * rowsPerSet)
                                                 .Take(totalRows - 2 * rowsPerSet).ToList();
            }
           
            

            return View(landingPage);
        }

        public IActionResult events(int pageUpcoming = 1, int pagePast = 1, int pageSize = 8)
        {
            // UPCOMING EVENTS
            var allUpcoming = _context.Tblevents
                .Where(p => p.Startdateofevent > DateOnly.FromDateTime(DateTime.Now))
                .Where(x => x.Isdeleted == null || x.Isdeleted == false)
                .Include(d => d.Theme)
                .Include(d => d.EventPricingType)
                .Include(d => d.EventType)
                .OrderBy(p => p.Enddateofevent);

            var pagedUpcoming = allUpcoming
                .Skip((pageUpcoming - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalUpcoming = allUpcoming.Count();
            ViewBag.PageUpcoming = pageUpcoming;

            // PAST EVENTS
            var allPast = _context.Tblevents
                .Where(p => p.Startdateofevent < DateOnly.FromDateTime(DateTime.Now))
                .Where(x => x.Isdeleted == null || x.Isdeleted == false)
                .Include(d => d.Theme)
                .Include(d => d.EventPricingType)
                .Include(d => d.EventType)
                .OrderByDescending(p => p.Enddateofevent);

            var pagedPast = allPast
                .Skip((pagePast - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPast = allPast.Count();
            ViewBag.PagePast = pagePast;

            ViewBag.Community = _context.TblSocialLinkdins
                .Where(p => p.IsDeleted == false)
                .OrderBy(x => x.ScLinkdinId)
                .ToList();

            ViewBag.PastEvent = pagedPast;
            return View(pagedUpcoming); // upcoming events go into the model
        }

        public IActionResult RSGallery(string searchKeyword, int? selectedCategory, int? selectedTopic, int page = 1)
        {
            int pageSize = 9;

            if (page <= 0)
                page = 1;

            var query = _context.Tblresources
                .Where(x => !x.Isdeleted && x.Isverified!=null && x.Isverified==true)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchKeyword))
            {
                var keywordLower = searchKeyword.ToLower();
                query = query.Where(x => x.Rshead != null && x.Rshead.ToLower().Contains(keywordLower));
            }

            if (selectedCategory.HasValue)
            {
                query = query.Where(x => x.CategoryId == selectedCategory.Value);
            }

            if (selectedTopic.HasValue)
            {
                query = query.Where(x => x.Topic != null && x.Topic.Contains(selectedTopic.Value));
            }

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            if (page > totalPages && totalPages > 0)
                page = 1;

            var pagedResources = query
                .OrderByDescending(x => x.Resourceid)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CategoryRS = _context.MstLookups
                .Where(p => p.Lookupflag == 15)
                .OrderBy(x => x.Seqno)
                .ToList();

            ViewBag.TopicRS = _context.MstLookups
                .Where(p => p.Lookupflag == 16)
                .OrderBy(x => x.Seqno)
                .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.SearchKeyword = searchKeyword;
            ViewBag.SelectedCategory = selectedCategory;
            ViewBag.SelectedTopic = selectedTopic;

            return View(pagedResources);
        }

        public IActionResult Aboutus()
        {
            var data=_context.Tblaboutus.FirstOrDefault();
            ViewData["OurTeam"] = _context.Tblourteams.Where(x=>x.Photo!=null && x.Description!=null).ToList();
            ViewData["Successsyory"] = _context.Tblsuccesstests.Where(x => x.IsDeleted == null || x.IsDeleted == false).ToList();
            return View(data);
        }

        public IActionResult Talent()
        {
            ViewData["Comp"] = _context.Tblcompanies.Where(x => x.Isdeleted == null || x.Isdeleted == false).OrderBy(x => x.Name).ToList();
            ViewData["Cityid"] = _context.MstLookups.Where(p => p.Lookupflag == 55 && p.Active == true).ToList();
            ViewData["WDid"] = _context.MstLookups.Where(p => p.Lookupflag == 56 && p.Active == true).ToList();
            ViewData["ETid"] = _context.MstLookups.Where(p => p.Lookupflag == 57 && p.Active == true).ToList();
            ViewData["FCid"] = _context.MstLookups.Where(p => p.Lookupflag == 14 && p.Active == true).ToList();

            var totalJobs = _context.Tbljobs.Include(p => p.Company).Where(x => x.Isdeleted == null || x.Isdeleted == false).OrderByDescending(x => x.Createdat).ToList().Count;

            var totalPages = (int)Math.Ceiling(totalJobs / (double)24);
            ViewData["TotalPages"] = totalPages;
            ViewData["TotalJobs"] = totalJobs;
            ViewData["PageNumber"] = 1;
            ViewData["PageSize"] = 24;
            ViewData["Jobs"] = _context.Tbljobs.Include(p => p.Company).Where(x => x.Isdeleted == null || x.Isdeleted == false).OrderByDescending(x => x.Createdat).Skip(0).Take(24).ToList();

            return View();
        }

        public IActionResult JobDetail(int id)
        {

            Tbljob job = _context.Tbljobs.Where(p=>p.Jobid == id).Include(p=>p.Company).FirstOrDefault() ?? new Tbljob();
            ViewData["WDid"] = _context.MstLookups.Where(p => p.Lookupflag == 56 && p.Active == true).ToList();
            ViewData["ETid"] =_context.MstLookups.Where(p => p.Lookupflag == 57 && p.Active == true).ToList();
            ViewData["Companyid"] =_context.Tblcompanies.ToList();
            ViewData["FunctionalCategory"] =_context.MstLookups.Where(p => p.Lookupflag == 14 && p.Active == true).ToList();
            ViewData["LeavePolicies"] =_context.MstLookups.Where(p => p.Lookupflag == 17 && p.Active == true).ToList();
            ViewData["FlexibleWorkOption"] =_context.MstLookups.Where(p => p.Lookupflag == 18 && p.Active == true).ToList();
            ViewData["LearningAndDevelopment"] =_context.MstLookups.Where(p => p.Lookupflag == 19 && p.Active == true).ToList();
            ViewData["HealthcareAndWellness"] =_context.MstLookups.Where(p => p.Lookupflag == 20 && p.Active == true).ToList();
            ViewData["DEIAndWomenFriendlyPolicies"] =_context.MstLookups.Where(p => p.Lookupflag == 21 && p.Active == true).ToList();
            return View(job);
        }

        public IActionResult EventDetail(int? id)
        {

            // Get the existing landing page data
            var EventList = _context.Tblevents.Include(d => d.Tbleventbenefits).Include(d => d.Tbleventparticipants).Include(d => d.Theme).Include(d => d.EventPricingType).Include(d => d.Tblevidences).Include(d => d.EventType).Where(p => p.Eventid == id).ToList();
            ViewData["PastEvent"] = _context.Tblevents.Where(p => p.Eventid == id).Include(d=>d.Tbleventbenefits).Include(d=>d.Tbleventparticipants).Include(d => d.Theme).Include(d => d.EventPricingType).Include(d => d.EventType).ToList();
            return View(EventList);
        }

        public IActionResult LoadPartialView()
        {
           
            ViewData["Jobs"] = _context.Tbljobs.Where(x => x.Isdeleted == null || x.Isdeleted == false).Include(p => p.Company).ToList();
            ViewData["Comp"] = _context.Tblcompanies.Where(x => x.Isdeleted == null || x.Isdeleted == false).ToList();
            ViewData["Cityid"] = _context.MstLookups.Where(p => p.Lookupflag == 55 && p.Active == true).ToList();
            ViewData["WDid"] = _context.MstLookups.Where(p => p.Lookupflag == 56 && p.Active == true).ToList();
            ViewData["ETid"] = _context.MstLookups.Where(p => p.Lookupflag == 57 && p.Active == true).ToList();

            return PartialView("_JobPartial");
        }

        public IActionResult FilterJobs(string designation, string jobLocation, string role, string skills, string company, string experience, string experienceTo, string remote, string FSID, int pageNumber = 1, int pageSize = 24)
        {
            // Start with the base query
            var query = _context.Tbljobs.Include(p => p.Company).Where(x => x.Isdeleted == null || x.Isdeleted == false).AsQueryable();

            // Apply filters conditionally

            // Filter by Job Title (Designation)
            if (!string.IsNullOrEmpty(designation))
            {
                query = query.Where(j => j.Jobtitle.ToLower().Contains(designation.ToLower()));
            }

            // Filter by Location (City)
            if (!string.IsNullOrEmpty(jobLocation))
            {
                // Split by any non-alphanumeric character (comma, space, semicolon, etc.)
                var locations = Regex.Split(jobLocation, @"\W+")
                                     .Where(l => !string.IsNullOrEmpty(l))  // Remove any empty results
                                     .Select(l => l.Trim().ToLower())       // Trim and make lowercase
                                     .ToList();

                query = query.Where(j => !string.IsNullOrEmpty(j.City) && locations.Any(l => j.City.ToLower().Contains(l)));
            }

            // Filter by Role
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(j => !string.IsNullOrEmpty(j.Roleoverview) && j.Roleoverview.ToLower().Contains(role.ToLower()));
            }

            // Filter by Skills
            if (!string.IsNullOrEmpty(skills))
            {
                query = query.Where(j => !string.IsNullOrEmpty(j.Skill) && j.Skill.ToLower().Contains(skills.ToLower()));
            }

            // Filter by Company (CompanyId)
            if (!string.IsNullOrEmpty(company) && int.TryParse(company, out int CompanyId))
            {
                query = query.Where(j => j.Companyid == CompanyId);
            }

            // Filter by Experience (Years of Experience)
            if (!string.IsNullOrEmpty(experience) && int.TryParse(experience, out int experienceint))
            {
                if (!string.IsNullOrEmpty(experienceTo) && int.TryParse(experienceTo, out int experienceToint))
                {
                    query = query.Where(j => j.Yearexperience == experienceint || j.Yearexperience == experienceToint);
                }
                else
                {
                    query = query.Where(j => j.Yearexperience == experienceint);
                }
                    
            }
            else
            {
                if (!string.IsNullOrEmpty(experienceTo) && int.TryParse(experienceTo, out int experienceToint))
                {
                    query = query.Where(j=> j.Yearexperience == experienceToint);
                }
            }

            // Filter by Work Mode (Remote/Hybrid)
            if (!string.IsNullOrEmpty(remote) && int.TryParse(remote, out int remoteint))
            {
                query = query.Where(j => j.Workmode == remoteint);
            }

            // Filter by Work Mode (Remote/Hybrid)
            if (!string.IsNullOrEmpty(FSID) && int.TryParse(FSID, out int FSIDint))
            {
                query = query.Where(j => j.Functionalcategory.Contains(FSIDint));
            }

            // Pagination logic
            var totalJobs = query.OrderByDescending(x => x.Createdat).Count();
            var totalPages = (int)Math.Ceiling(totalJobs / (double)pageSize);

            var jobs = query.OrderByDescending(x => x.Createdat).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            ViewData["Jobs"] = jobs;
            ViewData["Comp"] = _context.Tblcompanies.ToList();
            ViewData["Cityid"] = _context.MstLookups.Where(p => p.Lookupflag == 55 && p.Active == true).ToList();
            ViewData["WDid"] = _context.MstLookups.Where(p => p.Lookupflag == 56 && p.Active == true).ToList();
            ViewData["ETid"] = _context.MstLookups.Where(p => p.Lookupflag == 57 && p.Active == true).ToList();
            ViewData["TotalPages"] = totalPages;
            ViewData["TotalJobs"] = totalJobs;
            ViewData["PageNumber"] = pageNumber;
            ViewData["PageSize"] = pageSize;
            // Return a partial view with the filtered jobs
            return PartialView("_JobPartial");
        }

        public IActionResult ComingSoon()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveParticipant(string name, string emailid, string mobileno, string linkedin, int eventID)
        {
            var participant = new Tbleventparticipant
            {
                Name = name,
                Emailid = emailid,
                Mobileno = mobileno,
                Linkedin = linkedin,
                Eventid = eventID
            };

            await _context.Tbleventparticipants.AddAsync(participant);
            var lstevent = _context.Tblevents
                          .Where(p => p.Eventid == eventID)
                          .FirstOrDefault();

            var body = $"Hi {name},<br><br>" +
                  $"Thank you for registering for <strong>{lstevent?.EventName}</strong>! We're excited to have you join us.<br><br>" +
                  $"Here are the event details:<br>" +
                  $"<strong>Date:</strong> {lstevent?.Startdateofevent:MMMM dd, yyyy}<br>" +
                  $"<strong>Time:</strong> {lstevent?.EventTimeStart} – {lstevent?.EventTimeEnd}<br>" +
                  $"<strong>Location:</strong> {lstevent?.EventVenue}<br><br>" +
                  //$"<strong>What to Expect:</strong><br>" +
                  //$"[Brief Description of Event Highlights]" +
                  //$"[Key Speakers / Guests]" +
                  //$"[Key Topics / Sessions]" +
                  $"We'll be in touch with reminders and any updates closer to the event. In the meantime, if you have any questions, feel free to reply to this email or reach out to us at <a href='mailto:letstalk@equilead.org'>letstalk@equilead.org</a>.<br><br>" +
                  $"Looking forward to seeing you there!<br><br>" +
                  $"Warm regards,<br>" +
                   $"The Equilead Team<br>" +
                $"<a href='https://www.linkedin.com/company/101423339' target='_blank'><img src='https://cdn-icons-png.flaticon.com/512/174/174857.png' width='20' alt='LinkedIn' style='margin-right:10px;'></a>" +
                 $"<a href='https://x.com/EquiLeadOrg' target='_blank'><img src='https://cdn-icons-png.flaticon.com/512/733/733579.png' width='20' alt='Twitter'></a>" +
                 $"<a href='https://www.instagram.com/equi.lead/' target='_blank'><img src='https://cdn-icons-png.flaticon.com/512/174/174855.png' width='20' alt='Instagram' style='margin: 0 10px;'></a>" +
                 $"<a href='https://www.youtube.com/EquiLead24' target='_blank'><img src='https://cdn-icons-png.flaticon.com/512/174/174883.png' width='20' alt='YouTube'></a>";




             await _service.SendEmailAsync(
                 emailid ,
                  $"You're Registered for {lstevent.EventName}", body);
            await _context.SaveChangesAsync();

            return RedirectToAction("landingpagenw", "Home");
        }

        [HttpGet]
        public JsonResult GetApplicantsDetails()
        {
            var isLoggedIn = HttpContext.Session.GetString("ApplicantId");
            if (isLoggedIn == null || isLoggedIn == "") { return Json("NO"); }
            else
            {
                var savepart = _context.Applicants
                                .Where(e => e.ApplicantId == Convert.ToInt32(isLoggedIn)).FirstOrDefault();
                return Json(savepart);
            }


        }

        [HttpPost]
        public async Task<IActionResult> EventDetailSaveParticipant(string name, string emailid, string mobileno, string linkedin, int eventID)
        {
            var participant = new Tbleventparticipant
            {
                Name = name,
                Emailid = emailid,
                Mobileno = mobileno,
                Linkedin = linkedin,
                Eventid = eventID
            };

            await _context.Tbleventparticipants.AddAsync(participant);
            await _context.SaveChangesAsync();
            // Set a success message
            TempData["SuccessMessage"] = "Registration successful!";
            return RedirectToAction("EventDetail", "Home", new { id = eventID });
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetSessionValue()
        {
            var isLoggedIn = HttpContext.Session.GetString("ApplicantId");
            return Json(new { isLoggedIn = isLoggedIn });
        }
        [HttpPost]
        public async Task<IActionResult> SubscribeNewsletter(string email)
        {
            if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            {
                TempData["ErrorMessage"] = "Please enter a valid email address.";
                return RedirectToAction("landingpagenw", "Home");
            }

            var existingSubscription = await _context.NewsletterSubscriptions
                .FirstOrDefaultAsync(n => n.Email == email);

            if (existingSubscription != null)
            {
                if (existingSubscription.Subscribed==null || existingSubscription.Subscribed==true)
                {
                    TempData["InfoMessage"] = "You are already subscribed.";
                }
                else
                {
                    existingSubscription.Subscribed = true;
                    existingSubscription.UpdatedAt = DateTime.UtcNow;
                    _context.NewsletterSubscriptions.Update(existingSubscription);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Subscription reactivated successfully!";
                }
            }
            else
            {
                var subscription = new NewsletterSubscription
                {
                    Email = email,
                    Subscribed = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _context.NewsletterSubscriptions.AddAsync(subscription);
                await _context.SaveChangesAsync();
                var body = $"Thank you for Subscribing To Our Newsletter.<br><br>";




                await _service.SendEmailAsync(
                    email,
                     $"Subscribed successfully", body);
                TempData["SuccessMessage"] = "Subscribed successfully! Thank You";
            }

            return RedirectToAction("landingpagenw", "Home");
        }
        [HttpPost]
        public IActionResult JobApplyClickTrack( int jobId)
        {
            int applicantId = 0;
            int.TryParse(HttpContext.Session.GetString("ApplicantId"), out applicantId);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var clickLog = new JobClickLog
            {
                JobId = jobId,
                UserId = applicantId,
                IpAddress = ipAddress,
                ClickedAt = DateTime.Now
            };

            _context.JobClickLogs.Add(clickLog);
            _context.SaveChanges();

            return Ok();
        }
        [HttpPost]
        public IActionResult ResourceApplyClickTrack( int resourceId)
        {
            int applicantId = 0;
            int.TryParse(HttpContext.Session.GetString("ApplicantId"), out applicantId);
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var clickLog = new ResourceClickLog
            {
                ResourceId = resourceId,
                UserId = applicantId,
                IpAddress = ipAddress,
                ClickedAt = DateTime.Now
            };

            _context.ResourceClickLogs.Add(clickLog);
            _context.SaveChanges();

            return Ok();
        }

    }
}
