using Entities.Reposatories;
using Microsoft.AspNetCore.Mvc;
using Entities.Models;
using System.Security.Claims;
//using System.Web.Mvc;
using Controller = Microsoft.AspNetCore.Mvc.Controller;
using Microsoft.AspNetCore.Authorization;
using DataAccessLayer.Data;

namespace ProjectMVC.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
       
        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
            
        }
        
        public IActionResult Index()
        {
            
            var ProductList=unitOfWork.Product.GetAll();
            return View("Index",ProductList);
        }
        
        public IActionResult Detalis(int ProductID)
        {
            ShopingCart shopincartvm = new ShopingCart()
            {
                ProductId = ProductID,
                Product= unitOfWork.Product.GetByID(p => p.Id == ProductID, icludeWord: "Category"),
                Count=1,
        };
          //  var product = unitOfWork.Product.GetByID(p => p.Id == id, icludeWord: "Category");

            return View("Details", shopincartvm);

        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public IActionResult Detalis(ShopingCart shoppinCart)
        {
            var claimsidentity = (ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);
            shoppinCart.applicationUserId = claim.Value;
            var cartFromDatabase = unitOfWork.ShoppingCart.GetByID(
                u=>u.applicationUserId==claim.Value && u.ProductId==shoppinCart.ProductId);

            
            if(cartFromDatabase==null)
            {

                unitOfWork.ShoppingCart.add(shoppinCart);

            }
            else
            {
                unitOfWork.ShoppingCart.IncreaseCount(cartFromDatabase, shoppinCart.Count);
               
                unitOfWork.complete();
            }

            //  var product = unitOfWork.Product.GetByID(p => p.Id == id, icludeWord: "Category");
         
            unitOfWork.complete();
            //  return View("Details", shopincartvm);
            return RedirectToAction("index");
        }
    }
}
