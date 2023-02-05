using Microsoft.AspNetCore.Mvc;
using SunPerfume.DataAccess.Repository.IRepository;
using SunPerfume.Models.ViewModels;

namespace SunPerfume.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ProductVM ProductVM { get; set; }
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
 
        public IActionResult Category(string? cateId)
        {
            if (cateId == null)
            {
                ProductVM = new ProductVM()
                {
                    ProductList = _unitOfWork.ProductRepository.GetAll(),
                };
            }
            else
            {
                ProductVM = new ProductVM()
                {
                    Category = _unitOfWork.CategoryRepository.GetFirstOrDefault(u => u.CategoryId == cateId),
                    ProductList = _unitOfWork.ProductRepository.GetAll(u => u.CategoryId == cateId),

                };
            }
            return View(ProductVM);
        }
    }
}
