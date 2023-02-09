using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Stripe.Checkout;
using SunPerfume.DataAccess.Repository.IRepository;
using SunPerfume.Models;
using SunPerfume.Models.ViewModels;
using SunPerfume.Utility;
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
        public CartVM CartVM { get; set; }
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
                cart.Price = cart.Product.Price;
                CartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
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
                cart.Price = cart.Product.Price;
				CartVM.OrderHeader.OrderTotal += cart.Product.Price * cart.Count;
			}
			return View(CartVM);
		}
        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]

        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity?)User.Identity;
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);
            CartVM.ListCart = _unitOfWork.CartRepository.GetAll(u => u.ApplicationUserId == claim.Value,
                 includeProperties: "Product");

            CartVM.OrderHeader.OrderDate = DateTime.Now;
            CartVM.OrderHeader.ApplicationUserId = claim.Value;

            foreach (var cart in CartVM.ListCart)
            {
                cart.Price = cart.Product.Price;
                CartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }

            ApplicationUser applicationUser = _unitOfWork.ApplicationUserRepository.GetFirstOrDefault(u => u.Id == claim.Value);
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                CartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                CartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                CartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
                CartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            //Create Order Header
            _unitOfWork.OrderHeaderRepository.Add(CartVM.OrderHeader);
            _unitOfWork.Save();

            //Create Order Detail
            foreach (var cart in CartVM.ListCart)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = CartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count,
                };
                _unitOfWork.OrderDetailRepository.Add(orderDetail);
                //_unitOfWork.Save(); Redundant
            }
            _unitOfWork.Save();
            var uri = new Uri($"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}");
            // If individual user
            if (applicationUser.CompanyId.GetValueOrDefault() == 0)
            {
                //Stripe settings
                var domain = "https://" + uri.Host + ":" + uri.Port;
                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string>
                    {
                        "card",
                    },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + $"/customer/cart/OrderConfirmation?id={CartVM.OrderHeader.Id}",
                    CancelUrl = domain + "/customer/cart/index",
                };
                foreach (var item in CartVM.ListCart)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price), // 20.00 -> 2000
                            Currency = "vnd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                            },
                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionLineItem);
                }
                var service = new SessionService();
                Session session = service.Create(options);

                //Update do not use unit Of Work
                //CartVM.OrderHeader.SessionId = session.Id;
                //CartVM.OrderHeader.PaymentIntentId = session.PaymentIntentId;

                _unitOfWork.OrderHeaderRepository.UpdateStripePaymentID(CartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.Save();

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            else
            {
                return RedirectToAction("OrderConfirmation", "Cart", new { id = CartVM.OrderHeader.Id });
            }
        }
        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeaderRepository.GetFirstOrDefault(u => u.Id == id, includeProperties: "ApplicationUser");

            if (orderHeader == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                // Check the stripe status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeaderRepository.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeaderRepository.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    //_unitOfWork.Save();
                }
            }
            else
            {
                orderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
            }
            _emailSender.SendEmailAsync(orderHeader.ApplicationUser.Email, "New Order - Sun Perfume", "<p> New Order Created</p>");
            List<Cart> cartList = _unitOfWork.CartRepository.GetAll(
                u => u.ApplicationUserId == orderHeader.ApplicationUserId
                ).ToList();
            _unitOfWork.CartRepository.RemoveRange(cartList);
            HttpContext.Session.Clear();
            _unitOfWork.Save();

            return View(id);
        }
        public IActionResult Plus(int cartId)
        {
            var cart = _unitOfWork.CartRepository.GetFirstOrDefault(u => u.Id == cartId);
            _unitOfWork.CartRepository.IncrementCount(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cartId)
        {
            var cart = _unitOfWork.CartRepository.GetFirstOrDefault(u => u.Id == cartId);
            if (cart.Count <= 1)
            {
                _unitOfWork.CartRepository.Remove(cart);
                HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.CartRepository.GetAll(u => u.ApplicationUserId ==
                        cart.ApplicationUserId).ToList().Count - 1);
            }
            else
            {
                _unitOfWork.CartRepository.DecrementCount(cart, 1);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Remove(int cartId)
        {
            var cart = _unitOfWork.CartRepository.GetFirstOrDefault(u => u.Id == cartId);
            _unitOfWork.CartRepository.Remove(cart);
            HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.CartRepository.GetAll(u => u.ApplicationUserId ==
                        cart.ApplicationUserId).ToList().Count - 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
