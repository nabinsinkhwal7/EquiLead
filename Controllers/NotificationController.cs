using EquidCMS.Models;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

                if(ccEmail != null)
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
                //var lstReminder = _context.TblEventNotifications.ToList();
                //foreach(var reminder in lstReminder)
                //{
                //    if (timeUnit == "Days" && reminder.HourDayUnit == timeUnit)
                //    {
                //        var compDate = DateTime.Now.AddDays(-reminder.HourDay);
                //        var lstevent = _context.Tblevents.Where(p => p.Startdateofevent.Year == compDate.Year && p.Startdateofevent.Month == compDate.Month && p.Startdateofevent.Day == compDate.Day).ToList();

                //        var lstApplicants = _context.Applicants.Select(p => p.Email).ToList();

                //        foreach(var eventItem in lstevent)
                //        {
                //            var lstEventParticipent = _context.Tbleventparticipants.Where(k => k.Eventid == eventItem.Eventid).Select(p => p.Emailid).ToList();

                //            var subject = "Just " + reminder.HourDay.ToString() + " Day to Go for ["+ eventItem.EventName.ToString() + "] , [User’s Name]! Don’t Miss Out!";

                //            var body = "Hi,"+"<br><br>"+
                //                "It’s almost here! We’re just 1 day away from[Event Name]! Here’s a quick reminder with all the details you need to join us:"+
                //            "Event Details:" + "<br><br>" +"Name:["+ eventItem.EventName.ToString()
                            

                //            if (lstevent.Count > 0)
                //            {
                //                await SendEmail(lstApplicants, null, lstEventParticipent, subject, body);
                //            }
                //        }
                        
                        

                //    } else if (timeUnit == "Hours" && reminder.HourDayUnit == timeUnit) {
                //        var compDate = DateTime.Now.AddHours(-reminder.HourDay);

                //        var lstevent = _context.Tblevents.Where(p => p.Startdateofevent.Year == compDate.Year && p.Startdateofevent.Month == compDate.Month && p.Startdateofevent.Day == compDate.Day && p.EventTimeStart.Value.Hour == compDate.Hour).ToList();

                //        var lstApplicants = _context.Applicants.Select(p => p.Email).ToList();

                //        foreach (var eventItem in lstevent)
                //        {
                //            var lstEventParticipent = _context.Tbleventparticipants.Where(k => k.Eventid == eventItem.Eventid).Select(p => p.Emailid).ToList();

                //            var subject = "Just " + reminder.HourDay.ToString() + " Day to Go for [" + eventItem.EventName.ToString() + "] Don’t Miss Out!";

                //            var body = "Hi Participent, This is to inform you event will start within time";

                //            if (lstevent.Count > 0)
                //            {
                //                await SendEmail(lstApplicants, null, lstEventParticipent, subject, body);
                //            }
                //        }
                //    }
                //}

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
