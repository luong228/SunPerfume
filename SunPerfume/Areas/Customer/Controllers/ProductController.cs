using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SunPerfume.DataAccess.Repository.IRepository;
using SunPerfume.Models;
using SunPerfume.Models.ViewModels;
using SunPerfume.Utility;
using System.Security.Claims;

namespace SunPerfumeWeb.Areas.Customer.Controllers
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
        public IActionResult Brand(string brandId)
        {
            if (brandId == null)
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
                    Brand = _unitOfWork.BrandRepository.GetFirstOrDefault(u => u.BrandId == brandId),
                    ProductList = _unitOfWork.ProductRepository.GetAll(u => u.BrandId == brandId),

                };
            }
            return View(ProductVM);
        }
        public IActionResult Details (string productId)
        {
            Cart cart = new()
            {
                Count = 1,
                ProductId = productId,
                Product = _unitOfWork.ProductRepository.GetFirstOrDefault(u => u.ProductId == productId, includeProperties: "Brand,Category"),
                
            }; 
            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(Cart cart)
        {
            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            cart.ApplicationUserId = claim.Value;

            Cart cartFromDb = _unitOfWork.CartRepository.GetFirstOrDefault(
                u => u.ApplicationUserId == claim.Value && u.ProductId == cart.ProductId);

            if (cartFromDb == null)
            {
                _unitOfWork.CartRepository.Add(cart);
                _unitOfWork.Save();
            }
            else
            {
                _unitOfWork.CartRepository.IncrementCount(cartFromDb, cart.Count);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.CartRepository.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
            }
            return RedirectToAction("Category", "Product");
        }
    }
}
