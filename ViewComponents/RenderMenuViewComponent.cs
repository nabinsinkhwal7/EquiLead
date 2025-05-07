using EquidCMS.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RCH_Dynamic_Counselling.ViewComponents
{
    public class RenderMenuViewComponent : ViewComponent
    {
        private readonly EquiDbContext _context;

        public RenderMenuViewComponent(EquiDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Retrieve the RoleId from the session, and check if it's valid
            var roleIdStr = HttpContext.Session.GetString("RoleID");
            if (string.IsNullOrEmpty(roleIdStr) || !int.TryParse(roleIdStr, out var roleId))
            {
                // Handle invalid or missing roleId if necessary
                return View(new List<MstMenu>());
            }

            // Query the menu items based on the role ID
            var menuList = await (from c in _context.MstMenus
                                  join cn in _context.RoleMenus on c.MenuId equals cn.MenuId
                                  where c.MenuType == 1
                                        && c.IsDeleted == false
                                        && cn.RoleId == roleId
                                        && cn.Display == true
                                  orderby c.MenuSequence
                                  select c).ToListAsync();

            // Return the result to the view
            return View(menuList);
        }



    }
}
