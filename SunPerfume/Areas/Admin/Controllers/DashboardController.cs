using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunPerfume.Utility;

namespace SunPerfumeWeb.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        [Area("Admin")]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]

        public IActionResult Index()
        {
            return View();
        }
    }
}
