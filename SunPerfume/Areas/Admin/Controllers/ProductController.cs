using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Hosting;
using SunPerfume.DataAccess.Repository.IRepository;
using SunPerfume.Models;
using SunPerfume.Models.ViewModels;

namespace SunPerfumeWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        public ProductVM ProductVM { get; set; }
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Product> listProduct = _unitOfWork.ProductRepository.GetAll();
            return View(listProduct);
        }
        public IActionResult Upsert(string? id)
        {
            ProductVM ProductVM = new()
            {
                Product = new(),
                CategorySelectList = _unitOfWork.CategoryRepository.GetAll().Select(
                    u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.CategoryId
                    }),
                BrandSelectList = _unitOfWork.BrandRepository.GetAll().Select(
                    u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.BrandId
                    }),
            };
            if (id == null)
            {
                return View(ProductVM);
            }
            else
            {
                ProductVM.Product = _unitOfWork.ProductRepository.GetFirstOrDefault(u => u.ProductId == id);
                return View(ProductVM);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);

                    // Delete old image if exist
                    if (obj.Product.ImageUrl != null)
                    {
                        var test = obj.Product.ImageUrl.TrimStart('\\');
                        var oldImagePath = wwwRootPath + "\\images\\";
                        oldImagePath = Path.Combine(oldImagePath, obj.Product.ImageUrl.TrimStart('\\'));
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
                    obj.Product.ImageUrl = @"\products\" + fileName + extension;
                }
                if (obj.Product.ProductId == "new")
                {
                    _unitOfWork.ProductRepository.Add(obj.Product);
                    TempData["success"] = "Product created successfully";

                }
                else
                {
                    _unitOfWork.ProductRepository.Update(obj.Product);
                    TempData["success"] = "Product updated successfully";
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
            var obj = _unitOfWork.ProductRepository.GetFirstOrDefault(u => u.ProductId == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            string wwwRootPath = _hostEnvironment.WebRootPath;
            var oldImagePath = wwwRootPath + "\\images\\";
            oldImagePath = Path.Combine(oldImagePath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.ProductRepository.Remove(obj);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Product deleted successfully" });
        }
        #endregion
    }
}
