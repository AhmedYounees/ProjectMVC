using Entities.Models;
using Entities.Reposatories;
using Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using Stripe.Checkout;
using System.Security.Claims;
using Utilities;

namespace ProjectMVC.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public int TotalCarts { get; set; }


        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            ShoppingCartVM = new ShoppingCartVM();
        }
        public IActionResult Index()
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);
             
            ShoppingCartVM = new ShoppingCartVM()
            {
                CartList = _unitOfWork.ShoppingCart.GetAll(u => u.applicationUserId == claim.Value, icludeWord: "Product")
            };

            foreach (var item in ShoppingCartVM.CartList)
            {
                ShoppingCartVM.TotalCarts += (item.Count * item.Product.Price); 

			}
            return View(ShoppingCartVM);
        }
        [HttpGet]
        public IActionResult Summary()
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                CartList = _unitOfWork.ShoppingCart.GetAll(u => u.applicationUserId == claim.Value, icludeWord: "Product"),
                OrderHeader= new()
            };

            ShoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetByID(x=>x.Id == claim.Value);
            
            ShoppingCartVM.OrderHeader.Name=ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.Address = ShoppingCartVM.OrderHeader.ApplicationUser.Address;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.Phone = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

            foreach (var item in ShoppingCartVM.CartList)
            {
                ShoppingCartVM.OrderHeader.TotalPrice += (item.Count * item.Product.Price);

            }
            return View("summary",ShoppingCartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(ShoppingCartVM shoppingcartVM)
        {

            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingcartVM.CartList = _unitOfWork.ShoppingCart.GetAll(u => u.applicationUserId == claim.Value, icludeWord: "Product");

            shoppingcartVM.OrderHeader.OrderStatus = SD.Pending;
            shoppingcartVM.OrderHeader.PaymentSatuts = SD.Pending;
            shoppingcartVM.OrderHeader.OrderDate = DateTime.Now;
            shoppingcartVM.OrderHeader.ApplicationUserId = claim.Value;

            if (shoppingcartVM != null && shoppingcartVM.CartList != null)
            {
                foreach (var item in shoppingcartVM.CartList)
                {
                    shoppingcartVM.OrderHeader.TotalPrice += (item.Count * item.Product.Price);

                }
               
                _unitOfWork.OrderHeader.add(shoppingcartVM.OrderHeader);
                _unitOfWork.complete();
            }
            if (shoppingcartVM != null && shoppingcartVM.CartList != null)
            {
                foreach (var item in shoppingcartVM.CartList)
                {
                    OrderDetail orderDetail = new OrderDetail()
                    {
                        ProductId = item.ProductId,
                        OrderId = shoppingcartVM.OrderHeader.Id,
                        Price = item.Product.Price,
                        Count = item.Count
                    };

                    _unitOfWork.OrderDetail.add(orderDetail);
                    _unitOfWork.complete();


                }
            }
            ShoppingCartVM = shoppingcartVM;


            var domin = "http://localhost:1175/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),


                Mode = "payment",
                SuccessUrl = domin +$"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domin + $"customer/cart/index",
            };
            if (shoppingcartVM != null && shoppingcartVM.CartList != null)
            {
                foreach (var item in shoppingcartVM.CartList)
                {

                    var sessionlineOption = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            
                            UnitAmount = (long)(item.Product.Price * 100)+(long)((long)(item.Product.Price * 100) * 0.01),
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name,
                            },
                            
                        },
                        Quantity = item.Count,
                    };
                    options.LineItems.Add(sessionlineOption);
                }
            }


            var service = new SessionService();
            Session session = service.Create(options);
            shoppingcartVM.OrderHeader.SessionId = session.Id;
            shoppingcartVM.OrderHeader.PaymentIntentId = session.PaymentIntentId;
            _unitOfWork.complete();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);





        }

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.GetByID(u => u.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.updateOrderStates(id, SD.Approve, SD.Approve);
                _unitOfWork.complete();
            }
            List<ShopingCart> shoppingcarts = _unitOfWork.ShoppingCart.GetAll(u=>u.applicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.removeRange(shoppingcarts);
            _unitOfWork.complete();

            return View("confirm",id);
        }

        public IActionResult Plus(int cartid) 
        {
            var shoppingcart = _unitOfWork.ShoppingCart.GetAll().FirstOrDefault(x => x.ID == cartid);
            _unitOfWork.ShoppingCart.IncreaseCount(shoppingcart, 1);
            _unitOfWork.complete();
            return RedirectToAction("Index");
        }        

        public IActionResult Minus(int cartid) 
        {

            var shoppingcart = _unitOfWork.ShoppingCart.GetAll().FirstOrDefault(x => x.ID == cartid);
            if (shoppingcart.Count<=1)
            {
                _unitOfWork.ShoppingCart.remove(shoppingcart);
				_unitOfWork.complete();
				return RedirectToAction("Index","Home");
			}
            else
            {
            _unitOfWork.ShoppingCart.DecreaseCount(shoppingcart, 1);
            }
            _unitOfWork.complete();
            return RedirectToAction("Index");
        }
		public IActionResult Remove(int cartid)
		{
			var shoppingcart = _unitOfWork.ShoppingCart.GetAll().FirstOrDefault(x => x.ID == cartid);
			_unitOfWork.ShoppingCart.remove(shoppingcart);
			_unitOfWork.complete();
			return RedirectToAction("Index");
		}

	}
}
