using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunPerfume.DataAccess.Repository.IRepository;
using SunPerfume.Models;
using SunPerfume.Utility;

namespace SunPerfumeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region API CALL
        [HttpGet]
        public IActionResult GetAll()
        {

            IEnumerable<ApplicationUser> listUser = _unitOfWork.ApplicationUserRepository.GetAll();
            return Json(new {data = listUser});
        }
        #endregion
    }
}
