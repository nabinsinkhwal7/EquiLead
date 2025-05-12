using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EquidCMS.Models;
using EquidCMS.Controllers.ViewModels;
using Npgsql;

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

        //public IActionResult renderRightsMenuCMS(int RoleID)
        //{
        //    var MenuList = _context.RoleMenus
        //        .Where(c => c.Menu.MenuType == 1).
        //        Include(c => c.Menu)
        //        .OrderBy(c => c.Menu.MenuSequence).ToList();
        //    return PartialView("_PVUserRightMenuAdmin", MenuList);
        //}

        public IActionResult renderRightsMenuCMS(int RoleID)
        {
            List<RoleMenu> menuList;

            if (RoleID > 0)
            {
                var hasRights = _context.RoleMenus.Any(rm => rm.RoleId == RoleID && rm.Menu.MenuType == 1);

                if (hasRights)
                {
                    menuList = _context.RoleMenus
                        .Where(rm => rm.RoleId == RoleID && rm.Menu.MenuType == 1)
                        .Include(rm => rm.Menu)
                        .OrderBy(rm => rm.Menu.MenuSequence)
                        .ToList();
                }
                else
                {
                    menuList = _context.MstMenus
                        .Where(m => m.MenuType == 1)
                        .OrderBy(m => m.MenuSequence)
                        .Select(m => new RoleMenu
                        {
                            RoleId = RoleID,
                            MenuId = m.MenuId,
                            Menu = m,
                            Display = false,
                            AddNew = false,
                            Edit = false,
                            IsDeleted = false
                        }).ToList();
                }
            }
            else
            {
                menuList = _context.MstMenus
                    .Where(m => m.MenuType == 1)
                    .OrderBy(m => m.MenuSequence)
                    .Select(m => new RoleMenu
                    {
                        MenuId = m.MenuId,
                        Menu = m,
                        Display = false,
                        AddNew = false,
                        Edit = false,
                        IsDeleted = false
                    }).ToList();
            }

            return PartialView("_PVUserRightMenuAdmin", menuList);
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

        //public async Task<IActionResult> SaveUserRightAll([FromBody] List<UserRightsData> data)
        //{
        //    try
        //    {
        //        foreach (var item in data)
        //        {
        //            var roleMenu = _context.RoleMenus
        //                .FirstOrDefault(c => c.RoleId == item.roleid && c.MenuId == item.Menuid);

        //            if (roleMenu == null)
        //            {
        //                var newRoleMenu = new RoleMenu
        //                {
        //                    RoleId = item.roleid,
        //                    MenuId = item.Menuid,
        //                    Display = item.DSN,
        //                    AddNew = item.ADN,
        //                    Edit = item.EDN,
        //                    IsDeleted = Convert.ToBoolean(item.DLN)
        //                };
        //                _context.RoleMenus.Add(newRoleMenu);
        //            }
        //            else
        //            {
        //                roleMenu.Display = item.DSN;
        //                roleMenu.AddNew = item.ADN;
        //                roleMenu.Edit = item.EDN;
        //                roleMenu.IsDeleted = Convert.ToBoolean(item.DLN);
        //                _context.RoleMenus.Update(roleMenu);
        //            }

        //            if (item.MenuParentid != 0)
        //            {
        //                var parentRoleMenu = _context.RoleMenus
        //                    .FirstOrDefault(c => c.RoleId == item.roleid && c.MenuId == item.MenuParentid);

        //                if (parentRoleMenu == null)
        //                {
        //                    var newParentRoleMenu = new RoleMenu
        //                    {
        //                        RoleId = item.roleid,
        //                        MenuId = item.MenuParentid,
        //                        Display = item.DSN,
        //                        AddNew = item.ADN,
        //                        Edit = item.EDN,
        //                        IsDeleted = Convert.ToBoolean(item.DLN)
        //                    };
        //                    _context.RoleMenus.Add(newParentRoleMenu);
        //                }
        //                else
        //                {
        //                    parentRoleMenu.Display = item.DSN;
        //                    parentRoleMenu.AddNew = item.ADN;
        //                    parentRoleMenu.Edit = item.EDN;
        //                    parentRoleMenu.IsDeleted = Convert.ToBoolean(item.DLN);
        //                    _context.RoleMenus.Update(parentRoleMenu);
        //                }
        //            }
        //        }

        //        await _context.SaveChangesAsync();

        //        return Json(new { success = true });
        //    }
        //    catch (PostgresException ex) when (ex.SqlState == "23505")
        //    {
        //        return Json(new { success = false, message = "Duplicate key violation." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}


        public async Task<IActionResult> SaveUserRightAll([FromBody] List<UserRightsData> data)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        foreach (var item in data)
                        {
                            var roleMenu = await _context.RoleMenus
                                .FirstOrDefaultAsync(c => c.RoleId == item.roleid && c.MenuId == item.Menuid);

                            if (roleMenu == null)
                            {
                                var newRoleMenu = new RoleMenu
                                {
                                    RoleId = item.roleid,
                                    MenuId = item.Menuid,
                                    Display = item.DSN,
                                    AddNew = item.ADN,
                                    Edit = item.EDN,
                                    IsDeleted = Convert.ToBoolean(item.DLN)
                                };
                                _context.RoleMenus.Add(newRoleMenu);
                            }
                            else
                            {
                                roleMenu.Display = item.DSN;
                                roleMenu.AddNew = item.ADN;
                                roleMenu.Edit = item.EDN;
                                roleMenu.IsDeleted = Convert.ToBoolean(item.DLN);
                                _context.RoleMenus.Update(roleMenu);
                            }

                            if (item.MenuParentid != 0)
                            {
                                var parentRoleMenu = await _context.RoleMenus
                                    .FirstOrDefaultAsync(c => c.RoleId == item.roleid && c.MenuId == item.MenuParentid);

                                if (parentRoleMenu == null)
                                {
                                    var newParentRoleMenu = new RoleMenu
                                    {
                                        RoleId = item.roleid,
                                        MenuId = item.MenuParentid,
                                        Display = item.DSN,
                                        AddNew = item.ADN,
                                        Edit = item.EDN,
                                        IsDeleted = Convert.ToBoolean(item.DLN)
                                    };
                                    _context.RoleMenus.Add(newParentRoleMenu);
                                }
                                else
                                {
                                    parentRoleMenu.Display = item.DSN;
                                    parentRoleMenu.AddNew = item.ADN;
                                    parentRoleMenu.Edit = item.EDN;
                                    parentRoleMenu.IsDeleted = Convert.ToBoolean(item.DLN);
                                    _context.RoleMenus.Update(parentRoleMenu);
                                }
                            }
                        }

                        // Commit the transaction
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return Json(new { success = true });
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if something goes wrong
                        await transaction.RollbackAsync();
                        return Json(new { success = false, message = ex.Message });
                    }
                }
            }
            catch (PostgresException ex) when (ex.SqlState == "23505")
            {
                return Json(new { success = false, message = "Duplicate key violation." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }






        //public IActionResult SaveUserRightAll([FromBody] List<UserRightsData> data)
        //{
        //    try
        //    {


        //        foreach (var item in data)
        //        {
        //            RoleMenu RoleMenuN = (from c in _context.RoleMenus
        //                                  where c.RoleMenuId == item.RoleMenuIdN
        //                                  select c).FirstOrDefault();
        //            RoleMenuN.Display = item.DSN;
        //            RoleMenuN.AddNew = item.ADN;
        //            RoleMenuN.Edit = item.EDN;
        //            RoleMenuN.IsDeleted = Convert.ToBoolean(item.DLN);
        //            _context.Update(RoleMenuN);
        //            _context.SaveChanges();


        //            if (item.MenuParentid != 0)
        //            {
        //                RoleMenu RoleMenuNParent = (from c in _context.RoleMenus
        //                                            where c.MenuId == item.MenuParentid && c.RoleId == item.roleid
        //                                            select c).FirstOrDefault();
        //                RoleMenuNParent.Display = item.DSN;
        //                RoleMenuNParent.AddNew = item.ADN;
        //                RoleMenuNParent.Edit = item.EDN;
        //                RoleMenuNParent.IsDeleted = Convert.ToBoolean(item.DLN);
        //                _context.Update(RoleMenuNParent);
        //                _context.SaveChanges();
        //            }
        //        }
        //        return Json(new { success = true });
        //    }
        //    catch
        //    {
        //        return Json(new { success = false });
        //    }
        //    //if (data.Flag == "Admin")
        //    //{
        //    //    var MenuList = _context.RoleMenus.Where(c => c.RoleId == data.RoleMenuIdN && (c.Menu.MenuType == 1)).Include(c => c.Menu).OrderBy(c => c.Menu.MenuSequence).ToList();
        //    //    return PartialView("_PVUserRightMenuAdmin", MenuList);
        //    //}
        //    //else
        //    //{
        //    //    var MenuList = _context.RoleMenus.Where(c => c.RoleId == data.RoleMenuIdN && (c.Menu.MenuType == 1)).Include(c => c.Menu).OrderBy(c => c.Menu.MenuSequence).ToList();
        //    //    return PartialView("_PVUserRightMenuAdmin", MenuList);
        //    //    //var MenuList = _context.RoleMenus.Where(c => c.RoleID == RoleMenuN.RoleID).Include(c => c.Menu).ToList();
        //    //    //return PartialView("_PVUserRightMenu", MenuList);
        //    //}
        //}




    }
}
