using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EquidCMS.Models;
using EquidCMS.Controllers.ViewModels;

namespace EquidCMS.Controllers
{
    public class UserRightController : Controller
    {
        private readonly EquiDbContext _context;

        public UserRightController(EquiDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AssignRightsToRole()
        {
            ViewData["RoleID"] = new SelectList(_context.MstRoles.Where(p => p.IsDeleted == 0).OrderBy(p => p.Seq), "RoleId", "Role");
            return View();
        }

        public IActionResult renderRightsMenuCMS(int RoleID)
        {
            var MenuList = _context.RoleMenus
                .Where(c => c.RoleId == RoleID 
                && (c.Menu.MenuType == 1)).
                Include(c => c.Menu)
                .OrderBy(c => c.Menu.MenuSequence).ToList();
            return PartialView("_PVUserRightMenuAdmin", MenuList);
        }

        public IActionResult SaveUserRights(bool DSN, bool ADN, bool EDN, int DLN, int RoleMenuIdN, int MenuParentid, int roleid, string Flag)
        {
            RoleMenu RoleMenuN = (from c in _context.RoleMenus
                                  where c.RoleMenuId == RoleMenuIdN
                                  select c).FirstOrDefault();
            RoleMenuN.Display = DSN;
            RoleMenuN.AddNew = ADN;
            RoleMenuN.Edit = EDN;
            RoleMenuN.IsDeleted =Convert.ToBoolean(DLN);
            _context.Update(RoleMenuN);
            _context.SaveChanges();


            if (MenuParentid == 0) { }
            else
            {
                RoleMenu RoleMenuNParent = (from c in _context.RoleMenus
                                            where c.MenuId == MenuParentid && c.RoleId == roleid
                                            select c).FirstOrDefault();
                RoleMenuNParent.Display = DSN;
                RoleMenuNParent.AddNew = ADN;
                RoleMenuNParent.Edit = EDN;
                RoleMenuNParent.IsDeleted =Convert.ToBoolean(DLN);
                _context.Update(RoleMenuNParent);
                _context.SaveChanges();
            }


            if (Flag == "Admin")
            {
                var MenuList = _context.RoleMenus.Where(c => c.RoleId == RoleMenuN.RoleId && (c.Menu.MenuType == 1)).Include(c => c.Menu).OrderBy(c => c.Menu.MenuSequence).ToList();
                return PartialView("_PVUserRightMenuAdmin", MenuList);
            }
            else
            {
                var MenuList = _context.RoleMenus.Where(c => c.RoleId == RoleMenuN.RoleId && (c.Menu.MenuType == 1)).Include(c => c.Menu).OrderBy(c => c.Menu.MenuSequence).ToList();
                return PartialView("_PVUserRightMenuAdmin", MenuList);
                //var MenuList = _context.RoleMenus.Where(c => c.RoleID == RoleMenuN.RoleID).Include(c => c.Menu).ToList();
                //return PartialView("_PVUserRightMenu", MenuList);
            }

        }
        public IActionResult SaveUserRightAll([FromBody] List<UserRightsData> data)
        {
            try
            {


                foreach (var item in data)
                {
                    RoleMenu RoleMenuN = (from c in _context.RoleMenus
                                          where c.RoleMenuId == item.RoleMenuIdN
                                          select c).FirstOrDefault();
                    RoleMenuN.Display = item.DSN;
                    RoleMenuN.AddNew = item.ADN;
                    RoleMenuN.Edit = item.EDN;
                    RoleMenuN.IsDeleted = Convert.ToBoolean(item.DLN);
                    _context.Update(RoleMenuN);
                    _context.SaveChanges();


                    if (item.MenuParentid != 0)
                    {
                        RoleMenu RoleMenuNParent = (from c in _context.RoleMenus
                                                    where c.MenuId == item.MenuParentid && c.RoleId == item.roleid
                                                    select c).FirstOrDefault();
                        RoleMenuNParent.Display = item.DSN;
                        RoleMenuNParent.AddNew = item.ADN;
                        RoleMenuNParent.Edit = item.EDN;
                        RoleMenuNParent.IsDeleted = Convert.ToBoolean(item.DLN);
                        _context.Update(RoleMenuNParent);
                        _context.SaveChanges();
                    }
                }
                return Json(new { success = true });
            }
            catch
            {
                return Json(new { success = false });
            }
            //if (data.Flag == "Admin")
            //{
            //    var MenuList = _context.RoleMenus.Where(c => c.RoleId == data.RoleMenuIdN && (c.Menu.MenuType == 1)).Include(c => c.Menu).OrderBy(c => c.Menu.MenuSequence).ToList();
            //    return PartialView("_PVUserRightMenuAdmin", MenuList);
            //}
            //else
            //{
            //    var MenuList = _context.RoleMenus.Where(c => c.RoleId == data.RoleMenuIdN && (c.Menu.MenuType == 1)).Include(c => c.Menu).OrderBy(c => c.Menu.MenuSequence).ToList();
            //    return PartialView("_PVUserRightMenuAdmin", MenuList);
            //    //var MenuList = _context.RoleMenus.Where(c => c.RoleID == RoleMenuN.RoleID).Include(c => c.Menu).ToList();
            //    //return PartialView("_PVUserRightMenu", MenuList);
            //}
        }




    }
}
