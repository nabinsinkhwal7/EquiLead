using EquidCMS.Controllers;
using EquidCMS.Models;
using EquidCMS.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

            TempData["EditRight"] = _context.RoleMenus.Where(c => c.RoleId == Convert.ToInt32(HttpContext.Session.GetString("RoleID")) && c.MenuId == Convert.ToInt32(HttpContext.Session.GetString("MenuId"))).Select(p => p.Edit).FirstOrDefault();

            TempData["DeleteRight"] = _context.RoleMenus.Where(c => c.RoleId == Convert.ToInt32(HttpContext.Session.GetString("RoleID")) && c.MenuId == Convert.ToInt32(HttpContext.Session.GetString("MenuId"))).Select(p => p.IsDeleted).FirstOrDefault();

            TempData["AddRight"] = _context.RoleMenus.Where(c => c.RoleId == Convert.ToInt32(HttpContext.Session.GetString("RoleID")) && c.MenuId == Convert.ToInt32(HttpContext.Session.GetString("MenuId"))).Select(p => p.AddNew).FirstOrDefault();

            SPMAilCommonModel spmailmodel = new SPMAilCommonModel();
            if (HttpContext.Session.GetString("UserTypeId") == "1")
            {
                spmailmodel.LSTSpmailInbox = _context.TblSpmailInboxes.Where(p => p.IsDeletedRc == false && p.RecieverId == null).ToList();
            }
            else
            {
                spmailmodel.LSTSpmailInbox = _context.TblSpmailInboxes.Where(p => p.IsDeletedRc == false && p.RecieverId == Convert.ToInt32(HttpContext.Session.GetString("Suid"))).ToList();

            }

            spmailmodel.LSTSpmailAttachment = _context.TblSpmailAttachments.ToList();

            spmailmodel.LSTRegistrationForm = _context.TblRegistrationForms.ToList();

            ViewData["SchoolList"] = new SelectList(_context.TblRegistrationForms.Where(p => p.IsDeleted == false), "Suid", "SchoolName");

            int? suid = null;
            if (HttpContext.Session.GetString("UserTypeId") == "1")
            {
                suid = null;
            }
            else
            {
                suid = Convert.ToInt32(HttpContext.Session.GetString("Suid"));

            }

            getcount(suid);

            ViewBag.Flag = "IN";

            return View(spmailmodel);
        }

        public void getcount(int? suid)
        {
            ViewBag.TOTInbox = _context.TblSpmailInboxes.Where(p => p.IsDeletedRc == false && p.RecieverId == suid && p.IsDraft == false).Count();

            ViewBag.TOTSent = _context.TblSpmailInboxes.Where(p => p.IsDeletedSn == false && p.SenderId == suid && p.IsDraft == false).Count();

            ViewBag.TOTDraft = _context.TblSpmailInboxes.Where(p => p.IsDeletedSn == false && p.SenderId == suid && p.IsDraft == true).Count();

            ViewBag.TOTTrash = _context.TblSpmailInboxes.Where(p => p.IsDeletedRc == true && (p.RecieverId == suid || p.SenderId == suid)).Count();

            ViewBag.TOTContact = _context.TblContactDetails.Count();

            ViewBag.TOTCGroup = _context.MstContactGroups.Where(p => p.Isdeleted == false).Count();

        }

        [HttpPost]
        public ActionResult SendMailM(List<IFormFile> files)
        {
            try
            {
                int? SUID = null;
                if (HttpContext.Request.Form["SUID"] == "null")
                {
                    SUID = null;
                }
                else
                {
                    SUID = Convert.ToInt32(HttpContext.Request.Form["SUID"]);

                }

                var Subject = Convert.ToString(HttpContext.Request.Form["Subject"]);
                var Message = Convert.ToString(HttpContext.Request.Form["Message"]);
                var Flag = Convert.ToString(HttpContext.Request.Form["Flag"]);

                var AttachmentFiles = files;


                TblSpmailInbox SpmailInbox = new TblSpmailInbox();

                SpmailInbox.RecieverId = SUID;
                if (HttpContext.Session.GetString("UserTypeId") == "1")
                {
                    SpmailInbox.SenderId = null;
                }
                else
                {
                    SpmailInbox.SenderId = Convert.ToInt32(HttpContext.Session.GetString("Suid"));

                }

                SpmailInbox.Subject = Subject;
                SpmailInbox.Message = Message;
                SpmailInbox.IsInTrash = false;
                SpmailInbox.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
                SpmailInbox.CreatedOn = System.DateTime.Now;

                if (Flag == "SEND") { SpmailInbox.IsDraft = false; } else { SpmailInbox.IsDraft = true; }

                _context.Add(SpmailInbox);
                _context.SaveChanges();

                string bannerimgname = "", bannerext = "", bannerPath = "", attachmentSize = "";

                TblSpmailAttachment stEvi = new TblSpmailAttachment();
                List<TblSpmailAttachment> lststEvi = new List<TblSpmailAttachment>();
                if (AttachmentFiles != null)
                {
                    foreach (var formFile in AttachmentFiles)
                    {
                        if (formFile.Length > 0)
                        {

                            var bannerName = Path.GetFileName(formFile.FileName);
                            bannerext = Path.GetExtension(formFile.FileName);


                            bannerimgname = Guid.NewGuid().ToString();

                            bannerPath = "/images/MailAttachments/" + SpmailInbox.PrmailId + "MLA_" + bannerimgname + bannerext;

                            using (var stream = new FileStream(Path.Combine("wwwroot/images/MailAttachments", SpmailInbox.PrmailId + "MLA_" + bannerimgname + bannerext), FileMode.Create))
                            {
                                formFile.CopyToAsync(stream);
                            }

                            lststEvi.Add(new TblSpmailAttachment
                            {
                                PrmailId = SpmailInbox.PrmailId,
                                AttachmentPath = SpmailInbox.PrmailId + "MLA_" + bannerimgname + bannerext,
                                AttachmentOrigName = bannerName,
                                AttachmentExtension = bannerext,
                            });

                        }
                    }
                    _context.AddRange(lststEvi);
                    _context.SaveChanges();

                }

                //---------------------------------Email common FN----------------------------//

                string sendertextdisplay = "";
                if (HttpContext.Session.GetString("UserTypeId") == "1")
                {
                    sendertextdisplay = " from Fairtrade India Schools Programme Team";
                }
                else
                {

                    sendertextdisplay = " from " + _context.TblRegistrationForms.Where(p => p.Suid == Convert.ToInt32(HttpContext.Session.GetString("Suid"))).Select(p => p.SchoolName).FirstOrDefault();

                }
                //please check this with your school email//is mail is sented good or not
                string body = "You have received an email " + sendertextdisplay + ". Requesting you to visit FISP reporting portal profile.";
                string emailto = _context.TblRegistrationForms.Where(p => p.Suid == SUID).Select(p => p.SchoolEmail).FirstOrDefault();
                string subject = "Email Notification";
                string friendlyname = _context.EmailConfigurations.Where(x => x.Key == "EmailFriendlyName" && !x.IsDeleted).Select(p => p.Value).FirstOrDefault();
                string from = _context.EmailConfigurations.Where(x => x.Key == "EmailFrom" && !x.IsDeleted).Select(p => p.Value).FirstOrDefault();
                string username = _context.EmailConfigurations.Where(x => x.Key == "UserName" && !x.IsDeleted).Select(p => p.Value).FirstOrDefault();
                string password = _context.EmailConfigurations.Where(x => x.Key == "Password" && !x.IsDeleted).Select(p => p.Value).FirstOrDefault();
                string smtpclienthost = _context.EmailConfigurations.Where(x => x.Key == "SmtpClientHost" && !x.IsDeleted).Select(p => p.Value).FirstOrDefault();
                CommonFN.CommonEmailFN(body, emailto, subject, friendlyname, from, username, password, smtpclienthost);

                //----------------------------------------------------------------------//

                return Json("1");
            }
            catch (Exception e)
            {
                return Json("0");
            }

        }

        [HttpPost]
        public ActionResult SendMailtomember(List<IFormFile> files)
        {
            try
            {
                int? SUID = null;
                if (HttpContext.Request.Form["SUID"] == "null")
                {
                    SUID = null;
                }
                else
                {
                    SUID = Convert.ToInt32(HttpContext.Request.Form["SUID"]);

                }

                var Subject = Convert.ToString(HttpContext.Request.Form["Subject"]);
                var Message = Convert.ToString(HttpContext.Request.Form["Message"]);
                var Flag = Convert.ToString(HttpContext.Request.Form["Flag"]);

                var MemberID = Convert.ToInt32(HttpContext.Request.Form["MemberID"]);

                var MFlag = Convert.ToString(HttpContext.Request.Form["MFlag"]);

                var AttachmentFiles = files;


                TblSpmailInbox SpmailInbox = new TblSpmailInbox();

                SpmailInbox.RecieverId = SUID;
                if (HttpContext.Session.GetString("UserTypeId") == "1")
                {
                    SpmailInbox.SenderId = null;
                }
                else
                {
                    SpmailInbox.SenderId = Convert.ToInt32(HttpContext.Session.GetString("Suid"));

                }

                SpmailInbox.Subject = Subject;
                SpmailInbox.Message = Message;
                SpmailInbox.IsInTrash = false;
                SpmailInbox.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
                SpmailInbox.CreatedOn = System.DateTime.Now;

                if (MFlag == "1") { SpmailInbox.IsAlert = false; } else { SpmailInbox.IsAlert = true; }

                if (Flag == "SEND") { SpmailInbox.IsDraft = false; } else { SpmailInbox.IsDraft = true; }

                _context.Add(SpmailInbox);
                _context.SaveChanges();

                string bannerimgname = "", bannerext = "", bannerPath = "", attachmentSize = "";

                TblSpmailAttachment stEvi = new TblSpmailAttachment();
                List<TblSpmailAttachment> lststEvi = new List<TblSpmailAttachment>();
                if (AttachmentFiles != null)
                {
                    foreach (var formFile in AttachmentFiles)
                    {
                        if (formFile.Length > 0)
                        {

                            var bannerName = Path.GetFileName(formFile.FileName);
                            bannerext = Path.GetExtension(formFile.FileName);


                            bannerimgname = Guid.NewGuid().ToString();

                            bannerPath = "/images/MailAttachments/" + SpmailInbox.PrmailId + "MLA_" + bannerimgname + bannerext;

                            using (var stream = new FileStream(Path.Combine("wwwroot/images/MailAttachments", SpmailInbox.PrmailId + "MLA_" + bannerimgname + bannerext), FileMode.Create))
                            {
                                formFile.CopyToAsync(stream);
                            }

                            lststEvi.Add(new TblSpmailAttachment
                            {
                                PrmailId = SpmailInbox.PrmailId,
                                AttachmentPath = SpmailInbox.PrmailId + "MLA_" + bannerimgname + bannerext,
                                AttachmentOrigName = bannerName,
                                AttachmentExtension = bannerext,
                            });

                        }
                    }
                    _context.AddRange(lststEvi);
                    _context.SaveChanges();

                }

                //---------------------------------Email common FN----------------------------//

                string body = Message;
                string emailto = _context.TblContactDetails.Where(p => p.MemberId == MemberID).Select(p => p.EmailId).FirstOrDefault();
                string subject = Subject;
                string friendlyname = _context.EmailConfigurations.Where(x => x.Key == "EmailFriendlyName" && !x.IsDeleted).Select(p => p.Value).FirstOrDefault();
                string from = _context.EmailConfigurations.Where(x => x.Key == "EmailFrom" && !x.IsDeleted).Select(p => p.Value).FirstOrDefault();
                string username = _context.EmailConfigurations.Where(x => x.Key == "UserName" && !x.IsDeleted).Select(p => p.Value).FirstOrDefault();
                string password = _context.EmailConfigurations.Where(x => x.Key == "Password" && !x.IsDeleted).Select(p => p.Value).FirstOrDefault();
                string smtpclienthost = _context.EmailConfigurations.Where(x => x.Key == "SmtpClientHost" && !x.IsDeleted).Select(p => p.Value).FirstOrDefault();
                CommonFN.CommonEmailFN(body, emailto, subject, friendlyname, from, username, password, smtpclienthost);

                //----------------------------------------------------------------------//

                return Json("1");
            }
            catch (Exception e)
            {
                return Json("0");
            }
        }


        public class suidlist
        {
            public int? suid { get; set; }
        }
        [HttpPost]
        public async Task<ActionResult> SendMailMultitomember(List<IFormFile> files)
        {
            try
            {
                var Subject = Convert.ToString(HttpContext.Request.Form["Subject"]);
                var Message = Convert.ToString(HttpContext.Request.Form["Message"]);
                var Flag = Convert.ToString(HttpContext.Request.Form["Flag"]);
                var MFlag = Convert.ToString(HttpContext.Request.Form["MFlag"]);
                var GroupID = Convert.ToInt32(HttpContext.Request.Form["GroupID"]);
                var PGRPID = Convert.ToInt32(HttpContext.Request.Form["PGRPID"]);

                if (GroupID != 0)
                {
                    List<suidlist> lstcntgrpmapschool = new List<suidlist>();
                    await _context.TblContacgGrpMappings.Include(p => p.Member).Where(p => p.ContactGroupId == GroupID).Select(p => p.Member.Suid).Distinct().ForEachAsync(m =>
                       lstcntgrpmapschool.Add(new suidlist
                       {
                           suid = m,
                       }));

                    foreach (var item1 in lstcntgrpmapschool)
                    {
                        sendmailtoschoolfn(Message, Subject, MFlag, Flag, item1.suid, files);
                    }

                    List<TblContacgGrpMapping> lstcntgrpmap = _context.TblContacgGrpMappings.Include(p => p.Member).Where(p => p.ContactGroupId == GroupID).ToList();
                    foreach (var item in lstcntgrpmap)
                    {
                        sendmailfn(Message, item.Member.EmailId, Subject);
                    }
                }
                else if (PGRPID == 1)
                {
                    List<TblRegistrationForm> lstschoollist = _context.TblRegistrationForms.Where(p => p.IsVerified == true).ToList();

                    foreach (var item1 in lstschoollist)
                    {
                        sendmailtoschoolfn(Message, Subject, MFlag, Flag, item1.Suid, files);

                        sendmailfn("You have received an email " + item1.SchoolName + ". Requesting you to visit FISP reporting portal profile.", item1.SchoolEmail, "Email Notification");
                    }
                }
                else if (PGRPID == 2)
                {
                    List<suidlist> lstcntgrpmapschool = new List<suidlist>();
                    await _context.Stage1Stp2committeeDetails.Include(p => p.Su).Where(p => p.IsDeleted == false).Select(p => p.Suid).Distinct().ForEachAsync(m =>
                       lstcntgrpmapschool.Add(new suidlist
                       {
                           suid = m,
                       }));

                    foreach (var item1 in lstcntgrpmapschool)
                    {
                        sendmailtoschoolfn(Message, Subject, MFlag, Flag, item1.suid, files);
                    }

                    List<Stage1Stp2committeeDetail> lstcntgrpmap = _context.Stage1Stp2committeeDetails.Include(p => p.Su).Where(p => p.IsDeleted == false).ToList();
                    foreach (var item in lstcntgrpmap)
                    {
                        //sendmailfn(Message, item.EmailId, Subject);
                        sendmailfn("You have received an email " + item.Name + ". Requesting you to visit FISP reporting portal profile.", item.EmailId, "Email Notification");
                    }
                }


                return Json("1");
            }
            catch (Exception e)
            {
                return Json("0");
            }
        }

        public void sendmailfn(string Message, string EmailId, string Subject)
        {
            //---------------------------------Email common FN----------------------------//

            string body = Message;
            string emailto = EmailId;
            string subject = Subject;
            string friendlyname = _context.EmailConfigurations.Where(x => x.Key == "EmailFriendlyName" && !x.IsDeleted).Select(p => p.Value).FirstOrDefault();
            string from = _context.EmailConfigurations.Where(x => x.Key == "EmailFrom" && !x.IsDeleted).Select(p => p.Value).FirstOrDefault();
            string username = _context.EmailConfigurations.Where(x => x.Key == "UserName" && !x.IsDeleted).Select(p => p.Value).FirstOrDefault();
            string password = _context.EmailConfigurations.Where(x => x.Key == "Password" && !x.IsDeleted).Select(p => p.Value).FirstOrDefault();
            string smtpclienthost = _context.EmailConfigurations.Where(x => x.Key == "SmtpClientHost" && !x.IsDeleted).Select(p => p.Value).FirstOrDefault();
            CommonFN.CommonEmailFN(body, emailto, subject, friendlyname, from, username, password, smtpclienthost);

            //----------------------------------------------------------------------//
        }

        public void sendmailtoschoolfn(string Message, string Subject, string MFlag, string Flag, int? suid, List<IFormFile> files)
        {
            int? SUID = suid;
            var AttachmentFiles = files;
            TblSpmailInbox SpmailInbox = new TblSpmailInbox();
            SpmailInbox.RecieverId = SUID;
            if (HttpContext.Session.GetString("UserTypeId") == "1")
            {
                SpmailInbox.SenderId = null;
            }
            else
            {
                SpmailInbox.SenderId = Convert.ToInt32(HttpContext.Session.GetString("Suid"));

            }

            SpmailInbox.Subject = Subject;
            SpmailInbox.Message = Message;
            SpmailInbox.IsInTrash = false;
            SpmailInbox.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
            SpmailInbox.CreatedOn = System.DateTime.Now;

            if (MFlag == "ML") { SpmailInbox.IsAlert = false; } else { SpmailInbox.IsAlert = true; }

            if (Flag == "SEND") { SpmailInbox.IsDraft = false; } else { SpmailInbox.IsDraft = true; }

            _context.Add(SpmailInbox);
            _context.SaveChanges();

            string bannerimgname = "", bannerext = "", bannerPath = "", attachmentSize = "";

            TblSpmailAttachment stEvi = new TblSpmailAttachment();
            List<TblSpmailAttachment> lststEvi = new List<TblSpmailAttachment>();
            if (AttachmentFiles != null)
            {
                foreach (var formFile in AttachmentFiles)
                {
                    if (formFile.Length > 0)
                    {

                        var bannerName = Path.GetFileName(formFile.FileName);
                        bannerext = Path.GetExtension(formFile.FileName);


                        bannerimgname = Guid.NewGuid().ToString();

                        bannerPath = "/images/MailAttachments/" + SpmailInbox.PrmailId + "MLA_" + bannerimgname + bannerext;

                        using (var stream = new FileStream(Path.Combine("wwwroot/images/MailAttachments", SpmailInbox.PrmailId + "MLA_" + bannerimgname + bannerext), FileMode.Create))
                        {
                            formFile.CopyToAsync(stream);
                        }

                        lststEvi.Add(new TblSpmailAttachment
                        {
                            PrmailId = SpmailInbox.PrmailId,
                            AttachmentPath = SpmailInbox.PrmailId + "MLA_" + bannerimgname + bannerext,
                            AttachmentOrigName = bannerName,
                            AttachmentExtension = bannerext,
                        });

                    }
                }
                _context.AddRange(lststEvi);
                _context.SaveChanges();

            }


        }

        [HttpPost]
        public ActionResult FilterInboxList(string Flag)
        {
            int? suid = null;
            if (HttpContext.Session.GetString("UserTypeId") == "1")
            {
                suid = null;
            }
            else
            {
                suid = Convert.ToInt32(HttpContext.Session.GetString("Suid"));

            }
            SPMAilCommonModel spmailmodel = new SPMAilCommonModel();
            spmailmodel.LSTSpmailInbox = _context.TblSpmailInboxes.Where(p => p.RecieverId == suid).ToList();
            spmailmodel.LSTSpmailAttachment = _context.TblSpmailAttachments.ToList();

            spmailmodel.LSTRegistrationForm = _context.TblRegistrationForms.ToList();
            if (Flag == "IN")
            {
                spmailmodel.LSTSpmailInbox = _context.TblSpmailInboxes.Where(p => p.IsDeletedRc == false && p.RecieverId == suid && p.IsDraft == false).ToList();
            }
            else if (Flag == "SN")
            {
                spmailmodel.LSTSpmailInbox = _context.TblSpmailInboxes.Where(p => p.IsDeletedSn == false && p.SenderId == suid && p.IsDraft == false).ToList();
            }
            else if (Flag == "DR")
            {
                spmailmodel.LSTSpmailInbox = _context.TblSpmailInboxes.Where(p => p.IsDeletedSn == false && p.SenderId == suid && p.IsDraft == true).ToList();
            }
            else if (Flag == "CNT")
            {
                spmailmodel.LSTContactDetail = _context.TblContactDetails.Include(p => p.Su).Include(p => p.Category).Where(p => p.IsDeleted == false).ToList();
            }
            else if (Flag == "GRP")
            {
                spmailmodel.LSTContactGroup = _context.MstContactGroups.Where(p => p.Isdeleted == false).ToList();
                spmailmodel.LSTContacgGrpMapping = _context.TblContacgGrpMappings.Include(p => p.Member).ToList();
            }
            else if (Flag == "PGRP")
            {

                spmailmodel.LSTCommitteeDetail = _context.Stage1Stp2committeeDetails.Where(p => p.IsDeleted == false).ToList();

            }
            else
            {
                spmailmodel.LSTSpmailInbox = _context.TblSpmailInboxes.Where(p => p.IsDeletedRc == true && (p.RecieverId == suid || p.SenderId == suid)).ToList();
            }

            getcount(suid);

            ViewBag.Flag = Flag;

            return PartialView("/Views/SPMail/_PVInboxlist.cshtml", spmailmodel);

        }

        [HttpPost]
        public ActionResult RemoveToTrash(string Flag, int PRID, string MFlag)
        {


            if (Flag == "Trash")
            {
                TblSpmailInbox tb = _context.TblSpmailInboxes.Where(p => p.PrmailId == PRID).FirstOrDefault();
                if (MFlag == "IN")
                {
                    tb.IsDeletedRc = true;
                }
                else if (MFlag == "SN" || MFlag == "DR")
                {
                    tb.IsDeletedSn = true;
                }
                //tb.IsInTrash = true;
                tb.InTrashFlag = MFlag;
                _context.Update(tb);

            }
            else
            {

                TblSpmailInbox tb = _context.TblSpmailInboxes.Where(p => p.PrmailId == PRID).FirstOrDefault();
                if (tb.IsDeletedSn == true && tb.IsDeletedRc == true)
                {
                    _context.Remove(tb);
                }
                else
                {
                    if (MFlag == "IN")
                    {
                        tb.IsDeletedRc = true;
                    }
                    else if (MFlag == "SN" || MFlag == "DR")
                    {
                        tb.IsDeletedSn = true;
                    }
                    _context.Update(tb);
                }


            }

            _context.SaveChanges();
            int? suid = null;
            if (HttpContext.Session.GetString("UserTypeId") == "1")
            {
                suid = null;
            }
            else
            {
                suid = Convert.ToInt32(HttpContext.Session.GetString("Suid"));

            }
            getcount(suid);



            return PartialView("/Views/SPMail/_PVInboxLeftCnt.cshtml");

        }

        [HttpPost]
        public ActionResult GetAttachment(int PRID, string Flag)
        {

            SPMAilCommonModel spmailmodel = new SPMAilCommonModel();

            spmailmodel.LSTRegistrationForm = _context.TblRegistrationForms.ToList();

            spmailmodel.LSTSpmailInbox = _context.TblSpmailInboxes.Where(p => p.PrmailId == PRID).ToList();

            spmailmodel.LSTSpmailAttachment = _context.TblSpmailAttachments.Where(p => p.PrmailId == PRID).ToList();

            if (Flag == "IN")
            {
                TblSpmailInbox inisread = _context.TblSpmailInboxes.Where(p => p.PrmailId == PRID).FirstOrDefault();
                inisread.IsRead = true;
                _context.Update(inisread);
                _context.SaveChanges();
            }

            ViewBag.Flag = Flag;

            return PartialView("/Views/SPMail/_PVInboxAttachment.cshtml", spmailmodel);

        }

        [HttpPost]
        public ActionResult GetDraftAttachment(int PRID)
        {

            SPMAilCommonModel spmailmodel = new SPMAilCommonModel();

            spmailmodel.LSTSpmailAttachment = _context.TblSpmailAttachments.Where(p => p.PrmailId == PRID).ToList();



            return PartialView("/Views/SPMail/_PVDraftAttachment.cshtml", spmailmodel);

        }

        #region Addcontact

        public IActionResult AddContact()
        {
            if (Convert.ToString(HttpContext.Session.GetString("UserID")) == "")
            {
                return RedirectToAction("Login", "Logout");
            }

            TempData["EditRight"] = _context.RoleMenus.Where(c => c.RoleId == Convert.ToInt32(HttpContext.Session.GetString("RoleID")) && c.MenuId == Convert.ToInt32(HttpContext.Session.GetString("MenuId"))).Select(p => p.Edit).FirstOrDefault();

            TempData["DeleteRight"] = _context.RoleMenus.Where(c => c.RoleId == Convert.ToInt32(HttpContext.Session.GetString("RoleID")) && c.MenuId == Convert.ToInt32(HttpContext.Session.GetString("MenuId"))).Select(p => p.IsDeleted).FirstOrDefault();

            TempData["AddRight"] = _context.RoleMenus.Where(c => c.RoleId == Convert.ToInt32(HttpContext.Session.GetString("RoleID")) && c.MenuId == Convert.ToInt32(HttpContext.Session.GetString("MenuId"))).Select(p => p.AddNew).FirstOrDefault();

            SPMAilCommonModel spmailmodel = new SPMAilCommonModel();
            spmailmodel.LSTContactDetail = _context.TblContactDetails.Include(p => p.Su).Include(p => p.Category).Include(p => p.MemberType).Where(p => p.IsDeleted == false).ToList();

            ViewData["Category"] = new SelectList(_context.CommitteCategories.Where(p => p.IsDeleted == false), "Id", "Value");
            ViewData["School"] = new SelectList(_context.TblRegistrationForms.Where(p => p.IsVerified == true), "Suid", "SchoolName");
            return View(spmailmodel);
        }
        public JsonResult FillContact(int ContactId)
        {
            IList<TblContactDetail> TblContactDetail = new List<TblContactDetail>();
            TblContactDetail = (from c in _context.TblContactDetails
                                where c.MemberId == ContactId
                                select c).ToList();
            var sa = new JsonSerializerSettings();
            return Json(TblContactDetail, sa);
        }
        [HttpPost]
        public IActionResult SaveContact(int Suid,string SchoolName, int Category, string Name, string RandR, string Email, string contactNo, string flag, int ContactId)
        {
            try
            {
                if (flag == "I")
                {
                    TblContactDetail TblContDetail = new TblContactDetail();
                    if(Suid == 0)
                    {
                        TblContDetail.Suid = null;
                    }
                    else
                    {
                        TblContDetail.Suid = Suid;
                    }
                    TblContDetail.Name = Name;
                    TblContDetail.CategoryId = Category;
                    TblContDetail.RolesAndResponsibility = RandR;
                    TblContDetail.EmailId = Email;
                    TblContDetail.ContactNo = contactNo;
                    TblContDetail.MemberTypeId = 5;
                    TblContDetail.SchoolName = SchoolName;
                    _context.Add(TblContDetail);
                    _context.SaveChanges();
                }
                else
                {
                    TblContactDetail TblContDetail = (from c in _context.TblContactDetails
                                                      where c.MemberId == ContactId
                                                      select c).FirstOrDefault();
                    TblContDetail.Name = Name;
                    if (Suid == 0)
                    {
                        TblContDetail.Suid = null;
                    }
                    else
                    {
                        TblContDetail.Suid = Suid;
                    }
                    TblContDetail.CategoryId = Category;
                    TblContDetail.RolesAndResponsibility = RandR;
                    TblContDetail.EmailId = Email;
                    TblContDetail.ContactNo = contactNo;
                    TblContDetail.SchoolName = SchoolName;
                    _context.Update(TblContDetail);
                    _context.SaveChanges();
                }

                SPMAilCommonModel spmailmodel = new SPMAilCommonModel();
                spmailmodel.LSTContactDetail = _context.TblContactDetails.Include(p => p.Su).Include(p => p.Category).Include(p => p.MemberType).Where(p => p.IsDeleted == false).ToList();

                return PartialView("/Views/SPMail/_PVContactList.cshtml", spmailmodel);
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(AddContact));
            }

        }


        [HttpPost]
        public IActionResult RemoveContact(int ContactId)
        {

            TblContactDetail TblContDetail = (from c in _context.TblContactDetails
                                              where c.MemberId == ContactId
                                              select c).FirstOrDefault();
            TblContDetail.IsDeleted = true;
            _context.Update(TblContDetail);
            _context.SaveChanges();

            SPMAilCommonModel spmailmodel = new SPMAilCommonModel();
            spmailmodel.LSTContactDetail = _context.TblContactDetails.Include(p => p.Su).Include(p => p.Category).Where(p => p.IsDeleted == false).ToList();

            return PartialView("/Views/SPMail/_PVContactList.cshtml", spmailmodel);
        }
        #endregion

        #region AddGroup

        public IActionResult AddGroup()
        {
            if (Convert.ToString(HttpContext.Session.GetString("UserID")) == "")
            {
                return RedirectToAction("Login", "Logout");
            }

            TempData["EditRight"] = _context.RoleMenus.Where(c => c.RoleId == Convert.ToInt32(HttpContext.Session.GetString("RoleID")) && c.MenuId == Convert.ToInt32(HttpContext.Session.GetString("MenuId"))).Select(p => p.Edit).FirstOrDefault();

            TempData["DeleteRight"] = _context.RoleMenus.Where(c => c.RoleId == Convert.ToInt32(HttpContext.Session.GetString("RoleID")) && c.MenuId == Convert.ToInt32(HttpContext.Session.GetString("MenuId"))).Select(p => p.IsDeleted).FirstOrDefault();

            TempData["AddRight"] = _context.RoleMenus.Where(c => c.RoleId == Convert.ToInt32(HttpContext.Session.GetString("RoleID")) && c.MenuId == Convert.ToInt32(HttpContext.Session.GetString("MenuId"))).Select(p => p.AddNew).FirstOrDefault();

            SPMAilCommonModel spmailmodel = new SPMAilCommonModel();
            spmailmodel.LSTContactGroup = _context.MstContactGroups.Where(p => p.Isdeleted == false).ToList();
            spmailmodel.LSTContactDetail = _context.TblContactDetails.Include(p => p.Su).Include(p => p.Category).Where(p => p.IsDeleted == false).ToList();
            spmailmodel.LSTContacgGrpMapping = _context.TblContacgGrpMappings.Include(p => p.Su).Include(p => p.Member).Include(p => p.ContactGroup).ToList();

            return View(spmailmodel);
        }
        public JsonResult FillGroup(int GroupId)
        {
            IList<MstContactGroup> TblGroup = new List<MstContactGroup>();
            TblGroup = (from c in _context.MstContactGroups
                        where c.ContactGroupId == GroupId
                                select c).ToList();
            var sa = new JsonSerializerSettings();
            return Json(TblGroup, sa);
        }
        [HttpPost]
        public IActionResult SaveGroup(string Groupname, string flag, int groupId)
        {
            try
            {
                if (flag == "I")
                {
                    MstContactGroup TblGroup = new MstContactGroup();
                    TblGroup.GroupName = Groupname;
                    
                    _context.Add(TblGroup);
                    _context.SaveChanges();
                }
                else
                {
                    MstContactGroup TblGroup = (from c in _context.MstContactGroups
                                where c.ContactGroupId == groupId
                                                select c).FirstOrDefault();
                    TblGroup.GroupName = Groupname;
                    _context.Update(TblGroup);
                    _context.SaveChanges();
                }

                SPMAilCommonModel spmailmodel = new SPMAilCommonModel();
                spmailmodel.LSTContactGroup = _context.MstContactGroups.Where(p => p.Isdeleted == false).ToList();
                spmailmodel.LSTContacgGrpMapping = _context.TblContacgGrpMappings.Include(p => p.Su).Include(p => p.Member).Include(p => p.ContactGroup).ToList();
                return PartialView("/Views/SPMail/_PVGRPList.cshtml", spmailmodel);
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(AddContact));
            }

        }


        [HttpPost]
        public IActionResult RemoveGroup(int GroupId)
        {

            MstContactGroup TblgroDetail = (from c in _context.MstContactGroups
                                             where c.ContactGroupId == GroupId
                                              select c).FirstOrDefault();
            TblgroDetail.Isdeleted = true;
            _context.Update(TblgroDetail);
            _context.SaveChanges();

            SPMAilCommonModel spmailmodel = new SPMAilCommonModel();
            spmailmodel.LSTContactGroup = _context.MstContactGroups.Where(p => p.Isdeleted == false).ToList();
            spmailmodel.LSTContacgGrpMapping = _context.TblContacgGrpMappings.Include(p => p.Su).Include(p => p.Member).Include(p => p.ContactGroup).ToList();
            return PartialView("/Views/SPMail/_PVGRPList.cshtml", spmailmodel);
        }

        
        [HttpPost]
        public IActionResult LinkMember(string MemselectedItems,int ContactGroupId,string flag)
        {
            List<suidlist> suidlist = JsonConvert.DeserializeObject<List<suidlist>>(MemselectedItems);

            List<TblContacgGrpMapping> lstPart = new List<TblContacgGrpMapping>();

            foreach (var item1 in suidlist)
            {
                if(flag == "MEM")
                {
                    lstPart.Add(new TblContacgGrpMapping
                    {
                        ContactGroupId = ContactGroupId,
                        MemberId = item1.suid,
                        IsSchoolList = false

                    });
                }
                else
                {
                    lstPart.Add(new TblContacgGrpMapping
                    {
                        ContactGroupId = ContactGroupId,
                        Suid = item1.suid,
                        IsSchoolList = true

                    });
                }

            }

            _context.TblContacgGrpMappings.Where(m => m.ContactGroupId == ContactGroupId)
                                   .ToList().ForEach(p => _context.TblContacgGrpMappings.Remove(p));

            _context.AddRange(lstPart);
            _context.SaveChanges();


            SPMAilCommonModel spmailmodel = new SPMAilCommonModel();
            //if (flag == "MEM")
            //{
            //    spmailmodel.LSTContactDetail = _context.TblContactDetails.Include(p => p.Su).Include(p => p.Category).Where(p => p.IsDeleted == false).ToList();
            //}
            //else
            //{
            //    spmailmodel.LSTRegistrationForm = _context.TblRegistrationForms.Include(p => p.State).Include(p => p.District).Where(p => p.IsDeleted == false).ToList();
            //}
            //spmailmodel.LSTContacgGrpMapping = _context.TblContacgGrpMappings.Where(p => p.ContactGroupId == ContactGroupId).ToList();
            spmailmodel.LSTContactGroup = _context.MstContactGroups.Where(p => p.Isdeleted == false).ToList();
            spmailmodel.LSTContactDetail = _context.TblContactDetails.Include(p => p.Su).Include(p => p.Category).Where(p => p.IsDeleted == false).ToList();
            spmailmodel.LSTContacgGrpMapping = _context.TblContacgGrpMappings.Include(p => p.Su).Include(p => p.Member).Include(p => p.ContactGroup).ToList();
            ViewBag.MemSCHFlag = flag;
            return PartialView("/Views/SPMail/_PVGRPList.cshtml", spmailmodel);
        }

        [HttpPost]
        public IActionResult FillGroupContent(int GroupId,string flag)
        {
            try
            {
                SPMAilCommonModel spmailmodel = new SPMAilCommonModel();
                if (flag == "MEM")
                {
                    spmailmodel.LSTContactDetail = _context.TblContactDetails.Include(p => p.Su).Include(p => p.Category).Where(p => p.IsDeleted == false).ToList();
                }
                else
                {
                    spmailmodel.LSTRegistrationForm = _context.TblRegistrationForms.Include(p => p.State).Include(p => p.District).Where(p => p.IsDeleted == false).ToList();
                }
                spmailmodel.LSTContacgGrpMapping = _context.TblContacgGrpMappings.Where(p => p.ContactGroupId == GroupId).ToList();

                ViewBag.MemSCHFlag = flag;
                return PartialView("/Views/SPMail/_PVGetForGRPContactList.cshtml", spmailmodel);
            }
            catch (Exception e)
            {
                return RedirectToAction(nameof(AddContact));
            }

        }


        #endregion

        public IActionResult Alertlist()
        {
            SPMAilCommonModel spmailmodel = new SPMAilCommonModel();
            
            spmailmodel.LSTSpmailInbox = _context.TblSpmailInboxes.Where(p => p.IsDeletedRc == false && p.RecieverId == Convert.ToInt32(HttpContext.Session.GetString("Suid"))).ToList();
            return View(spmailmodel);
        }


    }
}
