using Microsoft.AspNetCore.Mvc;
using SunPerfume.DataAccess.Repository.IRepository;
using SunPerfume.Models;
using SunPerfume.Models.ViewModels;
using System.Diagnostics;

namespace SunPerfume.Area.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeVM HomeVM { get; set; }
        public HomeController(IUnitOfWork unitOfWork, ILogger<HomeController> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            HomeVM = new HomeVM()
            {
                ProductList = _unitOfWork.ProductRepository.GetAll(),
                CategoryList = _unitOfWork.CategoryRepository.GetAll(),
            };
            return View(HomeVM);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}