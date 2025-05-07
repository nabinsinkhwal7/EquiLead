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
    public class LandingpageCMS : Controller
    {
        private readonly EquiDbContext _context;

        public LandingpageCMS(EquiDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Create()
        {
            // Get the existing landing page data
            Tbllandingpage landingPage = _context.Tbllandingpages.FirstOrDefault() ?? new Tbllandingpage();
            ViewData["Jobs"] = _context.Tbljobs.Take(4).ToList();
            ViewData["Events"] = _context.Tblevents.Take(4).ToList();
            return View(landingPage);
        }

        [HttpPost]
        public IActionResult UpdateSection1(Tbllandingpage model)
        {
            if (ModelState.IsValid)
            {
                // Handle the update of Section 1 (Banner main heading and subheading)
                var existingLandingPage = _context.Tbllandingpages.FirstOrDefault();
                if (existingLandingPage != null)
                {
                    existingLandingPage.Bannermainheading = model.Bannermainheading;
                    existingLandingPage.Bannersubheading = model.Bannersubheading;
                    existingLandingPage.Bannerbtnlfttext = model.Bannerbtnlfttext;
                    existingLandingPage.Bannerbtnrhstext = model.Bannerbtnrhstext;
                    _context.SaveChanges();
                }

                // Return the view with the updated model so the user stays on the same page
                ViewData["Jobs"] = _context.Tbljobs.Take(4).ToList();
                ViewData["Events"] = _context.Tblevents.Take(4).ToList();
                return RedirectToAction("Create", "LandingpageCMS");
            }

            // If the model state is not valid, return the same view with validation messages
            ViewData["Jobs"] = _context.Tbljobs.Take(4).ToList();
            ViewData["Events"] = _context.Tblevents.Take(4).ToList();
            return RedirectToAction("Create", "LandingpageCMS");
        }

        [HttpPost]
        public IActionResult UpdateSection2(Tbllandingpage model, IFormFile seccrd1Upload, IFormFile seccrd2Upload, IFormFile seccrd3Upload)
        {
            //if (ModelState.IsValid)
            //{
                // Handle the update of Section 1 (Banner main heading and subheading)
                var existingLandingPage = _context.Tbllandingpages.FirstOrDefault();
                if (existingLandingPage != null)
                {

                    // Handle the first file upload (Image)
                    if (seccrd1Upload != null && seccrd1Upload.Length > 0)
                    {
                        // Generate a unique file name to avoid conflicts
                        var imageFileName = Path.GetFileNameWithoutExtension(seccrd1Upload.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(seccrd1Upload.FileName);

                        // Define the full file path
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "img", imageFileName);

                        // Ensure the directory exists
                        var directoryPath = Path.GetDirectoryName(imagePath);
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        try
                        {
                            // Save the file to the specified path
                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                seccrd1Upload.CopyToAsync(stream);
                            }

                            // Save the full file path in the model (not just the file name)
                            existingLandingPage.Sectile1img = imagePath;
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("FileUploadError", "An error occurred while uploading the image: " + ex.Message);
                            return View("Create", model);
                        }
                    }

                    // Handle the first file upload (Image)
                    if (seccrd3Upload != null && seccrd3Upload.Length > 0)
                    {
                        // Generate a unique file name to avoid conflicts
                        var imageFileName = Path.GetFileNameWithoutExtension(seccrd3Upload.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(seccrd3Upload.FileName);

                        // Define the full file path
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "img", imageFileName);

                        // Ensure the directory exists
                        var directoryPath = Path.GetDirectoryName(imagePath);
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        try
                        {
                            // Save the file to the specified path
                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                seccrd3Upload.CopyToAsync(stream);
                            }

                            // Save the full file path in the model (not just the file name)
                            existingLandingPage.Sectile3img = imagePath;
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("FileUploadError", "An error occurred while uploading the image: " + ex.Message);
                            return View("Create", model);
                        }
                    }

                    // Handle the first file upload (Image)
                    if (seccrd2Upload != null && seccrd2Upload.Length > 0)
                    {
                        // Generate a unique file name to avoid conflicts
                        var imageFileName = Path.GetFileNameWithoutExtension(seccrd2Upload.FileName) + "_" + Guid.NewGuid().ToString() + Path.GetExtension(seccrd2Upload.FileName);

                        // Define the full file path
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "img", imageFileName);

                        // Ensure the directory exists
                        var directoryPath = Path.GetDirectoryName(imagePath);
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        try
                        {
                            // Save the file to the specified path
                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                seccrd2Upload.CopyToAsync(stream);
                            }

                            // Save the full file path in the model (not just the file name)
                            existingLandingPage.Sectile2img = imagePath;
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("FileUploadError", "An error occurred while uploading the image: " + ex.Message);
                            return View("Create", model);
                        }
                    }


                    existingLandingPage.Sechdtxt = model.Sechdtxt;
                    existingLandingPage.Secspn = model.Secspn;
                    existingLandingPage.Secdesc = model.Secdesc;
                    existingLandingPage.Sectile1hd = model.Sectile1hd;
                    existingLandingPage.Sectile2hd = model.Sectile2hd;
                    existingLandingPage.Sectile3hd = model.Sectile3hd;
                    _context.SaveChanges();
                }

            // Return the view with the updated model so the user stays on the same page
            //return View("Create", model);
            //}

            // If the model state is not valid, return the same view with validation messages
            ViewData["Jobs"] = _context.Tbljobs.Take(4).ToList();
            ViewData["Events"] = _context.Tblevents.Take(4).ToList();
            return View("Create", model);
        }

        [HttpPost]
        public IActionResult UpdateSection3(Tbllandingpage model)
        {
            //if (ModelState.IsValid)
            //{
                // Handle the update of Section 1 (Banner main heading and subheading)
                var existingLandingPage = _context.Tbllandingpages.FirstOrDefault();
                if (existingLandingPage != null)
                {
                    existingLandingPage.Sec3mnhd = model.Sec3mnhd;
                    existingLandingPage.Sec3mnbtntext = model.Sec3mnbtntext;
                    _context.SaveChanges();
                }

            // Return the view with the updated model so the user stays on the same page
            //return View("Create", model);
            //}

            // If the model state is not valid, return the same view with validation messages
            ViewData["Jobs"] = _context.Tbljobs.Take(4).ToList();
            ViewData["Events"] = _context.Tblevents.Take(4).ToList();
            return View("Create", model);
        }

        [HttpPost]
        public IActionResult UpdateSection4(Tbllandingpage model)
        {
            //if (ModelState.IsValid)
            //{
                // Handle the update of Section 1 (Banner main heading and subheading)
                var existingLandingPage = _context.Tbllandingpages.FirstOrDefault();
                if (existingLandingPage != null)
                {
                    existingLandingPage.Sec4mnhd = model.Sec4mnhd;
                    existingLandingPage.Sec4mndsc = model.Sec4mndsc;
                    existingLandingPage.Sec4mnbtntxt = model.Sec4mnbtntxt;
                    _context.SaveChanges();
                }

            // Return the view with the updated model so the user stays on the same page
            // return View("Create", model);
            //}

            // If the model state is not valid, return the same view with validation messages
            ViewData["Jobs"] = _context.Tbljobs.Take(4).ToList();
            ViewData["Events"] = _context.Tblevents.Take(4).ToList();
            return View("Create", model);
        }

        [HttpPost]
        public IActionResult UpdateSection5(Tbllandingpage model)
        {
            //if (ModelState.IsValid)
            //{
                // Handle the update of Section 1 (Banner main heading and subheading)
                var existingLandingPage = _context.Tbllandingpages.FirstOrDefault();
                if (existingLandingPage != null)
                {
                    existingLandingPage.Sec5mnhd = model.Sec5mnhd;
                    existingLandingPage.Sec5mnbtntext = model.Sec5mnbtntext;
                    _context.SaveChanges();
                }

                // Return the view with the updated model so the user stays on the same page
                return View("Create", model);
            //}

            // If the model state is not valid, return the same view with validation messages
            ViewData["Jobs"] = _context.Tbljobs.Take(4).ToList();
            ViewData["Events"] = _context.Tblevents.Take(4).ToList();
            return View("Create", model);
        }

        [HttpPost]
        public IActionResult UpdateSection6(Tbllandingpage model)
        {
            //if (ModelState.IsValid)
            //{
                // Handle the update of Section 1 (Banner main heading and subheading)
                var existingLandingPage = _context.Tbllandingpages.FirstOrDefault();
                if (existingLandingPage != null)
                {
                    existingLandingPage.Sec6hd = model.Sec6hd;
                    existingLandingPage.Sec6dsc = model.Sec6dsc;
                    existingLandingPage.Sec6btntxt = model.Sec6btntxt;
                    _context.SaveChanges();
                }

            // Return the view with the updated model so the user stays on the same page
            //return View("Create", model);
            //}

            // If the model state is not valid, return the same view with validation messages
            ViewData["Jobs"] = _context.Tbljobs.Take(4).ToList();
            ViewData["Events"] = _context.Tblevents.Take(4).ToList();
            return View("Create", model);
        }

        [HttpPost]
        public IActionResult UpdateSection7(Tbllandingpage model)
        {
            //if (ModelState.IsValid)
            //{
                // Handle the update of Section 1 (Banner main heading and subheading)
                var existingLandingPage = _context.Tbllandingpages.FirstOrDefault();
                if (existingLandingPage != null)
                {
                    existingLandingPage.Sec7hd = model.Sec7hd;
                    existingLandingPage.Sec7btntxt= model.Sec7btntxt;
                    _context.SaveChanges();
                }

            // Return the view with the updated model so the user stays on the same page
            //    return View("Create", model);
            //}

            // If the model state is not valid, return the same view with validation messages
            ViewData["Jobs"] = _context.Tbljobs.Take(4).ToList();
            ViewData["Events"] = _context.Tblevents.Take(4).ToList();
            return View("Create", model);
        }

        [HttpPost]
        public IActionResult UpdateSection8(Tbllandingpage model)
        {
            //if (ModelState.IsValid)
            //{
                // Handle the update of Section 1 (Banner main heading and subheading)
                var existingLandingPage = _context.Tbllandingpages.FirstOrDefault();
                if (existingLandingPage != null)
                {
                    existingLandingPage.Sec8hd = model.Sec8hd;
                    existingLandingPage.Sec8dsc = model.Sec8dsc;
                    existingLandingPage.Sec8btntxt = model.Sec8btntxt;
                    _context.SaveChanges();
                }

            //    // Return the view with the updated model so the user stays on the same page
            //    return View("Create", model);
            //}

            // If the model state is not valid, return the same view with validation messages
            ViewData["Jobs"] = _context.Tbljobs.Take(4).ToList();
            ViewData["Events"] = _context.Tblevents.Take(4).ToList();
            return View("Create", model);
        }

    }
}
    