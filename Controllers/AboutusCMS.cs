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

namespace EquidCMS.Controllers
{
    public class AboutusCMS : Controller
    {
        private readonly EquiDbContext _context;

        public AboutusCMS(EquiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Get the existing landing page data
            Tblaboutu Aboutus = _context.Tblaboutus.FirstOrDefault() ?? new Tblaboutu();
            return View(Aboutus);
        }

        [HttpPost]
        public IActionResult Update(Tblaboutu model)
        {
            if (ModelState.IsValid)
            {
                // Handle the update of Section 1 (Banner main heading and subheading)
                var existingAboutus = _context.Tblaboutus.FirstOrDefault();
                if (existingAboutus != null)
                {
                    existingAboutus.Bannerimglink = "";
                    existingAboutus.Bannermainheading = model.Bannermainheading;
                    existingAboutus.Bannersubheading = model.Bannersubheading;

                    existingAboutus.Secdesc = model.Secdesc;
                    existingAboutus.Sechdtxt = model.Sechdtxt;

                    existingAboutus.Thirdhdtxt = model.Thirdhdtxt;
                    existingAboutus.Thirddesc = model.Thirddesc;
                    existingAboutus.Thirdsubonehdtxt = model.Thirdsubonehdtxt;
                    existingAboutus.Thirdsubonedesc = model.Thirdsubonedesc;
                    existingAboutus.Thirdsubtwohdtxt = model.Thirdsubtwohdtxt;
                    existingAboutus.Thirdsubtwodesc = model.Thirdsubtwodesc;
                    _context.SaveChanges();
                }

                 return RedirectToAction("Create", "AboutusCMS");
            }

            return RedirectToAction("Create", "AboutusCMS");
        }


    }
}
