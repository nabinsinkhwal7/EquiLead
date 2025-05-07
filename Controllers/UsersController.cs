using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EquidCMS.Models;

namespace RCH_Dynamic_Counselling.Controllers
{
    public class UsersController : Controller
    {
        private readonly EquiDbContext _context;

        public UsersController(EquiDbContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(await _context.MstUsers.Where(a => a.IsDeleted == 0 && a.IsActive == 1).Include(a => a.Role).OrderByDescending(a=>a.UserId).ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mstUser = await _context.MstUsers
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (mstUser == null)
            {
                return NotFound();
            }

            return View(mstUser);
        }

        [HttpGet]
        public async Task<JsonResult> UsernameExists(string username)
        {
            var userExists = await _context.MstUsers.AnyAsync(u => u.UserName == username);
            return Json(new { exists = userExists });
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            
            ViewData["RoleId"] = new SelectList(_context.MstRoles.Where(m => m.IsDeleted == 0).OrderBy(p => p.RoleId), "RoleId", "Role");

            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,UserName,FullName,Mobile,Password,RoleId,EmailId,AuthToken,StateId,DistrictId,BlockId,VillageId,Address,Pincode,IsActive,IsDeleted,PwdLinkExpTime,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn,NoofLogin,LastLogin,Vcode")] MstUser mstUser)
        {
            try
            {
                if (IsUserNameExist(mstUser.UserName))
                {
                    SetViewBagState(); 
                    ModelState.AddModelError("", "Username already exists.");
                    //SetViewDataRole(mstUser.RoleId);
                    //return RedirectToAction(nameof(Create));
                    ViewData["RoleId"] = new SelectList(_context.MstRoles.Where(m => m.IsDeleted == 0).OrderBy(p => p.RoleId), "RoleId", "Role");
                    return View(mstUser);
                    //return RedirectToAction(nameof(Create));
                }
                mstUser.IsActive = 1;
                mstUser.CreatedBy = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
                mstUser.CreatedOn = DateTime.Now;
                mstUser.Password = CreateUserNameHash(mstUser.Password); 
                _context.Add(mstUser);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save changes. Please try again."); 
                                                                                           
                Console.WriteLine($"Error in Create action: {ex.Message}");
            }
            return View(mstUser);
            //return RedirectToAction(nameof(Create));
        }



        private bool IsUserNameExist(string userName)
        {
            return _context.MstUsers.Any(p => p.UserName == userName);
        }
        private bool IsValidMobile(string mobile)
        {
            return !string.IsNullOrEmpty(mobile) && mobile.Length >= 10;
        }

        private void SetViewBagState()
        {
            
        }

        private void SetViewDataRole(int? roleId)
        {
            ViewData["RoleId"] = new SelectList(_context.MstRoles.Where(m => m.IsDeleted == 0).OrderBy(p => p.RoleId), "RoleId", "Role", roleId);
        }
        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mstUser = await _context.MstUsers.FindAsync(id);
            if (mstUser == null)
            {
                return NotFound();
            }
            
            ViewData["RoleId"] = new SelectList(_context.MstRoles.Where(m => m.IsDeleted == 0).OrderBy(p => p.RoleId), "RoleId", "Role", mstUser.RoleId);
            return View(mstUser);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,UserName,FullName,Mobile,Password,RoleId,EmailId,AuthToken,StateId,DistrictId,BlockId,VillageId,Address,Pincode,IsActive,IsDeleted,PwdLinkExpTime,CreatedBy,CreatedOn,UpdatedBy,UpdatedOn,NoofLogin,LastLogin,Vcode")] MstUser mstUser)
        {
            if (id != mstUser.UserId)
            {
                return NotFound();
            }

            if (!IsValidMobile(mstUser.Mobile))
            {
                SetViewBagState();
                ModelState.AddModelError("", "Invalid Mobile Number!.");
                SetViewDataRole(mstUser.RoleId);
                return View(mstUser);
            }

            var existingUser = await _context.MstUsers.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

            try
            {
                UpdateExistingUser(existingUser, mstUser);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                ModelState.AddModelError("", "Unable to save changes due to concurrency issues. Try again, and if the problem persists, see your system administrator.");
            }

            SetViewDataRole(mstUser.RoleId);
            return RedirectToAction(nameof(Index));
        }
       
        private void UpdateExistingUser(MstUser existingUser, MstUser mstUser)
        {
            existingUser.FullName = mstUser.FullName;
            
            existingUser.Mobile = mstUser.Mobile;
            existingUser.EmailId = mstUser.EmailId;
            existingUser.Address = mstUser.Address;
            existingUser.UpdatedBy = Convert.ToInt32(HttpContext.Session.GetString("UserID"));
            existingUser.UpdatedOn = DateTime.Now;

            _context.Update(existingUser);
        }
       
        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mstUser = await _context.MstUsers
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (mstUser == null)
            {
                return NotFound();
            }

            return View(mstUser);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mstUser = await _context.MstUsers.FindAsync(id);
            if (mstUser != null)
            {
                mstUser.IsDeleted = 1;
            }
          
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MstUserExists(int id)
        {
            return _context.MstUsers.Any(e => e.UserId == id);
        }
        
        public static string CreateUserNameHash(string UserName)
        {
            int Password_saltArraySize = 16;
            string saltAndPwd = String.Concat(UserName, Password_saltArraySize.ToString());
            HashAlgorithm hashAlgorithm = SHA512.Create();
            List<byte> pass = new List<byte>(Encoding.Unicode.GetBytes(saltAndPwd));
            string hashedPwd = Convert.ToBase64String(hashAlgorithm.ComputeHash(pass.ToArray()));
            hashedPwd = String.Concat(hashedPwd, Password_saltArraySize.ToString());
            return hashedPwd;
        }
    }
}
