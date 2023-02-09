using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SunPerfume.DataAccess.Repository.IRepository;
using SunPerfume.Models.ViewModels;
using System.Security.Claims;

namespace SunPerfumeWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        [BindProperty]
        CartVM CartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            CartVM = new CartVM()
            {
                ListCart = _unitOfWork.CartRepository.GetAll(u => u.ApplicationUserId == claim.Value,
                 includeProperties: "Product"),
                OrderHeader = new()
            };
            foreach (var cart in CartVM.ListCart)
            {
                CartVM.OrderHeader.OrderTotal += cart.Product.Price * cart.Count;
            }
            return View(CartVM);
        }
		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity?)User.Identity;
			var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

			CartVM = new CartVM()
			{
				ListCart = _unitOfWork.CartRepository.GetAll(u => u.ApplicationUserId == claim.Value,
				 includeProperties: "Product"),
				OrderHeader = new()
			};
			CartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUserRepository.GetFirstOrDefault(
				u => u.Id == claim.Value
				);
			CartVM.OrderHeader.Name = CartVM.OrderHeader.ApplicationUser.Name;
			CartVM.OrderHeader.PhoneNumber = CartVM.OrderHeader.ApplicationUser.PhoneNumber;
			CartVM.OrderHeader.StreetAddress = CartVM.OrderHeader.ApplicationUser.StreetAddress;
			CartVM.OrderHeader.City = CartVM.OrderHeader.ApplicationUser.City;
			CartVM.OrderHeader.State = CartVM.OrderHeader.ApplicationUser.State;
			CartVM.OrderHeader.PostalCode = CartVM.OrderHeader.ApplicationUser.PostalCode;

			foreach (var cart in CartVM.ListCart)
			{
				CartVM.OrderHeader.OrderTotal += cart.Product.Price * cart.Count;
			}
			return View(CartVM);
		}
	}
}
