using Entities.Reposatories;
using Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using System.Security.Claims;

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
