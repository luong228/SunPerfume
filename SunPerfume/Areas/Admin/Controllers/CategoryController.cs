using SunPerfume.DataAccess.Repository.IRepository;
using SunPerfume.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;

namespace SunPerfumeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public CategoryController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            var listCategory = _unitOfWork.CategoryRepository.GetAll();
            return View(listCategory);
        }
        public IActionResult Upsert(string? id)
        {
            Category category = new();
            if (id == null)
            {
                return View(category);
            }
            else
            {
                category = _unitOfWork.CategoryRepository.GetFirstOrDefault(u => u.CategoryId == id);
                return View(category);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images");
                    var extension = Path.GetExtension(file.FileName);

                    // Delete old image if exist
                    if (obj.ImageUrl != null)
                    {
                        var test = obj.ImageUrl.TrimStart('\\');
                        var oldImagePath = wwwRootPath + "\\images\\";
                        oldImagePath = Path.Combine(oldImagePath, obj.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        //Copy content from file to fileStream that already created
                        file.CopyTo(fileStreams);
                    }
                    obj.ImageUrl = fileName + extension;
                }
                if (obj.CategoryId == "new")
                {
                    _unitOfWork.CategoryRepository.Add(obj);
                    TempData["success"] = "Category created successfully";

                }
                else
                {
                    _unitOfWork.CategoryRepository.Update(obj);
                    TempData["success"] = "Category updated successfully";
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
            var obj = _unitOfWork.CategoryRepository.GetFirstOrDefault(u => u.CategoryId == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _unitOfWork.CategoryRepository.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Category deleted successfully" });
        }
        #endregion
    }
}
