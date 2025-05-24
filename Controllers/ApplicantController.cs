//using EquidCMS.Data;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using EquidCMS.Dto;
using EquidCMS.Models;
using EquidCMS.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EquidCMS.Controllers
{
    public class ApplicantController : Controller
    {
        private readonly EquiDbContext _context;
        public ApplicantController(EquiDbContext context)
        {
            _context = context;
        }

        [HttpGet("/refer")]
        public IActionResult Refer(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return RedirectToAction("Signup", "Applicant"); // If no referral code, go to normal signup
            }

            var referral = _context.ReferralCodes.FirstOrDefault(r => r.Code == code);
            if (referral == null)
            {
                return RedirectToAction("Signup", "Applicant"); // Invalid referral, go to normal signup
            }

            // Store the referral code in session for use during signup
            HttpContext.Session.SetString("ReferredBy", code);

            return RedirectToAction("Signup", "Applicant"); // Redirect to signup page
        }

        // GET: Show Email Verification Form
        [HttpGet]
        public IActionResult Signup()
        {
            return View();
        }
        // Displays the Forgot Password page
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // POST: Handle Forgot Password Request
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(string Email, [FromServices] EmailService emailService)
        {
            if (string.IsNullOrEmpty(Email))
            {
                ViewBag.ErrorMessage = "Email is required!";
                return View();
            }

            var existingUser = _context.Applicants.FirstOrDefault(u => u.Email == Email);
            if (existingUser == null)
            {
                ViewBag.ErrorMessage = "Data not found for the Email : "+ Email;
                return View();
            }
            // Generate OTP
            Random rnd = new Random();
            string otpCode = rnd.Next(100000, 999999).ToString();

            // Store OTP and Email in session
            HttpContext.Session.SetString("Email", Email);
            HttpContext.Session.SetString("OTP", otpCode);

            // Send OTP email
            string subject = "Your OTP Code for Password Reset";
            string body = $"Your OTP for password reset is: <strong>{otpCode}</strong>.";

            bool isEmailSent = await emailService.SendEmailAsync(Email, subject, body);
            if (!isEmailSent)
            {
                ViewBag.ErrorMessage = "Failed to send OTP. Please try again!";
                return View();
            }
            return RedirectToAction("ResetPassword");
        }

        // GET: Show OTP Verification Page
        [HttpGet]
        public IActionResult ResetPassword()
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            return View();
        }
        [HttpPost]
        public IActionResult VerifyToken(string token)
        {
            var sessionToken = HttpContext.Session.GetString("OTP");
            var email = HttpContext.Session.GetString("Email");

            if (token == sessionToken)
            {
                // OTP is correct, now allow password change
                return Json(new { success = true, message = "OTP Verified. You can now change your password." });
            }
            else
            {
                // OTP is incorrect
                return Json(new { success = false, message = "Invalid OTP. Please try again." });
            }
        }

        // POST: Handle Email Verification
        [HttpPost]
        public async Task<IActionResult> Signup(string Email, [FromServices] EmailService emailService)
        {
            if (string.IsNullOrEmpty(Email))
            {
                ViewBag.ErrorMessage = "Email is required!";
                return View();
            }

            var existingUser = _context.Applicants.FirstOrDefault(u => u.Email == Email);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "Email already registered!";
                return View();
            }

            // Generate OTP
            Random rnd = new Random();
            string otpCode = rnd.Next(100000, 999999).ToString();

            // Store in session
            HttpContext.Session.SetString("Email", Email);
            HttpContext.Session.SetString("OTP", otpCode);

            // Send OTP Email
            string subject = "Your OTP Code";
            string body = $"Your OTP for registration is: <strong>{otpCode}</strong>.";

            bool isEmailSent = await emailService.SendEmailAsync(Email, subject, body);
            if (!isEmailSent)
            {
                ViewBag.ErrorMessage = "Failed to send OTP. Please try again!";
                return View();
            }

            return RedirectToAction("VerifyOTP");
        }


        // GET: Show OTP Verification Page
        [HttpGet]
        public IActionResult VerifyOTP()
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");
            return View();
        }

        // POST: Verify OTP
        [HttpPost]
        public IActionResult VerifyOTP(string OTP)
        {
            var sessionOtp = HttpContext.Session.GetString("OTP");
            var email = HttpContext.Session.GetString("Email");

            if (OTP == sessionOtp)
            {
                // Remove OTP after verification
                HttpContext.Session.Remove("OTP");
                ViewBag.Email = HttpContext.Session.GetString("Email");

                return RedirectToAction("Register"); // Move to full registration
            }
            else
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                ViewBag.ErrorMessage = "Invalid OTP!";
                return View();
            }
        }

        // GET: Show Registration Form (after email verification)
        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Email = HttpContext.Session.GetString("Email");

            ViewData["Pronouns"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 6 && x.Active==true), "Description", "Description");
            ViewData["Gender"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 11 && x.Active==true), "Description", "Description");
            ViewData["CountryCode"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 7 && x.Active==true), "Description", "Description");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(ApplicantRegistrationModel model)
        {
            ViewData["Pronouns"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 6 && x.Active == true), "Description", "Description");
            ViewData["Gender"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 11 && x.Active==true), "Description", "Description");
            ViewData["CountryCode"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 7 && x.Active==true), "Description", "Description");
            if (!ModelState.IsValid)
            {
                ViewBag.Email = HttpContext.Session.GetString("Email");
                return View(model);
            }

            // Get email from session (already verified)
            string? email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("", "Email verification is required.");
                return View(model);
            }

            // Map ViewModel to Entity
            var applicant = new Applicant
            {
                Email = email,
                FullName = model.FullName, // Mapping Full Name
                Password = model.Password, // Hash password before saving
                PhoneNumber = model.PhoneNumber,
                CountryCode = model.CountryCode,
                Gender = model.Gender,
                ReferenceCode=model.ReferralCode,
                //JobRole = model.JobRole,
                //Industry = model.Industry,
                //ExperienceLevel = model.ExperienceLevel,
                //Skills = model.Skills, // You can save as a comma-separated string or in another format
                //PreferredJobType = model.PreferredJobType,
                LinkedinProfile = model.LinkedInProfile,
                Location = model.Location,
                //ResumeUrl = resumeFilePath, // If available
                //ReferralSource = model.ReferralSource, // If available
                //ReceiveJobAlerts = model.ReceiveJobAlerts, // Checkbox value
                CreatedAt = DateTime.UtcNow.ToLocalTime(), // Set CreatedAt timestamp
                UpdatedAt = DateTime.UtcNow.ToLocalTime() // Set UpdatedAt timestamp
            };
            //hashing a password
            var passwordHelper = new PasswordHelper();
            applicant.Password = passwordHelper.HashPassword(applicant.Password);
            // Save the applicant to the database (assuming you have a DbContext injected)
            _context.Applicants.Add(applicant);
            await _context.SaveChangesAsync();
            // Check if referral code exists in session
            string? referralCode = model.ReferralCode ?? HttpContext.Session.GetString("ReferredBy");
            if (!string.IsNullOrEmpty(referralCode))
            {
                var referral = await _context.ReferralCodes.FirstOrDefaultAsync(r => r.Code == referralCode);
                if (referral != null)
                {
                    referral.Count += 1; // Increase referral count
                    await _context.SaveChangesAsync();
                }
            }

            // Clear session after successful registration
            HttpContext.Session.Remove("ReferredBy");

            // Clear session after successful registration
            HttpContext.Session.Remove("Email");

            return RedirectToAction("Login");
        }


        // GET: Show OTP Verification Page
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Captcha = RandomString(6);
            return View();
        }
        private static Random random = new Random();
        public static string RandomString(int length)
        {
            string upperChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lowerChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToLower();
            string numericChar = "0123456789";
            string chars = upperChar + lowerChar + numericChar;
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        //// POST: Login/User Check
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(ApplicantLogin User)
        {
            try
            {

                string Username = User.Email;

                string Password = User.Password;


                if (Username != null && Password != null)
                {
                    var chkUser = _context.Applicants.Where(m => m.Email == User.Email).FirstOrDefault();

                    if (chkUser != null && chkUser.Email == User.Email && chkUser.Email.Equals(User.Email))
                    {
                            var authentic = Authenticate(User.Email,chkUser.Password, Password);
                            if (!authentic)
                            {
                                ViewBag.Message = "You have entered an invalid username or password";
                                ViewBag.Captcha = RandomString(6);
                                return View();
                            }
                            else
                            {

                                HttpContext.Session.SetString("UserName", chkUser.Email);
                                HttpContext.Session.SetString("Name", chkUser.FullName);
                                HttpContext.Session.SetString("ApplicantId", chkUser.ApplicantId.ToString());
                            //HttpContext.Session.SetString("EmailID", chkUser.EmailId);
                            // HttpContext.Session.SetString("MenuId", "1");
                            // Check if the user already has a referral code
                            var existingReferral = _context.ReferralCodes
                                    .FirstOrDefault(r => r.ApplicantId == chkUser.ApplicantId);

                                if (existingReferral != null)
                                {
                                    HttpContext.Session.SetString("ReferralCode", existingReferral.Code);
                                }

                                return RedirectToAction("landingpagenw", "Home");

                            }
                    }
                    else
                    {
                        ViewBag.Message = "You have entered an invalid username or password";
                        ViewBag.Captcha = RandomString(6);
                        return View();
                    }
                }
                else
                {
                    ViewBag.Message = "You have entered an invalid username or password";
                    ViewBag.Captcha = RandomString(6);
                    return View();
                }

            }
            catch (Exception)
            {

                throw;
            }
        }


        public Boolean Authenticate(string email, string hashedPassword, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedPassword))
                return false;

            var passwordHelper = new PasswordHelper();
            bool isPasswordValid = passwordHelper.VerifyPassword(password, hashedPassword);
            if (isPasswordValid) { 
                return true; }
            else
            {
                return false;
            }
        }

        public IActionResult Index()
        {     // Get the ApplicantId from session
            var applicantIdString = HttpContext.Session.GetString("ApplicantId");
            if (string.IsNullOrEmpty(applicantIdString))
            {
                // Redirect to login if session is missing
                return RedirectToAction("Login", "Applicant");
            }

            // Create dynamic ViewData for different skill types
            for (int i = 1; i < 4; i++)
            {
                var skillType = i;

                IEnumerable<SelectListItem> skillNames = new List<SelectListItem>();

                if (skillType == 1)
                    skillNames = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 9 && x.Active == true), "Description", "Description");
                else if (skillType == 2)
                    skillNames = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 12 && x.Active == true), "Description", "Description");
                else if (skillType == 3)
                    skillNames = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 13 && x.Active == true), "Description", "Description");

                ViewData[$"SkillName{i}"] = skillNames;
            }
            ViewData["EmploymentType"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 3 && x.Active == true), "Description", "Description");
            ViewData["Availability"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 5 && x.Active==true), "Lookupcode", "Description");
            ViewData["Pronouns"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 6 && x.Active==true), "Description", "Description");
            ViewData["CountryCode"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 7 && x.Active==true), "Description", "Description");
            ViewData["SkillType"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 8 && x.Active==true), "Lookupcode", "Description");
            ViewData["SkillName0"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 9 && x.Active==true), "Description", "Description");
            ViewData["SkillName1"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 12 && x.Active==true), "Description", "Description");
            ViewData["SkillName2"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 13 && x.Active==true), "Description", "Description");
            ViewData["LanguagesProficiency"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 10 && x.Active==true), "Lookupcode", "Description");
            ViewData["Gender"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 11 && x.Active==true), "Description", "Description");
            ViewData["FunctionalArea"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 14 && p.Active == true), "Lookupcode", "Description");
            ViewData["WorkMode"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 56 && p.Active == true), "Lookupcode", "Description");
            ViewData["PreferredSector"] = new SelectList(_context.MstLookups.Where(p => p.Lookupflag == 58 && p.Active == true), "Lookupcode", "Description");
            // Fetch applicant data from database
            var applicant = _context.Applicants
                .Include(x => x.ApplicantProfile)
                .Include(a => a.ApplicantWorkExperiences)
                .Include(a => a.ApplicantEducations)
                .Include(a => a.ApplicantSkills)
                .Include(a => a.ApplicantCertificationTranings)
                .Include(a => a.ApplicantLanguages)
                .Include(a => a.ApplicantVolunteerExperiences)
                .Include(a => a.ApplicantCareerPreference)
                .FirstOrDefault(a => a.ApplicantId == int.Parse(applicantIdString));

            if (applicant == null)
            {
                return NotFound();
            }

            // Map to DTO
            var profileDto = new ApplicantProfileDto
            {
                FullName = applicant.FullName,
                Email = applicant.Email,
                Pronouns = applicant.ApplicantProfile != null ? applicant.ApplicantProfile.Pronouns : "",
                CountryCode = applicant.CountryCode,
                PhoneNumber = applicant.PhoneNumber,
                Gender = applicant.Gender,
                Location = applicant.Location,
                JobSearchStatus = applicant.ApplicantProfile != null ? applicant.ApplicantProfile.JobSearchStatus : null,
                PersonalWebsite = applicant.ApplicantProfile != null ? applicant.ApplicantProfile.PersonalWebsite : "", // Add column if needed
                Headline = applicant.ApplicantProfile != null ? applicant.ApplicantProfile.Headline : "", // Add column if needed
                ShortBio = applicant.ApplicantProfile != null ? applicant.ApplicantProfile.ShortBio : "", // Add column if needed
                DesiredSalaryRange = applicant.ApplicantProfile != null ? applicant.ApplicantProfile.DesiredSalaryRange : "", // Add column if needed
                LinkedInProfile = applicant.LinkedinProfile,
                YearsOfExperence=applicant.YearsOfExperence,
                // New Fields
                InternationalExperience = applicant.ApplicantProfile != null ? applicant.ApplicantProfile.InternationalExperience : "",
                CommunityAdvocacyExperience = applicant.ApplicantProfile != null ? applicant.ApplicantProfile.CommunityAdvocacyExperience : "",


                WorkExperiences = applicant.ApplicantWorkExperiences.Select(w => new WorkExperienceDto
                {
                    JobTitle = w.JobTitle,
                    CompanyName = w.CompanyName,
                    Industry = w.Industry,
                    EmploymentType = w.EmploymentType,
                    StartDate = w.StartDate,
                    EndDate = w.EndDate,
                    KeyResponsibilities = w.KeyResponsibilities,
                    LeadershipImpact = w.LeadershipImpact
                }).ToList() ?? new List<WorkExperienceDto> { new WorkExperienceDto() },

                EducationHistory = applicant.ApplicantEducations.Select(e => new EducationDto
                {
                    Degree = e.Degree,
                    FieldOfStudy = e.FieldOfStudy,
                    InstitutionName = e.InstitutionName,
                    YearOfCompletion = e.YearOfCompletion
                }).ToList(),

                Certifications = applicant.ApplicantCertificationTranings.Select(c => new CertificationDto
                {
                    CertificationName = c.Name,
                    IssuingOrganization = c.IssuingOrganization,
                    YearEarned = c.YearEarned
                }).ToList(),
                Skills = applicant.ApplicantSkills.Select(l => new SkillDto
                {
                    SkillName = l.SkillName,
                    SkillType = l.SkillType,
                    SkillNames = !string.IsNullOrEmpty(l.SkillName)
                                                    ? l.SkillName.Split(new[] { ", " }, StringSplitOptions.None) // Split by comma and space
                                                    : new string[0],
                }).ToList(),
                Languages = applicant.ApplicantLanguages.Select(l => new LanguageDto
                {
                    Language = l.Language,
                    Proficiency = l.ProficiencyLevel,
                }).ToList(),

                CareerPreferences = new CareerPreferencesDto
                {
                    PreferredJobRole = applicant.ApplicantCareerPreference?.PreferredJobRole,
                    IndustriesOfInterest = applicant.ApplicantCareerPreference?.IndustriesOfInterest,
                    EmploymentTypePreference = applicant.ApplicantCareerPreference?.EmploymentTypePreference,
                    LeadershipAspirations = applicant.ApplicantCareerPreference?.LeadershipAspirations,
                    PreferredJobLocation = applicant.ApplicantCareerPreference?.PreferredJobLocation,
                    WillingToRelocate = applicant.ApplicantCareerPreference?.WillingToRelocate,
                    WorkMode=applicant.ApplicantCareerPreference.WorkMode,
                    PreferredSector=applicant.ApplicantCareerPreference.PreferredSector,
                    FunctionalArea=applicant.ApplicantCareerPreference.FunctionalArea
                },

                VolunteerExperiences = applicant.ApplicantVolunteerExperiences.Select(v => new VolunteerExperienceDto
                {
                    OrganizationName = v.OrganizationName,
                    Role = v.Role,
                    StartDate = v.StartDate,
                    EndDate = v.EndDate,
                    Description = v.Description
                }).ToList()
            };
            // List of all possible skill types (you can adjust this list based on your application requirements)
            var allSkillTypes = new List<short> { 1, 2, 3 };  // Example: 1 = Core Skills, 2 = Technical Skills, 3 = Soft Skills

            // Ensure that all skill types (1, 2, 3) are in the list, even if no skills were found for that type
            var finalSkillsList = allSkillTypes
                .Select(skillType =>
                {
                    // Find the group for this skillType, or create a default entry if not found
                    var existingSkill = profileDto.Skills.FirstOrDefault(g => g.SkillType == skillType);

                    if (existingSkill != null)
                    {
                        return existingSkill;  // If found, return the existing entry
                    }
                    else
                    {
                        // If no skill names are found for this SkillType, return an entry with an empty SkillName
                        return new SkillDto
                        {
                            SkillType = skillType,
                            SkillName = string.Empty  // Empty string for missing skills
                        };
                    }
                })
                .ToList();

            // Assign the final list of skills to the profileDto
            profileDto.Skills = finalSkillsList;
            var existingReferral = _context.ReferralCodes
                                  .FirstOrDefault(r => r.ApplicantId == int.Parse(applicantIdString));

            if (existingReferral != null)
            {
                HttpContext.Session.SetString("ReferralCode", existingReferral.Code);
                // Store the referral link in session
                string referralLink = $"{Request.Scheme}://{Request.Host}/refer?code={existingReferral.Code}";
                HttpContext.Session.SetString("ReferralLink", referralLink);

            }
            return View(profileDto);

        }

        [HttpPost]
        public async Task<IActionResult> Update(ApplicantProfileDto model)
        {
            try
            {
                var applicantId = int.Parse(HttpContext.Session.GetString("ApplicantId"));
                var applicant = _context.Applicants
                .Include(x => x.ApplicantProfile)
                .Include(a => a.ApplicantWorkExperiences)
                .Include(a => a.ApplicantEducations)
                .Include(a => a.ApplicantSkills)
                .Include(a => a.ApplicantCertificationTranings)
                .Include(a => a.ApplicantLanguages)
                .Include(a => a.ApplicantVolunteerExperiences).Where(x => x.ApplicantId == applicantId).FirstOrDefault();

                applicant.Email = model.Email;
                applicant.FullName = model.FullName; // Mapping Full Name
                applicant.PhoneNumber = model.PhoneNumber;
                applicant.CountryCode = model.CountryCode;
                applicant.Gender = model.Gender;
                applicant.YearsOfExperence = model.YearsOfExperence;
                applicant.LinkedinProfile = model.LinkedInProfile;
                applicant.Location = model.Location;
                applicant.UpdatedAt = DateTime.UtcNow.ToLocalTime();// Set UpdatedAt timestamp
                var applicantProfile = _context.ApplicantProfiles.Where(x => x.ApplicantId == applicantId).FirstOrDefault();
                if (applicantProfile == null)
                {
                    applicantProfile = new ApplicantProfile
                    {
                        ApplicantId = applicantId,
                        DesiredSalaryRange = model.DesiredSalaryRange,
                        Headline = model.Headline,
                        JobSearchStatus = model.JobSearchStatus,
                        PersonalWebsite = model.PersonalWebsite,
                        ShortBio = model.ShortBio,
                        Pronouns = model.Pronouns,
                        CreatedAt = DateTime.UtcNow.ToLocalTime(), // Set CreatedAt timestamp
                        UpdatedAt = DateTime.UtcNow.ToLocalTime(), // Set UpdatedAt timestamp
                        InternationalExperience = model.InternationalExperience,
                        CommunityAdvocacyExperience = model.CommunityAdvocacyExperience
                    };
                    // Save the applicant to the database (assuming you have a DbContext injected)
                    //_context.ApplicantProfiles.Add(applicantProfile);
                }
                else
                {
                    applicantProfile.DesiredSalaryRange = model.DesiredSalaryRange;
                    applicantProfile.Headline = model.Headline;
                    applicantProfile.JobSearchStatus = model.JobSearchStatus;
                    applicantProfile.PersonalWebsite = model.PersonalWebsite;
                    applicantProfile.ShortBio = model.ShortBio;
                    applicantProfile.Pronouns = model.Pronouns;
                    applicantProfile.UpdatedAt = DateTime.UtcNow.ToLocalTime(); // Set UpdatedAt timestamp

                    // Bind additional fields
                    applicantProfile.InternationalExperience = model.InternationalExperience;
                    applicantProfile.CommunityAdvocacyExperience = model.CommunityAdvocacyExperience;
                }

                applicant.ApplicantProfile = applicantProfile;

                // Save work experiences
                _context.ApplicantWorkExperiences.RemoveRange(applicant.ApplicantWorkExperiences);
                applicant.ApplicantWorkExperiences = [];
                foreach (var workExperienceModel in model.WorkExperiences)
                {
                    if (string.IsNullOrWhiteSpace(workExperienceModel.JobTitle) ||
                        string.IsNullOrWhiteSpace(workExperienceModel.CompanyName) ||
                        string.IsNullOrWhiteSpace(workExperienceModel.Industry) ||
                        string.IsNullOrWhiteSpace(workExperienceModel.EmploymentType))
                    {
                        continue;
                    }
                    // Create a new WorkExperience entry for each experience in the model
                    var workExperience = new ApplicantWorkExperience
                    {
                        ApplicantId = applicantId,
                        JobTitle = workExperienceModel.JobTitle,
                        CompanyName = workExperienceModel.CompanyName,
                        Industry = workExperienceModel.Industry,
                        EmploymentType = workExperienceModel.EmploymentType,
                        StartDate = workExperienceModel.StartDate,
                        EndDate = workExperienceModel.EndDate,
                        KeyResponsibilities = workExperienceModel.KeyResponsibilities,
                        LeadershipImpact = workExperienceModel.LeadershipImpact
                    };

                    // Add the work experience to the context
                    applicant.ApplicantWorkExperiences.Add(workExperience);
                }


                // Remove existing education history before adding new ones
                _context.ApplicantEducations.RemoveRange(applicant.ApplicantEducations);

                // Initialize the new list
                applicant.ApplicantEducations = new List<ApplicantEducation>();

                foreach (var edu in model.EducationHistory)
                {
                    // Validate required fields
                    if (string.IsNullOrWhiteSpace(edu.Degree) ||
                        string.IsNullOrWhiteSpace(edu.FieldOfStudy) ||
                        string.IsNullOrWhiteSpace(edu.InstitutionName) ||
                        edu.YearOfCompletion == null)
                    {
                        continue; // Skip invalid entries
                    }

                    // Create a new education entry
                    var education = new ApplicantEducation
                    {
                        ApplicantId = applicantId,
                        Degree = edu.Degree,
                        FieldOfStudy = edu.FieldOfStudy,
                        InstitutionName = edu.InstitutionName,
                        YearOfCompletion = edu.YearOfCompletion.Value
                    };

                    // Add to the applicant's collection
                    applicant.ApplicantEducations.Add(education);
                }

                // Remove existing certifications
                _context.ApplicantCertificationTranings.RemoveRange(applicant.ApplicantCertificationTranings);
                applicant.ApplicantCertificationTranings.Clear();

                // Add new certifications
                foreach (var cert in model.Certifications)
                {
                    if (!string.IsNullOrEmpty(cert.CertificationName) && !string.IsNullOrEmpty(cert.IssuingOrganization))
                    {
                        var newCert = new ApplicantCertificationTraning
                        {
                            ApplicantId = applicantId,
                            Name = cert.CertificationName,
                            IssuingOrganization = cert.IssuingOrganization,
                            YearEarned = cert.YearEarned
                        };
                        applicant.ApplicantCertificationTranings.Add(newCert);
                    }
                }

                // Remove existing skills
                _context.ApplicantSkills.RemoveRange(applicant.ApplicantSkills);
                applicant.ApplicantSkills.Clear();

                // Add new skills
                foreach (var skill in model.Skills)
                {

                    // Join the SkillNames array into a comma-separated string
                    if (skill.SkillNames != null && skill.SkillNames.Length > 0)
                    {
                        skill.SkillName = string.Join(", ", skill.SkillNames); // Joining SkillNames with commas
                    }


                    if (!string.IsNullOrEmpty(skill.SkillName))
                    {
                        var newSkill = new ApplicantSkill
                        {
                            ApplicantId = applicantId,
                            SkillName = skill.SkillName,
                            SkillType = skill.SkillType
                        };
                        applicant.ApplicantSkills.Add(newSkill);
                    }
                }
                // Remove existing languages
                _context.ApplicantLanguages.RemoveRange(applicant.ApplicantLanguages);
                applicant.ApplicantLanguages.Clear();

                // Add new languages
                foreach (var language in model.Languages)
                {
                    if (!string.IsNullOrEmpty(language.Language))
                    {
                        var newLanguage = new ApplicantLanguage
                        {
                            ApplicantId = applicantId,
                            Language = language.Language,
                            ProficiencyLevel = language.Proficiency
                        };
                        applicant.ApplicantLanguages.Add(newLanguage);
                    }
                }
                // Remove existing volunteer experiences
                _context.ApplicantVolunteerExperiences.RemoveRange(applicant.ApplicantVolunteerExperiences);
                applicant.ApplicantVolunteerExperiences.Clear();

                // Add new volunteer experiences
                foreach (var experience in model.VolunteerExperiences)
                {
                    if (!string.IsNullOrEmpty(experience.OrganizationName))
                    {
                        var newExperience = new ApplicantVolunteerExperience
                        {
                            ApplicantId = applicantId,
                            OrganizationName = experience.OrganizationName,
                            Role = experience.Role,
                            StartDate = experience.StartDate,
                            EndDate = experience.EndDate,
                            Description = experience.Description
                        };
                        applicant.ApplicantVolunteerExperiences.Add(newExperience);
                    }
                }

                // Check if the applicant has existing career preferences
                var careerPreferences = _context.ApplicantCareerPreferences
                    .FirstOrDefault(p => p.ApplicantId == applicantId);

                if (careerPreferences == null)
                {
                    careerPreferences = new ApplicantCareerPreference
                    {
                        ApplicantId = applicantId
                    };
                    _context.ApplicantCareerPreferences.Add(careerPreferences);
                }

                // Update values
                careerPreferences.PreferredJobRole = model.CareerPreferences.PreferredJobRole;
                careerPreferences.IndustriesOfInterest = model.CareerPreferences.IndustriesOfInterest;
                careerPreferences.EmploymentTypePreference = model.CareerPreferences.EmploymentTypePreference;
                careerPreferences.LeadershipAspirations = model.CareerPreferences.LeadershipAspirations;
                careerPreferences.PreferredJobLocation = model.CareerPreferences.PreferredJobLocation;
                careerPreferences.WillingToRelocate = model.CareerPreferences.WillingToRelocate;
                careerPreferences.WorkMode = model.CareerPreferences.WorkMode;
                careerPreferences.FunctionalArea = model.CareerPreferences.FunctionalArea;
                careerPreferences.PreferredSector = model.CareerPreferences.PreferredSector;

                _context.Applicants.Update(applicant);
                await _context.SaveChangesAsync();
                return RedirectToAction("index");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return RedirectToAction("index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Updateinner(ApplicantProfileDto model)
        {
            try
            {
                var applicantId = int.Parse(HttpContext.Session.GetString("ApplicantId"));
                var applicant = _context.Applicants
                .Include(x => x.ApplicantProfile)
                .Include(a => a.ApplicantWorkExperiences)
                .Include(a => a.ApplicantEducations)
                .Include(a => a.ApplicantSkills)
                .Include(a => a.ApplicantCertificationTranings)
                .Include(a => a.ApplicantLanguages)
                .Include(a => a.ApplicantVolunteerExperiences).Where(x => x.ApplicantId == applicantId).FirstOrDefault();

                applicant.Email = model.Email;
                applicant.FullName = model.FullName; // Mapping Full Name
                applicant.PhoneNumber = model.PhoneNumber;
                applicant.CountryCode = model.CountryCode;
                applicant.Gender = model.Gender;
                applicant.LinkedinProfile = model.LinkedInProfile;
                applicant.Location = model.Location;
                applicant.UpdatedAt = DateTime.UtcNow.ToLocalTime();// Set UpdatedAt timestamp
                var applicantProfile = _context.ApplicantProfiles.Where(x => x.ApplicantId == applicantId).FirstOrDefault();
                if (applicantProfile == null)
                {
                    applicantProfile = new ApplicantProfile
                    {
                        ApplicantId = applicantId,
                        DesiredSalaryRange = model.DesiredSalaryRange,
                        Headline = model.Headline,
                        JobSearchStatus = model.JobSearchStatus,
                        PersonalWebsite = model.PersonalWebsite,
                        ShortBio = model.ShortBio,
                        Pronouns = model.Pronouns,
                        CreatedAt = DateTime.UtcNow.ToLocalTime(), // Set CreatedAt timestamp
                        UpdatedAt = DateTime.UtcNow.ToLocalTime(), // Set UpdatedAt timestamp
                        InternationalExperience = model.InternationalExperience,
                        CommunityAdvocacyExperience = model.CommunityAdvocacyExperience
                    };
                    // Save the applicant to the database (assuming you have a DbContext injected)
                    //_context.ApplicantProfiles.Add(applicantProfile);
                }
                else
                {
                    applicantProfile.DesiredSalaryRange = model.DesiredSalaryRange;
                    applicantProfile.Headline = model.Headline;
                    applicantProfile.JobSearchStatus = model.JobSearchStatus;
                    applicantProfile.PersonalWebsite = model.PersonalWebsite;
                    applicantProfile.ShortBio = model.ShortBio;
                    applicantProfile.Pronouns = model.Pronouns;
                    applicantProfile.UpdatedAt = DateTime.UtcNow.ToLocalTime(); // Set UpdatedAt timestamp

                    // Bind additional fields
                    applicantProfile.InternationalExperience = model.InternationalExperience;
                    applicantProfile.CommunityAdvocacyExperience = model.CommunityAdvocacyExperience;
                }

                applicant.ApplicantProfile = applicantProfile;

                // Save work experiences
                _context.ApplicantWorkExperiences.RemoveRange(applicant.ApplicantWorkExperiences);
                applicant.ApplicantWorkExperiences = [];
                foreach (var workExperienceModel in model.WorkExperiences)
                {
                    if (string.IsNullOrWhiteSpace(workExperienceModel.JobTitle) ||
                        string.IsNullOrWhiteSpace(workExperienceModel.CompanyName) ||
                        string.IsNullOrWhiteSpace(workExperienceModel.Industry) ||
                        string.IsNullOrWhiteSpace(workExperienceModel.EmploymentType))
                    {
                        continue;
                    }
                    // Create a new WorkExperience entry for each experience in the model
                    var workExperience = new ApplicantWorkExperience
                    {
                        ApplicantId = applicantId,
                        JobTitle = workExperienceModel.JobTitle,
                        CompanyName = workExperienceModel.CompanyName,
                        Industry = workExperienceModel.Industry,
                        EmploymentType = workExperienceModel.EmploymentType,
                        StartDate = workExperienceModel.StartDate,
                        EndDate = workExperienceModel.EndDate,
                        KeyResponsibilities = workExperienceModel.KeyResponsibilities,
                        LeadershipImpact = workExperienceModel.LeadershipImpact
                    };

                    // Add the work experience to the context
                    applicant.ApplicantWorkExperiences.Add(workExperience);
                }


                // Remove existing education history before adding new ones
                _context.ApplicantEducations.RemoveRange(applicant.ApplicantEducations);

                // Initialize the new list
                applicant.ApplicantEducations = new List<ApplicantEducation>();

                foreach (var edu in model.EducationHistory)
                {
                    // Validate required fields
                    if (string.IsNullOrWhiteSpace(edu.Degree) ||
                        string.IsNullOrWhiteSpace(edu.FieldOfStudy) ||
                        string.IsNullOrWhiteSpace(edu.InstitutionName) ||
                        edu.YearOfCompletion == null)
                    {
                        continue; // Skip invalid entries
                    }

                    // Create a new education entry
                    var education = new ApplicantEducation
                    {
                        ApplicantId = applicantId,
                        Degree = edu.Degree,
                        FieldOfStudy = edu.FieldOfStudy,
                        InstitutionName = edu.InstitutionName,
                        YearOfCompletion = edu.YearOfCompletion.Value
                    };

                    // Add to the applicant's collection
                    applicant.ApplicantEducations.Add(education);
                }

                // Remove existing certifications
                _context.ApplicantCertificationTranings.RemoveRange(applicant.ApplicantCertificationTranings);
                applicant.ApplicantCertificationTranings.Clear();

                // Add new certifications
                foreach (var cert in model.Certifications)
                {
                    if (!string.IsNullOrEmpty(cert.CertificationName) && !string.IsNullOrEmpty(cert.IssuingOrganization))
                    {
                        var newCert = new ApplicantCertificationTraning
                        {
                            ApplicantId = applicantId,
                            Name = cert.CertificationName,
                            IssuingOrganization = cert.IssuingOrganization,
                            YearEarned = cert.YearEarned
                        };
                        applicant.ApplicantCertificationTranings.Add(newCert);
                    }
                }

                // Remove existing skills
                _context.ApplicantSkills.RemoveRange(applicant.ApplicantSkills);
                applicant.ApplicantSkills.Clear();

                // Add new skills
                foreach (var skill in model.Skills)
                {

                    // Join the SkillNames array into a comma-separated string
                    if (skill.SkillNames != null && skill.SkillNames.Length > 0)
                    {
                        skill.SkillName = string.Join(", ", skill.SkillNames); // Joining SkillNames with commas
                    }


                    if (!string.IsNullOrEmpty(skill.SkillName))
                    {
                        var newSkill = new ApplicantSkill
                        {
                            ApplicantId = applicantId,
                            SkillName = skill.SkillName,
                            SkillType = skill.SkillType
                        };
                        applicant.ApplicantSkills.Add(newSkill);
                    }
                }
                // Remove existing languages
                _context.ApplicantLanguages.RemoveRange(applicant.ApplicantLanguages);
                applicant.ApplicantLanguages.Clear();

                // Add new languages
                foreach (var language in model.Languages)
                {
                    if (!string.IsNullOrEmpty(language.Language))
                    {
                        var newLanguage = new ApplicantLanguage
                        {
                            ApplicantId = applicantId,
                            Language = language.Language,
                            ProficiencyLevel = language.Proficiency
                        };
                        applicant.ApplicantLanguages.Add(newLanguage);
                    }
                }
                // Remove existing volunteer experiences
                _context.ApplicantVolunteerExperiences.RemoveRange(applicant.ApplicantVolunteerExperiences);
                applicant.ApplicantVolunteerExperiences.Clear();

                // Add new volunteer experiences
                foreach (var experience in model.VolunteerExperiences)
                {
                    if (!string.IsNullOrEmpty(experience.OrganizationName))
                    {
                        var newExperience = new ApplicantVolunteerExperience
                        {
                            ApplicantId = applicantId,
                            OrganizationName = experience.OrganizationName,
                            Role = experience.Role,
                            StartDate = experience.StartDate,
                            EndDate = experience.EndDate,
                            Description = experience.Description
                        };
                        applicant.ApplicantVolunteerExperiences.Add(newExperience);
                    }
                }

                // Check if the applicant has existing career preferences
                var careerPreferences = _context.ApplicantCareerPreferences
                    .FirstOrDefault(p => p.ApplicantId == applicantId);

                if (careerPreferences == null)
                {
                    careerPreferences = new ApplicantCareerPreference
                    {
                        ApplicantId = applicantId
                    };
                    _context.ApplicantCareerPreferences.Add(careerPreferences);
                }

                // Update values
                careerPreferences.PreferredJobRole = model.CareerPreferences.PreferredJobRole;
                careerPreferences.IndustriesOfInterest = model.CareerPreferences.IndustriesOfInterest;
                careerPreferences.EmploymentTypePreference = model.CareerPreferences.EmploymentTypePreference;
                careerPreferences.LeadershipAspirations = model.CareerPreferences.LeadershipAspirations;
                careerPreferences.PreferredJobLocation = model.CareerPreferences.PreferredJobLocation;
                careerPreferences.WillingToRelocate = model.CareerPreferences.WillingToRelocate;

                _context.Applicants.Update(applicant);
                await _context.SaveChangesAsync();
                return RedirectToAction("Listing");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return RedirectToAction("Listing");
            }
        }


        [HttpGet]
        public IActionResult GenerateReferralCode()
        {
            // Get the ApplicantId from session
            var applicantIdString = HttpContext.Session.GetString("ApplicantId");
            if (applicantIdString == null)
            {
                return Unauthorized(); // Ensure user is logged in
            }
            var applicantId = int.Parse(applicantIdString); // Get logged-in user ID from session

                // Check if the user already has a referral code
                var existingCode = _context.ReferralCodes.FirstOrDefault(r => r.ApplicantId == applicantId);

                if (existingCode != null)
                {
                    return Json(new { referralCode = existingCode.Code }); // Return existing code
                }

                // Generate a new referral code
                string newReferralCode = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

                // Save the new referral code to the database
                var referralCode = new ReferralCode
                {
                    ApplicantId = (int)applicantId,
                    Code = newReferralCode,
                    CreatedAt = DateTime.Now
                };

                _context.ReferralCodes.Add(referralCode);
                _context.SaveChanges();
            // Store the referral link in session
            string referralLink = $"{Request.Scheme}://{Request.Host}/refer?code={referralCode}";
            HttpContext.Session.SetString("ReferralLink", referralLink);

            return RedirectToAction("Index", "Applicant");

        }

        [HttpPost]
        public IActionResult ChangePassword(string newPassword)
        {

            // Get the ApplicantId from session
            var applicantIdString = HttpContext.Session.GetString("ApplicantId");
            var email = HttpContext.Session.GetString("Email");
            if (string.IsNullOrEmpty(applicantIdString))
            {
                if (string.IsNullOrEmpty(email))
                {
                // Redirect to login if session is missing
                return RedirectToAction("Login", "Applicant");
            }
                applicantIdString = _context.Applicants.Where(x => x.Email == email).FirstOrDefault()?.ApplicantId.ToString();
            }
            var applicant = _context.Applicants.Where(x => x.ApplicantId == int.Parse(applicantIdString)).FirstOrDefault();
            if (applicant==null)
            {
                // Redirect to login if session is missing
                return RedirectToAction("Login", "Applicant");
            }
            else
            {
                var passwordHelper = new PasswordHelper();
                applicant.Password = passwordHelper.HashPassword(newPassword);
                _context.Applicants.Update(applicant);
                _context.SaveChanges();
                return RedirectToAction("Login", "Applicant");
            }
        }

        public IActionResult Listing(int page = 1, int pageSize = 10)
        {
            if (page <= 0) page = 1;

            // Get the total record count
            var totalRecords = _context.Applicants.Count();

            // Calculate total pages based on pageSize
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            // Fetch the data for the current page
            var applicants = _context.Applicants
                .OrderBy(a => a.ApplicantId) // Adjust the order by column as needed
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Pass the paginated data to the view
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalRecords = totalRecords;
            ViewBag.TotalPages = totalPages;

            return View(applicants);
        }



        public IActionResult InnerAllicantIndex(string id)
        {     // Get the ApplicantId from session
            var applicantIdString = id;
            HttpContext.Session.SetString("ApplicantId", id);

            // Create dynamic ViewData for different skill types
            for (int i = 1; i < 4; i++)
            {
                var skillType = i;

                IEnumerable<SelectListItem> skillNames = new List<SelectListItem>();

                if (skillType == 1)
                    skillNames = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 9 && x.Active == true), "Description", "Description");
                else if (skillType == 2)
                    skillNames = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 12 && x.Active == true), "Description", "Description");
                else if (skillType == 3)
                    skillNames = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 13 && x.Active == true), "Description", "Description");

                ViewData[$"SkillName{i}"] = skillNames;
            }
            ViewData["EmploymentType"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 3 && x.Active == true), "Description", "Description");
            ViewData["Availability"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 5 && x.Active == true), "Lookupcode", "Description");
            ViewData["Pronouns"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 6 && x.Active == true), "Description", "Description");
            ViewData["CountryCode"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 7 && x.Active == true), "Description", "Description");
            ViewData["SkillType"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 8 && x.Active == true), "Lookupcode", "Description");
            ViewData["SkillName0"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 9 && x.Active == true), "Description", "Description");
            ViewData["SkillName1"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 12 && x.Active == true), "Description", "Description");
            ViewData["SkillName2"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 13 && x.Active == true), "Description", "Description");
            ViewData["LanguagesProficiency"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 10 && x.Active == true), "Lookupcode", "Description");
            ViewData["Gender"] = new SelectList(_context.MstLookups.Where(x => x.Lookupflag == 11 && x.Active == true), "Description", "Description");
            // Fetch applicant data from database
            var applicant = _context.Applicants
                .Include(x => x.ApplicantProfile)
                .Include(a => a.ApplicantWorkExperiences)
                .Include(a => a.ApplicantEducations)
                .Include(a => a.ApplicantSkills)
                .Include(a => a.ApplicantCertificationTranings)
                .Include(a => a.ApplicantLanguages)
                .Include(a => a.ApplicantVolunteerExperiences)
                .Include(a => a.ApplicantCareerPreference)
                .FirstOrDefault(a => a.ApplicantId == int.Parse(id));

            if (applicant == null)
            {
                return NotFound();
            }

            // Map to DTO
            var profileDto = new ApplicantProfileDto
            {
                FullName = applicant.FullName,
                Email = applicant.Email,
                Pronouns = applicant.ApplicantProfile != null ? applicant.ApplicantProfile.Pronouns : "",
                CountryCode = applicant.CountryCode,
                PhoneNumber = applicant.PhoneNumber,
                Gender = applicant.Gender,
                Location = applicant.Location,
                JobSearchStatus = applicant.ApplicantProfile != null ? applicant.ApplicantProfile.JobSearchStatus : null,
                PersonalWebsite = applicant.ApplicantProfile != null ? applicant.ApplicantProfile.PersonalWebsite : "", // Add column if needed
                Headline = applicant.ApplicantProfile != null ? applicant.ApplicantProfile.Headline : "", // Add column if needed
                ShortBio = applicant.ApplicantProfile != null ? applicant.ApplicantProfile.ShortBio : "", // Add column if needed
                DesiredSalaryRange = applicant.ApplicantProfile != null ? applicant.ApplicantProfile.DesiredSalaryRange : "", // Add column if needed
                LinkedInProfile = applicant.LinkedinProfile,
                // New Fields
                InternationalExperience = applicant.ApplicantProfile != null ? applicant.ApplicantProfile.InternationalExperience : "",
                CommunityAdvocacyExperience = applicant.ApplicantProfile != null ? applicant.ApplicantProfile.CommunityAdvocacyExperience : "",


                WorkExperiences = applicant.ApplicantWorkExperiences.Select(w => new WorkExperienceDto
                {
                    JobTitle = w.JobTitle,
                    CompanyName = w.CompanyName,
                    Industry = w.Industry,
                    EmploymentType = w.EmploymentType,
                    StartDate = w.StartDate,
                    EndDate = w.EndDate,
                    KeyResponsibilities = w.KeyResponsibilities,
                    LeadershipImpact = w.LeadershipImpact
                }).ToList() ?? new List<WorkExperienceDto> { new WorkExperienceDto() },

                EducationHistory = applicant.ApplicantEducations.Select(e => new EducationDto
                {
                    Degree = e.Degree,
                    FieldOfStudy = e.FieldOfStudy,
                    InstitutionName = e.InstitutionName,
                    YearOfCompletion = e.YearOfCompletion
                }).ToList(),

                Certifications = applicant.ApplicantCertificationTranings.Select(c => new CertificationDto
                {
                    CertificationName = c.Name,
                    IssuingOrganization = c.IssuingOrganization,
                    YearEarned = c.YearEarned
                }).ToList(),
                Skills = applicant.ApplicantSkills.Select(l => new SkillDto
                {
                    SkillName = l.SkillName,
                    SkillType = l.SkillType,
                    SkillNames = !string.IsNullOrEmpty(l.SkillName)
                                                    ? l.SkillName.Split(new[] { ", " }, StringSplitOptions.None) // Split by comma and space
                                                    : new string[0],
                }).ToList(),
                Languages = applicant.ApplicantLanguages.Select(l => new LanguageDto
                {
                    Language = l.Language,
                    Proficiency = l.ProficiencyLevel,
                }).ToList(),

                CareerPreferences = new CareerPreferencesDto
                {
                    PreferredJobRole = applicant.ApplicantCareerPreference?.PreferredJobRole,
                    IndustriesOfInterest = applicant.ApplicantCareerPreference?.IndustriesOfInterest,
                    EmploymentTypePreference = applicant.ApplicantCareerPreference?.EmploymentTypePreference,
                    LeadershipAspirations = applicant.ApplicantCareerPreference?.LeadershipAspirations,
                    PreferredJobLocation = applicant.ApplicantCareerPreference?.PreferredJobLocation,
                    WillingToRelocate = applicant.ApplicantCareerPreference?.WillingToRelocate
                },

                VolunteerExperiences = applicant.ApplicantVolunteerExperiences.Select(v => new VolunteerExperienceDto
                {
                    OrganizationName = v.OrganizationName,
                    Role = v.Role,
                    StartDate = v.StartDate,
                    EndDate = v.EndDate,
                    Description = v.Description
                }).ToList()
            };
            // List of all possible skill types (you can adjust this list based on your application requirements)
            var allSkillTypes = new List<short> { 1, 2, 3 };  // Example: 1 = Core Skills, 2 = Technical Skills, 3 = Soft Skills

            // Ensure that all skill types (1, 2, 3) are in the list, even if no skills were found for that type
            var finalSkillsList = allSkillTypes
                .Select(skillType =>
                {
                    // Find the group for this skillType, or create a default entry if not found
                    var existingSkill = profileDto.Skills.FirstOrDefault(g => g.SkillType == skillType);

                    if (existingSkill != null)
                    {
                        return existingSkill;  // If found, return the existing entry
                    }
                    else
                    {
                        // If no skill names are found for this SkillType, return an entry with an empty SkillName
                        return new SkillDto
                        {
                            SkillType = skillType,
                            SkillName = string.Empty  // Empty string for missing skills
                        };
                    }
                })
                .ToList();

            // Assign the final list of skills to the profileDto
            profileDto.Skills = finalSkillsList;
            var existingReferral = _context.ReferralCodes
                                  .FirstOrDefault(r => r.ApplicantId == int.Parse(applicantIdString));

            if (existingReferral != null)
            {
                HttpContext.Session.SetString("ReferralCode", existingReferral.Code);
                // Store the referral link in session
                string referralLink = $"{Request.Scheme}://{Request.Host}/refer?code={existingReferral.Code}";
                HttpContext.Session.SetString("ReferralLink", referralLink);

            }
            return View(profileDto);

        }

        [HttpPost]
        public IActionResult DeleteApplicant(int id)
        {
            var record = _context.Applicants.Find(id);
            if (record != null)
            {
                record.IsMigrated = true;  // Set the flag to true
                _context.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Record not found." });
        }


        [HttpPost]
        public IActionResult ExportApplicants()
        {
            try
            {
                // First get the data from database with all includes
                var applicants = _context.Applicants
                    .Include(a => a.ApplicantProfile)
                    .Include(a => a.ApplicantCareerPreference)
                    .Include(a => a.ApplicantSkills)
                    .Include(a => a.ApplicantLanguages)
                    .Include(a => a.ApplicantCertificationTranings)
                    .Include(a => a.ApplicantEducations)
                    .Include(a => a.ApplicantWorkExperiences)
                    .Include(a => a.ApplicantVolunteerExperiences)
                    .Where(p => p.IsMigrated == false)
                    .ToList(); // Materialize the query first

                // Then perform the complex transformations in memory
                var applist = applicants.Select(a => new ApplicantExportDto
                {
                    // Basic Information
                    FullName = a.FullName,
                    Email = a.Email,
                    Pronouns = a.ApplicantProfile?.Pronouns ?? "",
                    Gender = a.Gender == "2" ? "Female" : (a.Gender == "1" ? "Male" : a.Gender),
                    CountryCode = a.CountryCode,
                    PhoneNumber = a.PhoneNumber,
                    Location = a.Location,
                    LinkedInProfile = a.LinkedinProfile,
                    PersonalWebsite = a.ApplicantProfile?.PersonalWebsite ?? "",
                    ReferenceCode = a.ReferenceCode,
                    CreatedAt = a.CreatedAt?.ToString("yyyy-MM-dd") ?? "",
                    YearsOfExperience = a.YearsOfExperence?.ToString() ?? "0",

                    // Profile Information
                    Headline = a.ApplicantProfile?.Headline ?? "",
                    JobSearchStatus = a.ApplicantProfile != null
                        ? (a.ApplicantProfile.JobSearchStatus == 1 ? "Actively Looking"
                          : a.ApplicantProfile.JobSearchStatus == 2 ? "Open to Opportunities"
                          : a.ApplicantProfile.JobSearchStatus == 3 ? "Not Looking, but Interested in Networking"
                          : "")
                        : "",
                    DesiredSalaryRange = a.ApplicantProfile?.DesiredSalaryRange ?? "",
                    ShortBio = a.ApplicantProfile?.ShortBio ?? "",
                    InternationalExperience = a.ApplicantProfile?.InternationalExperience ?? "",
                    CommunityAdvocacyExperience = a.ApplicantProfile?.CommunityAdvocacyExperience ?? "",

                    // Career Preferences
                    PreferredJobRole = a.ApplicantCareerPreference?.PreferredJobRole ?? "",
                    IndustriesOfInterest = a.ApplicantCareerPreference?.IndustriesOfInterest ?? "",
                    LeadershipAspirations = a.ApplicantCareerPreference?.LeadershipAspirations ?? "",
                    EmploymentTypePreference = a.ApplicantCareerPreference?.EmploymentTypePreference ?? "",
                    PreferredJobLocation = a.ApplicantCareerPreference?.PreferredJobLocation ?? "",
                    WillingToRelocate = a.ApplicantCareerPreference?.WillingToRelocate ?? "",

                    // Work Experience
                    WorkExperiences = string.Join(";\n", a.ApplicantWorkExperiences.Select(w =>
                        $"{w.JobTitle} at {w.CompanyName} ({w.Industry ?? "N/A"}) | " +
                        $"{w.EmploymentType ?? "N/A"} | " +
                        $"{w.StartDate.ToString("yyyy-MM-dd")} to {(w.EndDate.HasValue ? w.EndDate.Value.ToString("yyyy-MM-dd") : "Present")}\n" +
                        $"Responsibilities: {w.KeyResponsibilities ?? "N/A"}\n" +
                        $"Leadership Impact: {w.LeadershipImpact ?? "N/A"}"
                    )),

                    // Education
                    Educations = string.Join(";\n", a.ApplicantEducations.Select(e =>
                        $"{e.Degree ?? "N/A"} in {e.FieldOfStudy ?? "N/A"} | " +
                        $"{e.InstitutionName ?? "N/A"} | " +
                        $"Completed: {e.YearOfCompletion?.ToString() ?? "N/A"}"
                    )),

                    // Certifications
                    Certifications = string.Join(";\n", a.ApplicantCertificationTranings.Select(c =>
                        $"{c.Name ?? "N/A"} | {c.IssuingOrganization ?? "N/A"} | " +
                        $"Year: {c.YearEarned?.ToString() ?? "N/A"}"
                    )),

                    // Volunteer Experience
                    VolunteerExperiences = string.Join(";\n", a.ApplicantVolunteerExperiences.Select(v =>
                        $"{v.Role ?? "N/A"} at {v.OrganizationName ?? "N/A"} | " +
                        $"{v.StartDate.ToString()} to {(v.EndDate.HasValue ? v.EndDate.Value.ToString("yyyy-MM-dd") : "Present")}\n" +
                        $"Description: {v.Description ?? "N/A"}"
                    )),

                    // Skills
                    Skills = string.Join("\n", a.ApplicantSkills
                            .GroupBy(s => s.SkillType)
                             .Select(g => $"{g.Key}:\n{string.Join(", ", g.Select(s => s.SkillName))}")),

                    // Languages - now works because we're in memory
                    Languages = string.Join(", ", a.ApplicantLanguages?.Select(l =>
                    {
                        short levelCode = l.ProficiencyLevel;
                        string level = levelCode switch
                        {
                            1 => "Basic",
                            2 => "Intermediate",
                            3 => "Fluent",
                            4 => "Native",
                            _ => "Basic"
                        };
                        return $"{l.Language ?? "N/A"} ({level})";
                    }) ?? new List<string>()),

                    // Migration Status
                    IsMigrated = a.IsMigrated?.ToString() ?? "false"
                }).ToList();

                if (!applist.Any())
                {
                    return NotFound("No applicants found to export.");
                }

                var content = ExcelExportHelper.ExportToExcel(applist, "Applicants");

                return File(
                    content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"Applicants_Export_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Export failed: {ex.Message}");
            }
        }
        //[HttpPost]
        //public IActionResult ExportApplicants()
        //{
        //    try
        //    {
        //        var applist = _context.Applicants
        //            .Include(a => a.ApplicantProfile)
        //            .Include(a => a.ApplicantCareerPreference)
        //            .Include(a => a.ApplicantSkills)
        //            .Include(a => a.ApplicantLanguages)
        //            .Include(a => a.ApplicantCertificationTranings)
        //            .Include(a => a.ApplicantEducations)
        //            .Include(a => a.ApplicantWorkExperiences)
        //            .Include(a => a.ApplicantVolunteerExperiences)
        //            .Where(p => p.IsMigrated == false)
        //            .Select(a => new ApplicantExportDto
        //            {
        //                // Basic Information
        //                //ApplicantId = a.ApplicantId,
        //                FullName = a.FullName,
        //                Email = a.Email,
        //                Pronouns = a.ApplicantProfile != null ? a.ApplicantProfile.Pronouns : "",
        //                Gender = a.Gender == "2" ? "Female" : (a.Gender == "1" ? "Male" : a.Gender),
        //                CountryCode = a.CountryCode,
        //                PhoneNumber = a.PhoneNumber,
        //                Location = a.Location,
        //                LinkedInProfile = a.LinkedinProfile,
        //                PersonalWebsite = a.ApplicantProfile.PersonalWebsite ?? "",
        //                ReferenceCode = a.ReferenceCode,
        //                CreatedAt = a.CreatedAt.HasValue ? a.CreatedAt.Value.ToString("yyyy-MM-dd") : "",
        //               // UpdatedAt = a.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
        //                YearsOfExperience = a.YearsOfExperence.ToString() ?? "0",

        //                // Profile Information
        //                Headline = a.ApplicantProfile.Headline ?? "",
        //                JobSearchStatus = a.ApplicantProfile != null
        //              ? (a.ApplicantProfile.JobSearchStatus == 1 ? "Actively Looking"
        //                : a.ApplicantProfile.JobSearchStatus == 2 ? "Open to Opportunities"
        //                : a.ApplicantProfile.JobSearchStatus == 3 ? "Not Looking, but Interested in Networking"
        //                : "")
        //              : "",
        //              DesiredSalaryRange = a.ApplicantProfile.DesiredSalaryRange ?? "",
        //                ShortBio = a.ApplicantProfile.ShortBio ?? "",
        //                InternationalExperience = a.ApplicantProfile.InternationalExperience ?? "",
        //                CommunityAdvocacyExperience = a.ApplicantProfile.CommunityAdvocacyExperience ?? "",

        //                // Career Preferences
        //                PreferredJobRole = a.ApplicantCareerPreference.PreferredJobRole ?? "",
        //                IndustriesOfInterest = a.ApplicantCareerPreference.IndustriesOfInterest ?? "",
        //                LeadershipAspirations = a.ApplicantCareerPreference.LeadershipAspirations ?? "",
        //                EmploymentTypePreference = a.ApplicantCareerPreference.EmploymentTypePreference ?? "",
        //                PreferredJobLocation = a.ApplicantCareerPreference.PreferredJobLocation ?? "",
        //                WillingToRelocate = a.ApplicantCareerPreference.WillingToRelocate ?? "",

        //                // Work Experience
        //                WorkExperiences = string.Join(";\n", a.ApplicantWorkExperiences.Select(w =>
        //                    $"{w.JobTitle} at {w.CompanyName} ({w.Industry}) | " +
        //                    $"{w.EmploymentType} | " +
        //                    $"{w.StartDate.ToString("yyyy-MM-dd")} to {(w.EndDate.HasValue ? w.EndDate.Value.ToString("yyyy-MM-dd") : "Present")}\n" +
        //                    $"Responsibilities: {w.KeyResponsibilities}\n" +
        //                    $"Leadership Impact: {w.LeadershipImpact}"
        //                )),

        //                // Education
        //                Educations = string.Join(";\n", a.ApplicantEducations.Select(e =>
        //                    $"{e.Degree} in {e.FieldOfStudy} | " +
        //                    $"{e.InstitutionName} | " +
        //                    $"Completed: {e.YearOfCompletion}"
        //                )),

        //                // Certifications
        //                Certifications = string.Join(";\n", a.ApplicantCertificationTranings.Select(c =>
        //                    $"{c.Name} | {c.IssuingOrganization} | " +
        //                    $"Year: {c.YearEarned}"
        //                )),

        //                //Volunteer Experience
        //                VolunteerExperiences = string.Join(";\n", a.ApplicantVolunteerExperiences.Select(v =>
        //                    $"{v.Role} at {v.OrganizationName} | " +
        //                    $"{v.StartDate.ToString()} to {(v.EndDate.HasValue ? v.EndDate.Value.ToString("yyyy-MM-dd") : "Present")}\n" +
        //                    $"Description: {v.Description}"
        //                )),

        //                // Skills
        //                Skills = string.Join("\n", a.ApplicantSkills
        //                    .GroupBy(s => s.SkillType)
        //                    .Select(g => $"{g.Key}:\n{string.Join(", ", g.Select(s => s.SkillName))}")),

        //                // Languages
        //                Languages = string.Join(", ", a.ApplicantLanguages.Select(l =>
        //                {
        //                    int levelCode = (int)l.ProficiencyLevel;
        //                    string level = levelCode switch
        //                    {
        //                        1 => "Basic",
        //                        2 => "Intermediate",
        //                        3 => "Fluent",
        //                        4 => "Native",
        //                        _ => "Unknown"
        //                    };
        //                    return $"{l.Language} ({level})";
        //                })),


        //                // Migration Status
        //                IsMigrated = a.IsMigrated.ToString() ?? "false"
        //            })
        //            .ToList();

        //        if (applist == null || !applist.Any())
        //        {
        //            return NotFound("No applicants found to export.");
        //        }

        //        var content = ExcelExportHelper.ExportToExcel(applist, "Applicants");

        //        return File(
        //            content,
        //            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //            $"Applicants_Export_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
        //        );
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Export failed: {ex.Message}");
        //    }
        //}




    }

}
