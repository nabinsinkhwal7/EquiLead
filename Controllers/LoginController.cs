using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using EquidCMS.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Web;
using System.Text;
using EquidCMS;

namespace RCH_Dynamic_Counselling.Controllers
{
    public class LoginController : Controller
    {

        private readonly EquiDbContext _context;

        public LoginController(EquiDbContext context)
        {
            _context = context;
        }

        // GET: LoginController
        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Captcha = RandomString(6);
            return View();
        }


        //// POST: Login/User Check
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(MstUser User)
        {
            try
            {

                string Username = User.UserName;

                string Password = User.Vcode;

                Password = Password.Replace(User.Password, "");

                byte[] decryptedPasswrd = Convert.FromBase64String(Password);

                Password = Encoding.UTF8.GetString(decryptedPasswrd);

                if (Password.Substring(7, 8).Contains("H"))
                    Password = Password.Substring(8);

                byte[] decryptedPasswrd1 = Convert.FromBase64String(Password);

                Password = Encoding.UTF8.GetString(decryptedPasswrd1);


                if (Username != null && Password != null)
                {
                    var chkUser = _context.MstUsers.Where(m => m.UserName == User.UserName && m.IsDeleted == 0 && m.IsActive == 1).FirstOrDefault();

                    if (chkUser != null && chkUser.UserName == User.UserName && chkUser.UserName.Equals(User.UserName))
                    {
                        if (_context.MstUsers.Where(m => m.UserName == User.UserName && m.IsDeleted == 0 && m.IsActive == 1).Count() > 0)
                        {
                            var user = Authenticate(User.UserName, Password);
                            if (user == null)
                            {
                                ViewBag.Message = "You have entered an invalid username or password";
                                ViewBag.Captcha = RandomString(6);
                                return View();
                            }
                            else
                            {

                                HttpContext.Session.SetString("UserName", user.UserName);
                                HttpContext.Session.SetString("Name", user.FullName);
                                //HttpContext.Session.SetString("EmailID", user.EmailId);
                                HttpContext.Session.SetString("RoleID", user.RoleId.ToString());
                                HttpContext.Session.SetString("UserID", user.UserId.ToString());
                                // HttpContext.Session.SetString("MenuId", "1");

                                MstUser ust = _context.MstUsers.Where(p => p.UserId == user.UserId).FirstOrDefault();

                                if (ust.NoofLogin == null)
                                {
                                    ust.NoofLogin = 1;
                                }
                                else
                                {
                                    ust.NoofLogin = ust.NoofLogin + 1;
                                }
                                 ust.LastLogin = System.DateTime.Now;

                                _context.Update(ust);
                                _context.SaveChanges();

                                if (Convert.ToInt32(HttpContext.Session.GetString("RoleID")) == 1) //PMH-COP
                                {
                                    return RedirectToAction("Index", "Home");
                                    
                                }
                                else
                                {
                                    // return RedirectToAction("AdminIndex", "Home
                                     return RedirectToAction("Index", "Home");
                                }

                                //return RedirectToAction("Index", "DashboardMap");
                                //return RedirectToAction("ANCDashboard", "ANCDashboard");

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


        public MstUser Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            var user = _context.MstUsers.SingleOrDefault(x => x.UserName == username && x.IsDeleted == 0);

            // check if username exists
            if (user == null)
                return null;

            //check if password is correctpassword
            var passwordHashed = CommonController.CreatePasswordHash(password);
            if (user.Password != passwordHashed)
                return null;
            //var CHKpassword = _context.MstUser.SingleOrDefault(x => x.Password == passwordHashed);
            //if (CHKpassword == null)
            //    return null;

            // authentication successful
            return user;
        }





        [AllowAnonymous]
        public ActionResult GenerateCaptcha()
        {
            return Json(RandomString(6));
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


        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Clears all session data
            return RedirectToAction("landingpagenw", "Home"); // Redirect to login page
        }




        public ActionResult Index()
        {
            return View();
        }

        // GET: LoginController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: LoginController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LoginController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LoginController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LoginController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LoginController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LoginController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
