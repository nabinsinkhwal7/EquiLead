using DocumentFormat.OpenXml.InkML;
using EquidCMS.Dto;
using EquidCMS.Models;
using EquidCMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Reflection.Metadata.BlobBuilder;

namespace EquidCMS.Controllers
{
    public class HomeController : Controller
    {
        private readonly EquiDbContext _context;

        private readonly EmailService _service;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, EquiDbContext context, EmailService service)
        {
            _logger = logger;
            _context = context;
            _service = service;
        }
        public IActionResult Index(string? startDate, string? endDate)
        {
            // Try parse dd/MM/yyyy format from querystring
            DateTime? parsedStart = null;
            DateTime? parsedEnd = null;

            if (!string.IsNullOrEmpty(startDate))
                parsedStart = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(endDate))
                parsedEnd = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var clickLogs = _context.ResourceClickLogs.AsQueryable();

            if (parsedStart.HasValue && parsedEnd.HasValue)
                clickLogs = clickLogs.Where(c => c.ClickedAt >= parsedStart.Value && c.ClickedAt <= parsedEnd.Value);


            // Get total clicks count
            var totalClicks = clickLogs.Count();
            //var totalClicks = _context.ResourceClickLogs.Count();

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
                ClickCount = clickLogs
                    .Join(_context.Tblresources, c => c.ResourceId, r => r.Resourceid, (c, r) => new { c, r })
                    .Where(x => x.r.Topic.Contains(t.Key))
                    .Count()
            }).Where(x => x.ClickCount > 0).ToList();

            var clicksByCategory = categoryLookup.Select(c => new
            {
                Id = c.Key,
                Name = c.Value,
                ClickCount = clickLogs
                    .Join(_context.Tblresources, c => c.ResourceId, r => r.Resourceid, (c, r) => new { c, r })
                    .Where(x => x.r.CategoryId == c.Key)
                    .Count()
            }).Where(x => x.ClickCount > 0).ToList();

            var minDate = _context.ResourceClickLogs.Min(c => c.ClickedAt);
            var maxDate = _context.ResourceClickLogs.Max(c => c.ClickedAt);

            // ViewBags for display
            ViewBag.MinDate = minDate.ToString("dd/MM/yyyy");
            ViewBag.MaxDate = maxDate.ToString("dd/MM/yyyy");

            ViewBag.SelectedStartDate = parsedStart?.ToString("dd/MM/yyyy") ?? ViewBag.MinDate;
            ViewBag.SelectedEndDate = parsedEnd?.ToString("dd/MM/yyyy") ?? ViewBag.MaxDate;
            ViewBag.TotalClicks = totalClicks;
            ViewBag.ClicksByTopic = clicksByTopic;
            ViewBag.ClicksByCategory = clicksByCategory;

            return View();
        }

        public IActionResult JobClickAnalytics(string? startDate, string? endDate, string? filterType)
        {
            DateTime? parsedStart = null;
            DateTime? parsedEnd = null;
            var filterOptions = new Dictionary<string, string>
                    {
                        { "Company", "Company" },
                        { "FunctionalCategory", "Functional Category" },
                        { "WorkMode", "Work Mode" },
                        { "EmployeeType", "Employee Type" },
                        { "LeavePolicies", "Leave Policies" },
                        { "FlexibleWorkOption", "Flexible Work Option" },
                        { "LearningAndDevelopment", "Learning & Development" },
                        { "HealthcareAndWellness", "Healthcare & Wellness" },
                        { "DEIAndWomenFriendlyPolicies", "DEI & Women-Friendly Policies" }
                    };
            if (!string.IsNullOrEmpty(startDate))
                parsedStart = DateTime.ParseExact(startDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            if (!string.IsNullOrEmpty(endDate))
                parsedEnd = DateTime.ParseExact(endDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

            var minDate = _context.JobClickLogs.Min(c => c.ClickedAt);
            var maxDate = _context.JobClickLogs.Max(c => c.ClickedAt);

            var clickLogs = _context.JobClickLogs.AsQueryable();

            if (parsedStart.HasValue && parsedEnd.HasValue)
                clickLogs = clickLogs.Where(c => c.ClickedAt >= parsedStart.Value && c.ClickedAt <= parsedEnd.Value);
            // Data result container
            // Data result container
            var jobClicksByCompany = new List<JobClickAnalyticsModel>();

            if (filterType == "Company")
            {
                // Join click logs with job and company
                jobClicksByCompany = clickLogs
                    .Join(_context.Tbljobs,
                          click => click.JobId,
                          job => job.Jobid,
                          (click, job) => new { click, job })
                    .Where(x => x.job.Companyid.HasValue && x.job.Company != null)
                    .GroupBy(x => new { x.job.Companyid, x.job.Company.Name })
                    .Select(g => new JobClickAnalyticsModel
                    {
                        CompanyName = g.Key.Name,
                        JobCount = _context.Tbljobs.Count(j => j.Companyid == g.Key.Companyid),
                        ClickCount = g.Count()
                    })
                    .Where(x => x.JobCount > 0 || x.ClickCount > 0)
                    .OrderByDescending(x => x.ClickCount)
                    .ToList();
            }
            else if (filterType == "FunctionalCategory")
            {
                var functionalCategoryLookup = _context.MstLookups
                    .Where(p => p.Lookupflag == 14)
                    .ToDictionary(p => p.Lookupcode, p => p.Description);

                var allJobs = _context.Tbljobs
                    .Where(j => j.Functionalcategory != null && j.Functionalcategory.Count > 0);

                // Filter and join clicks with jobs, including functional category
                var jobClicksWithCategories = clickLogs
                    .Where(click => click.ClickedAt >= parsedStart.Value && click.ClickedAt <= parsedEnd.Value)
                    .Join(allJobs,
                          click => click.JobId,
                          job => job.Jobid,
                          (click, job) => new { click, job })
                    .SelectMany(x => x.job.Functionalcategory!,
                                (x, fc) => new { FunctionalCategoryCode = fc, x.click, x.job.Jobid });

                var jobClicksByCategory = jobClicksWithCategories
                    .GroupBy(x => x.FunctionalCategoryCode)
                    .Select(g => new JobAnalyticsModel
                    {
                        Title = functionalCategoryLookup.ContainsKey(g.Key) ? functionalCategoryLookup[g.Key] : "Unknown",
                        JobCount = allJobs.Count(j => j.Functionalcategory!.Contains(g.Key)),
                        ClickCount = g.Count()
                    })
                    .Where(x => x.JobCount > 0 || x.ClickCount > 0)
                    .OrderByDescending(x => x.ClickCount)
                    .ToList();

                // Pass data to ViewBag
                ViewBag.JobClicksByCategory = jobClicksByCategory;
            }
            else if (filterType == "WorkMode")
            {
                var workModeLookup = _context.MstLookups
                    .Where(p => p.Lookupflag == 56) // Work Mode filter (Lookupflag == 56)
                    .ToDictionary(p => p.Lookupcode, p => p.Description);

                var allJobs = _context.Tbljobs
                    .Where(j => j.Workmode.HasValue); // Only consider jobs with a Workmode set

                // Join click logs with jobs and their work modes
                var jobClicksWithWorkModes = clickLogs
                    .Where(click => click.ClickedAt >= parsedStart.Value && click.ClickedAt <= parsedEnd.Value)
                    .Join(allJobs,
                          click => click.JobId,
                          job => job.Jobid,
                          (click, job) => new { click, job })
                    .Where(x => x.job.Workmode.HasValue) // Only include jobs with a Workmode
                    .Select(x => new { WorkMode = x.job.Workmode.Value, x.click }); // Map to Workmode for grouping

                // Group by Work Mode and calculate the click counts and job counts
                var jobClicksByWorkMode = jobClicksWithWorkModes
                    .GroupBy(x => x.WorkMode)
                    .Select(g => new JobAnalyticsModel
                    {
                        Title = workModeLookup.ContainsKey(g.Key) ? workModeLookup[g.Key] : "Unknown",
                        JobCount = allJobs.Count(j => j.Workmode == g.Key), // Count the jobs with this Work Mode
                        ClickCount = g.Count() // Count the clicks for this Work Mode
                    })
                    .Where(x => x.JobCount > 0 || x.ClickCount > 0)
                    .OrderByDescending(x => x.ClickCount)
                    .ToList();

                // Pass the results to the ViewBag
                ViewBag.JobClicksByWorkMode = jobClicksByWorkMode;
            }
            else if (filterType == "EmployeeType")
            {
                var employeeTypeLookup = _context.MstLookups
                    .Where(p => p.Lookupflag == 57) // Employee Type filter (Lookupflag == 57)
                    .ToDictionary(p => p.Lookupcode, p => p.Description);

                var allJobs = _context.Tbljobs
                    .Where(j => j.Employeetype.HasValue); // Only consider jobs with a valid EmployeeType

                // Join click logs with jobs and their employee types
                var jobClicksWithEmployeeTypes = clickLogs
                    .Where(click => click.ClickedAt >= parsedStart.Value && click.ClickedAt <= parsedEnd.Value)
                    .Join(allJobs,
                          click => click.JobId,
                          job => job.Jobid,
                          (click, job) => new { click, job })
                    .Where(x => x.job.Employeetype.HasValue) // Only include jobs with a valid EmployeeType
                    .Select(x => new { EmployeeType = x.job.Employeetype.Value, x.click }); // Map to EmployeeType for grouping

                // Group by Employee Type and calculate the click counts and job counts
                var jobClicksByEmployeeType = jobClicksWithEmployeeTypes
                    .GroupBy(x => x.EmployeeType)
                    .Select(g => new JobAnalyticsModel
                    {
                        Title = employeeTypeLookup.ContainsKey(g.Key) ? employeeTypeLookup[g.Key] : "Unknown",
                        JobCount = allJobs.Count(j => j.Employeetype == g.Key), // Count the jobs with this Employee Type
                        ClickCount = g.Count() // Count the clicks for this Employee Type
                    })
                    .Where(x => x.JobCount > 0 || x.ClickCount > 0)
                    .OrderByDescending(x => x.ClickCount)
                    .ToList();

                // Pass the results to the ViewBag
                ViewBag.JobClicksByEmployeeType = jobClicksByEmployeeType;
            }
            else if (filterType == "LeavePolicies")
            {
                var leavePolicyLookup = _context.MstLookups
                    .Where(p => p.Lookupflag == 17) // Leave Policies filter (Lookupflag == 17)
                    .ToDictionary(p => p.Lookupcode, p => p.Description);

                var allJobs = _context.Tbljobs
                    .Where(j => j.Leavepolicies != null && j.Leavepolicies.Count > 0); // Only consider jobs with leave policies

                // Join click logs with jobs and their leave policies
                var jobClicksWithLeavePolicies = clickLogs
                    .Where(click => click.ClickedAt >= parsedStart.Value && click.ClickedAt <= parsedEnd.Value)
                    .Join(allJobs,
                          click => click.JobId,
                          job => job.Jobid,
                          (click, job) => new { click, job })
                    .Where(x => x.job.Leavepolicies != null && x.job.Leavepolicies.Count > 0) // Only include jobs with leave policies
                    .SelectMany(x => x.job.Leavepolicies!,
                                (x, leavePolicyCode) => new { LeavePolicyCode = leavePolicyCode, x.click }); // Map to leave policy for grouping

                // Group by Leave Policy and calculate the click counts and job counts
                var jobClicksByLeavePolicy = jobClicksWithLeavePolicies
                    .GroupBy(x => x.LeavePolicyCode)
                    .Select(g => new JobAnalyticsModel
                    {
                        Title = leavePolicyLookup.ContainsKey(g.Key) ? leavePolicyLookup[g.Key] : "Unknown",
                        JobCount = allJobs.Count(j => j.Leavepolicies.Contains(g.Key)), // Count the jobs with this leave policy
                        ClickCount = g.Count() // Count the clicks for this leave policy
                    })
                    .Where(x => x.JobCount > 0 || x.ClickCount > 0)
                    .OrderByDescending(x => x.ClickCount)
                    .ToList();

                // Pass the results to the ViewBag
                ViewBag.JobClicksByLeavePolicy = jobClicksByLeavePolicy;
            }
            else if (filterType == "FlexibleWorkOption")
            {
                var flexibleWorkOptionLookup = _context.MstLookups
                    .Where(p => p.Lookupflag == 18) // Flexible Work Option filter (Lookupflag == 18)
                    .ToDictionary(p => p.Lookupcode, p => p.Description);

                var allJobs = _context.Tbljobs
                    .Where(j => j.Flexibleworkoption.HasValue); // Only consider jobs with a flexible work option

                // Join click logs with jobs and their flexible work options
                var jobClicksWithFlexibleWorkOptions = clickLogs
                    .Where(click => click.ClickedAt >= parsedStart.Value && click.ClickedAt <= parsedEnd.Value)
                    .Join(allJobs,
                          click => click.JobId,
                          job => job.Jobid,
                          (click, job) => new { click, job })
                    .Where(x => x.job.Flexibleworkoption.HasValue) // Only include jobs with flexible work options
                    .Select(x => new { FlexibleWorkOptionCode = x.job.Flexibleworkoption.Value, x.click }); // Map to flexible work option for grouping

                // Group by Flexible Work Option and calculate the click counts and job counts
                var jobClicksByFlexibleWorkOption = jobClicksWithFlexibleWorkOptions
                    .GroupBy(x => x.FlexibleWorkOptionCode)
                    .Select(g => new JobAnalyticsModel
                    {
                        Title = flexibleWorkOptionLookup.ContainsKey(g.Key) ? flexibleWorkOptionLookup[g.Key] : "Unknown",
                        JobCount = allJobs.Count(j => j.Flexibleworkoption == g.Key), // Count the jobs with this flexible work option
                        ClickCount = g.Count() // Count the clicks for this flexible work option
                    })
                    .Where(x => x.JobCount > 0 || x.ClickCount > 0)
                    .OrderByDescending(x => x.ClickCount)
                    .ToList();

                // Pass the results to the ViewBag
                ViewBag.JobClicksByFlexibleWorkOption = jobClicksByFlexibleWorkOption;
            }
            else if (filterType == "LearningAndDevelopment")
            {
                var learningAndDevelopmentLookup = _context.MstLookups
                    .Where(p => p.Lookupflag == 19) // Learning and Development filter (Lookupflag == 19)
                    .ToDictionary(p => p.Lookupcode, p => p.Description);

                var allJobs = _context.Tbljobs
                    .Where(j => j.Learninganddeveloment != null && j.Learninganddeveloment.Count > 0); // Only consider jobs with Learning & Development options

                // Join click logs with jobs and their Learning & Development options
                var jobClicksWithLearningAndDevelopment = clickLogs
                    .Where(click => click.ClickedAt >= parsedStart.Value && click.ClickedAt <= parsedEnd.Value)
                    .Join(allJobs,
                          click => click.JobId,
                          job => job.Jobid,
                          (click, job) => new { click, job })
                    .Where(x => x.job.Learninganddeveloment != null && x.job.Learninganddeveloment.Count > 0) // Ensure the job has Learning & Development options
                    .Select(x => new { LearningAndDevelopmentCode = x.job.Learninganddeveloment!, x.click }); // Map to Learning & Development options for grouping

                // Group by Learning & Development and calculate the click counts and job counts
                var jobClicksByLearningAndDevelopment = jobClicksWithLearningAndDevelopment
                    .SelectMany(x => x.LearningAndDevelopmentCode, (x, ld) => new { LearningAndDevelopmentCode = ld, x.click })
                    .GroupBy(x => x.LearningAndDevelopmentCode)
                    .Select(g => new JobAnalyticsModel
                    {
                        Title = learningAndDevelopmentLookup.ContainsKey(g.Key) ? learningAndDevelopmentLookup[g.Key] : "Unknown",
                        JobCount = allJobs.Count(j => j.Learninganddeveloment.Contains(g.Key)), // Count the jobs with this Learning & Development option
                        ClickCount = g.Count() // Count the clicks for this Learning & Development option
                    })
                    .Where(x => x.JobCount > 0 || x.ClickCount > 0)
                    .OrderByDescending(x => x.ClickCount)
                    .ToList();

                // Pass the results to the ViewBag
                ViewBag.JobClicksByLearningAndDevelopment = jobClicksByLearningAndDevelopment;
            }
            else if (filterType == "HealthcareAndWellness")
            {
                var healthcareAndWellnessLookup = _context.MstLookups
                    .Where(p => p.Lookupflag == 20) // Healthcare and Wellness filter (Lookupflag == 20)
                    .ToDictionary(p => p.Lookupcode, p => p.Description);

                var allJobs = _context.Tbljobs
                    .Where(j => j.Healthcareandwellness != null && j.Healthcareandwellness.Count > 0); // Only consider jobs with Healthcare and Wellness options

                // Join click logs with jobs and their Healthcare and Wellness options
                var jobClicksWithHealthcareAndWellness = clickLogs
                    .Where(click => click.ClickedAt >= parsedStart.Value && click.ClickedAt <= parsedEnd.Value)
                    .Join(allJobs,
                          click => click.JobId,
                          job => job.Jobid,
                          (click, job) => new { click, job })
                    .Where(x => x.job.Healthcareandwellness != null && x.job.Healthcareandwellness.Count > 0) // Ensure the job has Healthcare and Wellness options
                    .Select(x => new { HealthcareAndWellnessCode = x.job.Healthcareandwellness!, x.click }); // Map to Healthcare and Wellness options for grouping

                // Group by Healthcare and Wellness and calculate the click counts and job counts
                var jobClicksByHealthcareAndWellness = jobClicksWithHealthcareAndWellness
                    .SelectMany(x => x.HealthcareAndWellnessCode, (x, hw) => new { HealthcareAndWellnessCode = hw, x.click })
                    .GroupBy(x => x.HealthcareAndWellnessCode)
                    .Select(g => new JobAnalyticsModel
                    {
                        Title = healthcareAndWellnessLookup.ContainsKey(g.Key) ? healthcareAndWellnessLookup[g.Key] : "Unknown",
                        JobCount = allJobs.Count(j => j.Healthcareandwellness.Contains(g.Key)), // Count the jobs with this Healthcare and Wellness option
                        ClickCount = g.Count() // Count the clicks for this Healthcare and Wellness option
                    })
                    .Where(x => x.JobCount > 0 || x.ClickCount > 0)
                    .OrderByDescending(x => x.ClickCount)
                    .ToList();

                // Pass the results to the ViewBag
                ViewBag.JobClicksByHealthcareAndWellness = jobClicksByHealthcareAndWellness;
            }
            else if (filterType == "DEIAndWomenFriendlyPolicies")
            {
                // Fetch DEI & Women-Friendly Policies from the lookup table
                var deiAndWomenFriendlyPoliciesLookup = _context.MstLookups
                    .Where(p => p.Lookupflag == 21) // Lookupflag for DEI and Women-Friendly Policies
                    .ToDictionary(p => p.Lookupcode, p => p.Description);

                // Fetch jobs with DEI and Women-Friendly Policies options
                var allJobs = _context.Tbljobs
                    .Where(j => j.Deiandwomenfriendlypolicies != null && j.Deiandwomenfriendlypolicies.Count > 0); // Only consider jobs with DEI & Women-Friendly policies

                // Join click logs with jobs and their DEI and Women-Friendly policies
                var jobClicksWithDEIAndWomenFriendlyPolicies = clickLogs
                    .Where(click => click.ClickedAt >= parsedStart.Value && click.ClickedAt <= parsedEnd.Value)
                    .Join(allJobs,
                          click => click.JobId,
                          job => job.Jobid,
                          (click, job) => new { click, job })
                    .Where(x => x.job.Deiandwomenfriendlypolicies != null && x.job.Deiandwomenfriendlypolicies.Count > 0) // Ensure the job has DEI & Women-Friendly policies
                    .Select(x => new { DeiAndWomenFriendlyPoliciesCode = x.job.Deiandwomenfriendlypolicies!, x.click }); // Map to DEI & Women-Friendly Policies for grouping

                // Group by DEI & Women-Friendly Policies and calculate the job and click counts
                var jobClicksByDEIAndWomenFriendlyPolicies = jobClicksWithDEIAndWomenFriendlyPolicies
                    .SelectMany(x => x.DeiAndWomenFriendlyPoliciesCode, (x, dei) => new { DeiAndWomenFriendlyPoliciesCode = dei, x.click })
                    .GroupBy(x => x.DeiAndWomenFriendlyPoliciesCode)
                    .Select(g => new JobAnalyticsModel
                    {
                        Title = deiAndWomenFriendlyPoliciesLookup.ContainsKey(g.Key) ? deiAndWomenFriendlyPoliciesLookup[g.Key] : "Unknown",
                        JobCount = allJobs.Count(j => j.Deiandwomenfriendlypolicies.Contains(g.Key)), // Count the jobs with this DEI & Women-Friendly policy
                        ClickCount = g.Count() // Count the clicks for this DEI & Women-Friendly policy
                    })
                    .Where(x => x.JobCount > 0 || x.ClickCount > 0)
                    .OrderByDescending(x => x.ClickCount)
                    .ToList();

                // Pass the results to the ViewBag
                ViewBag.JobClicksByDEIAndWomenFriendlyPolicies = jobClicksByDEIAndWomenFriendlyPolicies;
            }

            ViewBag.MinDate = minDate.ToString("dd/MM/yyyy");
            ViewBag.MaxDate = maxDate.ToString("dd/MM/yyyy");
            ViewBag.TotalClicks = clickLogs.Count();
            ViewBag.FilterOptions = filterOptions;
            ViewBag.SelectedFilter = filterType;
            ViewBag.SelectedStartDate = parsedStart?.ToString("dd/MM/yyyy") ?? ViewBag.MinDate;
            ViewBag.SelectedEndDate = parsedEnd?.ToString("dd/MM/yyyy") ?? ViewBag.MaxDate;
            ViewBag.JobClicksByCompany = jobClicksByCompany;

            return View();
        }

        [HttpGet]
        public JsonResult GetDrilldown(string type, int id, string? startDate, string? endDate)
        {
            var query = _context.ResourceClickLogs
                .Join(_context.Tblresources, c => c.ResourceId, r => r.Resourceid, (c, r) => new { c, r });

            // Parse dates (dd/MM/yyyy)
            DateTime? parsedStart = null;
            DateTime? parsedEnd = null;
            var format = "dd/MM/yyyy";
            var culture = CultureInfo.InvariantCulture;

            if (!string.IsNullOrWhiteSpace(startDate))
                parsedStart = DateTime.ParseExact(startDate, format, culture);

            if (!string.IsNullOrWhiteSpace(endDate))
                parsedEnd = DateTime.ParseExact(endDate, format, culture);

            // Apply date filtering if valid dates exist
            if (parsedStart.HasValue && parsedEnd.HasValue)
            {
                query = query.Where(x => x.c.ClickedAt >= parsedStart.Value && x.c.ClickedAt <= parsedEnd.Value);
            }
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
            ViewData["Jobs"] = _context.Tbljobs.Include(p => p.Company).Where(x => x.Isdeleted == null || x.Isdeleted == false).OrderByDescending(x => x.Createdat).Take(3).ToList();
            ViewData["Events"] = _context.Tblevents.OrderByDescending(p => p.Enddateofevent).Where(x => x.Isdeleted == null || x.Isdeleted == false).Where(p => p.Startdateofevent > DateOnly.FromDateTime(DateTime.Now)).Take(4).ToList();
            ViewData["Linkedinuser"] = _context.TblSocialLinkdins.Where(x => x.IsDeleted == null || x.IsDeleted == false).ToList();
            ViewData["Successsyory"] = _context.Tblsuccesstests.Where(x => x.IsDeleted == null || x.IsDeleted == false).ToList();

            ViewData["infographic"] = _context.Tblinfographics.Where(x => x.Isdeleted == null || x.Isdeleted == false).ToList();

            ViewData["Cityid"] = _context.MstLookups.Where(p => p.Lookupflag == 55 && p.Active == true).ToList();
            ViewData["WDid"] = _context.MstLookups.Where(p => p.Lookupflag == 56 && p.Active == true).ToList();
            ViewData["ETid"] = _context.MstLookups.Where(p => p.Lookupflag == 57 && p.Active == true).ToList();

            var totalRows = _context.TblSocialLinkdins.Where(p => p.IsDeleted == false).Count();
            var rowsPerSet = totalRows / 3;
            if (rowsPerSet < 3)
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
                .Where(x => !x.Isdeleted && x.Isverified != null && x.Isverified == true && x.Rsdocumenttypeid != 6)
                .AsQueryable();

            var blogsData = _context.Tblresources
                .Where(x => !x.Isdeleted && x.Isverified != null && x.Isverified == true && x.Rsdocumenttypeid == 6)
                .ToList();
            ViewBag.BlogResources = blogsData;
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
            var data = _context.Tblaboutus.FirstOrDefault();
            ViewData["OurTeam"] = _context.Tblourteams.Where(x => x.Photo != null && x.Description != null).ToList();
            ViewData["Successsyory"] = _context.Tblsuccesstests.Where(x => x.IsDeleted == null || x.IsDeleted == false).ToList();
            return View(data);
        }
        public IActionResult Network()
        {
            var data = _context.TblSocialLinkdins.Where(x => x.IsDeleted != true).ToList();
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

            Tbljob job = _context.Tbljobs.Where(p => p.Jobid == id).Include(p => p.Company).FirstOrDefault() ?? new Tbljob();
            ViewData["WDid"] = _context.MstLookups.Where(p => p.Lookupflag == 56 && p.Active == true).ToList();
            ViewData["ETid"] = _context.MstLookups.Where(p => p.Lookupflag == 57 && p.Active == true).ToList();
            ViewData["Companyid"] = _context.Tblcompanies.ToList();
            ViewData["FunctionalCategory"] = _context.MstLookups.Where(p => p.Lookupflag == 14 && p.Active == true).ToList();
            ViewData["LeavePolicies"] = _context.MstLookups.Where(p => p.Lookupflag == 17 && p.Active == true).ToList();
            ViewData["FlexibleWorkOption"] = _context.MstLookups.Where(p => p.Lookupflag == 18 && p.Active == true).ToList();
            ViewData["LearningAndDevelopment"] = _context.MstLookups.Where(p => p.Lookupflag == 19 && p.Active == true).ToList();
            ViewData["HealthcareAndWellness"] = _context.MstLookups.Where(p => p.Lookupflag == 20 && p.Active == true).ToList();
            ViewData["DEIAndWomenFriendlyPolicies"] = _context.MstLookups.Where(p => p.Lookupflag == 21 && p.Active == true).ToList();
            return View(job);
        }

        public IActionResult EventDetail(int? id)
        {

            // Get the existing landing page data
            var EventList = _context.Tblevents.Include(d => d.Tbleventbenefits).Include(d => d.Tbleventparticipants).Include(d => d.Theme).Include(d => d.EventPricingType).Include(d => d.Tblevidences).Include(d => d.EventType).Where(p => p.Eventid == id).ToList();
            ViewData["PastEvent"] = _context.Tblevents.Where(p => p.Eventid == id).Include(d => d.Tbleventbenefits).Include(d => d.Tbleventparticipants).Include(d => d.Theme).Include(d => d.EventPricingType).Include(d => d.EventType).ToList();
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

        public IActionResult FilterJobs(string designation, string jobLocation, string role, string skills, string company, string experience, string experienceTo, string remote, string FSID, int pageNumber = 1, int pageSize = 24, bool recommend = false)
        {
            if (recommend)
            {
                var isLoggedIn = HttpContext.Session.GetString("ApplicantId");
                if (isLoggedIn == null || isLoggedIn == "")
                {
                }
                else
                {
                    var applicantData = _context.Applicants.Include(x => x.ApplicantCareerPreference)
                                    .Where(e => e.ApplicantId == Convert.ToInt32(isLoggedIn)).FirstOrDefault();

                    if (applicantData != null)
                    {
                        var careerPref = applicantData.ApplicantCareerPreference;
                        // Use experience
                        experienceTo = applicantData.YearsOfExperence?.ToString();

                        // Use preferred job role
                        if (string.IsNullOrEmpty(designation) && !string.IsNullOrEmpty(careerPref?.PreferredJobRole))
                            designation = careerPref.PreferredJobRole;

                        // Use preferred job location
                        if (string.IsNullOrEmpty(jobLocation) && !string.IsNullOrEmpty(careerPref?.PreferredJobLocation))
                            jobLocation = careerPref.PreferredJobLocation;

                        // Use work mode
                        if (string.IsNullOrEmpty(remote) && careerPref?.WorkMode != null)
                            remote = careerPref.WorkMode.ToString();

                        // Use functional area (FSID)
                        if (string.IsNullOrEmpty(FSID) && careerPref?.FunctionalArea != null && careerPref.FunctionalArea.Any())
                            FSID = string.Join(",", careerPref.FunctionalArea);

                        // You could also fetch from applicant’s skills
                        var skillSet = applicantData.ApplicantSkills.Select(s => s.SkillName).ToList();
                        if (string.IsNullOrEmpty(skills) && skillSet.Any())
                            skills = string.Join(",", skillSet);

                        bool missingCareerInfo =
                         (string.IsNullOrEmpty(careerPref?.PreferredJobRole)) &&
                         (string.IsNullOrEmpty(careerPref?.PreferredJobLocation)) &&
                         (careerPref?.WorkMode == null) &&
                         (careerPref?.FunctionalArea == null || !careerPref.FunctionalArea.Any()) &&
                         (!skillSet.Any()) &&
                         (applicantData.YearsOfExperence == null);

                        if (missingCareerInfo)
                        {
                            ViewData["ProfileMessage"] = "Please update your career preferences to get personalized job recommendations in profile.";

                            // Return no jobs
                            ViewData["Jobs"] = new List<Tbljob>();
                            ViewData["TotalPages"] = 0;
                            ViewData["TotalJobs"] = 0;
                            ViewData["PageNumber"] = pageNumber;
                            ViewData["PageSize"] = pageSize;
                            ViewData["Comp"] = _context.Tblcompanies.ToList();
                            ViewData["Cityid"] = _context.MstLookups.Where(p => p.Lookupflag == 55 && p.Active == true).ToList();
                            ViewData["WDid"] = _context.MstLookups.Where(p => p.Lookupflag == 56 && p.Active == true).ToList();
                            ViewData["ETid"] = _context.MstLookups.Where(p => p.Lookupflag == 57 && p.Active == true).ToList();

                            return PartialView("_JobPartial");
                        }
                    }
                }
            }
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
            if (!string.IsNullOrEmpty(experience) || !string.IsNullOrEmpty(experienceTo))
            {
                bool hasMin = int.TryParse(experience, out int userMin);
                bool hasMax = int.TryParse(experienceTo, out int userMax);

                if (hasMin && hasMax)
                {
                    // Any overlap between user and job experience ranges
                    query = query.Where(j =>
                        (j.Yearexperiencefrom ?? int.MaxValue) >= userMin &&
                        (j.Yearexperienceto ?? 0) <= userMax
                    );
                }
                else if (hasMin)
                {
                    // Check if either job's from or to falls on or after userMin
                    query = query.Where(j =>
                        ((j.Yearexperienceto ?? int.MaxValue) >= userMin) && ((j.Yearexperiencefrom ?? 0) <= userMin)
                    );
                }
                else if (hasMax)
                {
                    // Check if either job's from or to falls on or before userMax
                    query = query.Where(j =>
                        ((j.Yearexperiencefrom ?? 0) <= userMax)
                    );
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
                emailid,
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
                if (existingSubscription.Subscribed == null || existingSubscription.Subscribed == true)
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
        public IActionResult JobApplyClickTrack(int jobId)
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
        public IActionResult ResourceApplyClickTrack(int resourceId)
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
