using Microsoft.AspNetCore.Mvc;
using SunPerfume.DataAccess.Repository.IRepository;
using SunPerfume.Models;
using SunPerfume.Models.ViewModels;

namespace SunPerfume.ViewComponents
{
    public class FilterSidebarViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public BrandCategoryVM BrandCategoryVM { get; set; }
        public FilterSidebarViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            BrandCategoryVM = new BrandCategoryVM()
            {
                CategoryList = _unitOfWork.CategoryRepository.GetAll(),
                BrandList = _unitOfWork.BrandRepository.GetAll()
            };
            
            return View(BrandCategoryVM);
        }
    }
}
