using Microsoft.AspNetCore.Mvc;
using SunPerfume.DataAccess.Repository.IRepository;
using SunPerfume.Models;
using SunPerfume.Models.ViewModels;
using System.Web;
using System;

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

            var uri = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");

            //var url = uri.AbsoluteUri;
            var query = HttpUtility.ParseQueryString(uri.Query);

            var cateId = query.Get("cateId");
            var brandId = query.Get("brandId");

            ViewData["cateId"] = cateId;
            ViewData["brandId"] = brandId;

            //ViewBag.cateId = cateId;
            //ViewBag.brandId = brandId;

            return View(BrandCategoryVM);
        }
    }
}
