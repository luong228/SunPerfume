using Microsoft.AspNetCore.Mvc;
using SunPerfume.DataAccess.Repository.IRepository;

namespace SunPerfume.Area.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork= unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
