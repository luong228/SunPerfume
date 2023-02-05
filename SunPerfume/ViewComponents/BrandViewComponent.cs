using Microsoft.AspNetCore.Mvc;
using SunPerfume.DataAccess.Repository.IRepository;
using SunPerfume.Models;

namespace SunPerfume.ViewComponents
{
    public class BrandViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public BrandViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<Brand> brandList = _unitOfWork.BrandRepository.GetAll();
            return View(brandList);
        }
    }
}
