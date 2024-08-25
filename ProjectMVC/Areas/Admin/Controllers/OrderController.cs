using DataAccessLayer.Implementation;
using Entities.Models;
using Entities.Reposatories;
using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ProjectMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult GetData() 
        {
            IEnumerable<OrderHeader> orderHeaders;
            orderHeaders = _unitOfWork.OrderHeader.GetAll(icludeWord: "ApplicationUser");
            return Json(new {data= orderHeaders});
        
        }

        public IActionResult Details(int orderid) 
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = _unitOfWork.OrderHeader.GetByID(u=>u.Id == orderid , icludeWord: "ApplicationUser"),
                OrderDetails = _unitOfWork.OrderDetail.GetAll(x => x.OrderId == orderid, icludeWord: "Product")
            };
            return View("Details", orderVM);
        }


    }
}
