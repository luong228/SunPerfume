using Microsoft.AspNetCore.Mvc;
using SunPerfume.DataAccess.Repository.IRepository;
using SunPerfume.Models;

namespace SunPerfume.Area.Admin.Controllers
{
    [Area("Admin")]
    public class BrandController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public BrandController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Brand> listBrand = _unitOfWork.BrandRepository.GetAll();
            return View(listBrand);
        }
        public IActionResult Upsert(string? id)
        {
            Brand brand = new();
            if (id == null)
            { 
                return View(brand);
            }
            else
            {
                brand = _unitOfWork.BrandRepository.GetFirstOrDefault(u => u.BrandId == id);
                return View(brand);
            }
           
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Brand obj)
        {
            if (ModelState.IsValid)
            {
                if (obj.BrandId == "new")
                {
                    obj.BrandId = "";
                    _unitOfWork.BrandRepository.Add(obj);
                    TempData["success"] = "Brand created successfully";

                }
                else 
                {
                    _unitOfWork.BrandRepository.Update(obj);
                    TempData["success"] = "Brand updated successfully";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");

            }
            return View(obj);
        }
        #region API CALL
        //POST
        [HttpDelete]
        public IActionResult Delete(string? id)
        {
            var obj = _unitOfWork.BrandRepository.GetFirstOrDefault(u => u.BrandId == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.BrandRepository.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Brand deleted successfully" });
        }
        #endregion
    }
}
