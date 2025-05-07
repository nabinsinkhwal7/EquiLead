using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;

namespace EquidCMS.Controllers
{
    public class CommonFN : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

     
    }
}
