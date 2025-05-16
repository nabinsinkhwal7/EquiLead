using EquidCMS.Models;
using Humanizer.Localisation;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MimeKit;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace EquidCMS.Controllers
{
    public class NotificationController : Controller
    {
        private readonly EquiDbContext _context;
        private readonly IConfiguration _config;

        public NotificationController(EquiDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async void SendNotificationBucket(string timeUnit)
        {
            await CreateNotification(timeUnit);
        }

        [HttpGet("test-alerts")]
        public async Task<IActionResult> TestSendAlerts()
        {
            await SendJobAlerts();
            return Ok("Alerts sent (if matching applicants found).");
        }

        public async Task<bool> SendEmail(List<string> toEmail, List<string?>? ccEmail, List<string?>? bccEmail, string subject, string body)
        {
            try
            {
                var emailSettings = _config.GetSection("EmailSettings");
                string smtpServer = emailSettings["SMTPServer"];
                int smtpPort = int.Parse(emailSettings["SMTPPort"]);
                string senderEmail = emailSettings["SenderEmail"];
                string senderPassword = emailSettings["SenderPassword"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Equilead", senderEmail));
                foreach (var item in toEmail)
                {
                    message.To.Add(new MailboxAddress("", item));
                }

                if (ccEmail != null)
                {
                    foreach (var item in ccEmail)
                    {
                        message.Cc.Add(new MailboxAddress("", item));
                    }
                }

                if (bccEmail != null)
                {
                    foreach (var item in bccEmail)
                    {
                        message.Bcc.Add(new MailboxAddress("", item));
                    }
                }
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = body };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(senderEmail, senderPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> CreateNotification(string timeUnit)
        {
            try
            {
                await EventReminderNotification(timeUnit);
                Console.WriteLine($"[Scheduler] CreateNotification called with timeUnit: {timeUnit} at {DateTime.Now}");

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private async Task EventReminderNotification(string timeUnit)
        {
            var lstReminder = _context.TblEventNotifications.ToList();
            foreach (var reminder in lstReminder)
            {
                if (timeUnit == "Days" && reminder.HourDayUnit == timeUnit)
                {
                    var compDate = DateTime.Now.AddDays(-reminder.HourDay);
                    var lstevent = _context.Tblevents.Where(p => p.Startdateofevent.Year == compDate.Year && p.Startdateofevent.Month == compDate.Month && p.Startdateofevent.Day == compDate.Day).ToList();

                    var lstApplicants = _context.Applicants.Select(p => p.Email).ToList();

                    foreach (var eventItem in lstevent)
                    {
                        var lstEventParticipent = _context.Tbleventparticipants.Where(k => k.Eventid == eventItem.Eventid).Select(p => p.Emailid).ToList();

                        var subject = "Just " + reminder.HourDay.ToString() + " Day to Go for [" + eventItem.EventName.ToString() + "] , [User’s Name]! Don’t Miss Out!";

                        var body = "Hi," + "<br><br>" +
                            "It’s almost here! We’re just 1 day away from[Event Name]! Here’s a quick reminder with all the details you need to join us:" +
                        "Event Details:" + "<br><br>" + "Name:[" + eventItem.EventName.ToString();


                        if (lstevent.Count > 0)
                        {
                            await SendEmail(lstApplicants, null, lstEventParticipent, subject, body);
                        }
                    }



                }
                else if (timeUnit == "Hours" && reminder.HourDayUnit == timeUnit)
                {
                    var compDate = DateTime.Now.AddHours(-reminder.HourDay);

                    var lstevent = _context.Tblevents.Where(p => p.Startdateofevent.Year == compDate.Year && p.Startdateofevent.Month == compDate.Month && p.Startdateofevent.Day == compDate.Day && p.EventTimeStart.Value.Hour == compDate.Hour).ToList();

                    var lstApplicants = _context.Applicants.Select(p => p.Email).ToList();

                    foreach (var eventItem in lstevent)
                    {
                        var lstEventParticipent = _context.Tbleventparticipants.Where(k => k.Eventid == eventItem.Eventid).Select(p => p.Emailid).ToList();

                        var subject = "Just " + reminder.HourDay.ToString() + " Day to Go for [" + eventItem.EventName.ToString() + "] Don’t Miss Out!";

                        var body = "Hi Participent, This is to inform you event will start within time";

                        if (lstevent.Count > 0)
                        {
                            await SendEmail(lstApplicants, null, lstEventParticipent, subject, body);
                        }
                    }
                }
            }
        }
        public async Task SendJobAlerts()
        {
            var today = DateTime.Today;

            // Get today's jobs
            var todaysJobs = _context.Tbljobs
                .Where(j => j.Createdat.HasValue &&
                j.Createdat.Value.Date == today && j.Isdeleted != true).Include(x=>x.Company)
                .ToList();

            foreach (var job in todaysJobs)
            {
                int? jobExperience = job.Yearexperience;
                int? jobWorkMode = job.Workmode;
                var jobFunctionalCategories = job.Functionalcategory;

                if (jobExperience == null || jobWorkMode == null || jobFunctionalCategories == null || !jobFunctionalCategories.Any())
                    continue; // skip if incomplete

                // Find applicants who match this job's experience and work mode
                var matchedApplicants = _context.Applicants
                                          .Where(a =>
                                              a.ApplicantCareerPreference != null &&
                                              (
                                                  (a.YearsOfExperence.HasValue && a.YearsOfExperence.Value >= jobExperience) ||
                                                  (a.ApplicantCareerPreference.WorkMode == jobWorkMode) ||
                                                  (a.ApplicantCareerPreference.FunctionalArea != null &&
                                                   a.ApplicantCareerPreference.FunctionalArea.Any(f => jobFunctionalCategories.Contains(f)))
                                              )
                                          )
                                          .ToList();

                if (matchedApplicants.Count == 0)
                    continue;

                foreach (var applicant in matchedApplicants)
                {
                    var subject = $"Unlock New Career Paths, {applicant.FullName ?? "Applicant"}! Check Out Latest Job Alerts!";

                    var body = $@"
                <div style='font-family: Arial, sans-serif; font-size: 14px; color: #333;'>
                    <p>Hi {applicant.FullName ?? "there"},</p>

                    <p>We’ve handpicked job opportunities that align perfectly with your skills and preferences – check them out below!</p>

                    <div style='margin-bottom: 15px;'>
                        <strong>{job.Jobtitle}</strong><br/>
                        Company: {job.Company.Name}<br/>
                        Location: {job.City}<br/>
                        Deadline: {(job.Applicationdeadline?.ToString("dd-mmm-yyyy") ?? "N/A")}<br/>
                    </div>

                    <p style='margin: 20px 0;'>
                        <a href='https://equilead.org/Home/JobDetail/{job.Jobid}' style='display: inline-block; background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;'>
                            View & Apply Now
                        </a>
                    </p>

                    <p>If you’re interested in a specific type of role, feel free to 
                        <a href='http://equilead.org/applicant/index'>update your preferences</a> 
                        and we’ll send tailored recommendations straight to your inbox.
                    </p>

                    <p>All the best for your application!</p>

                    <p>Best regards,<br/><strong>The EquiLead Team</strong></p>
                    <p><a href='http://equilead.microwarecomp.com/'>Visit Our Platform</a></p>
                </div>";

                    // Send email to individual applicant
                    await SendEmail(new List<string> { applicant.Email }, null, null, subject, body);
                }
            }
        }

        // Helper method to convert WorkMode ID to readable text
        private string GetWorkModeText(int mode)
        {
            return mode switch
            {
                1 => "Remote",
                2 => "Hybrid",
                3 => "On-site",
                _ => "Not Specified"
            };
        }

    }
}
