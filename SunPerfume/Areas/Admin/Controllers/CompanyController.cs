using SunPerfume.DataAccess.Repository.IRepository;
using SunPerfume.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using SunPerfume.Utility;
using System.Data;

namespace SunPerfumeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public CompanyController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            var listCompany = _unitOfWork.CompanyRepository.GetAll();
            return View(listCompany);
        }
        public IActionResult Upsert(int? id)
        {
            Company company = new();
            if (id == 0 || id == null)
            {
                return View(company);
            }
            else
            {
                company = _unitOfWork.CompanyRepository.GetFirstOrDefault(u => u.Id == id);
                return View(company);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.Id == 0)
                {
                    _unitOfWork.CompanyRepository.Add(obj);
                    TempData["success"] = "Company created successfully";

                }
                else
                {
                    _unitOfWork.CompanyRepository.Update(obj);
                    TempData["success"] = "Company updated successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");

            }
            return View(obj);
        }
        #region API CALL
        //POST
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _unitOfWork.CompanyRepository.GetFirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.CompanyRepository.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Company deleted successfully" });
        }
        #endregion
    }
}
