using Microsoft.AspNetCore.Mvc;
using SunPerfume.DataAccess.Repository.IRepository;
using SunPerfume.Models;

namespace SunPerfume.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryViewComponent(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            IEnumerable<Category> categoryList = _unitOfWork.CategoryRepository.GetAll();
            return View(categoryList);
        }
    }
}
