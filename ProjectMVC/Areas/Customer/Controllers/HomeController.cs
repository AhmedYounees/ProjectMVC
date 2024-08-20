using Entities.Reposatories;
using Microsoft.AspNetCore.Mvc;
using Entities.Models;
using Entities.ViewModels;

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
        public IActionResult Detalis(int id)
        {
            ShopingCartVM shopincartvm = new ShopingCartVM()
            {
                Product= unitOfWork.Product.GetByID(p => p.Id == id, icludeWord: "Category"),
                Count=1,
        };
          //  var product = unitOfWork.Product.GetByID(p => p.Id == id, icludeWord: "Category");

            return View("Details", shopincartvm);

        }
    }
}
