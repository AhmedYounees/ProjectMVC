using Entities.Models;
using Entities.Reposatories;
using Entities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
        private readonly ICompositeViewEngine _viewEngine;

        public CartController(IUnitOfWork unitOfWork, ICompositeViewEngine viewEngine)
        {
            _unitOfWork = unitOfWork;
            _viewEngine = viewEngine;
        }

        public IActionResult Index()
        {
            var shoppingCartVM = GetShoppingCartVM();
            ViewBag.IsCartPage = true;
            return View(shoppingCartVM);
        }

        [HttpGet]
        public IActionResult Summary()
        {
            var shoppingCartVM = GetShoppingCartVM();
            PopulateOrderHeader(shoppingCartVM);
            ViewBag.IsCartPage = true; 
            return View(shoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult SummaryPost(ShoppingCartVM shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM.CartList = _unitOfWork.ShoppingCart.GetAll(u => u.applicationUserId == claim.Value, icludeWord: "Product");

            shoppingCartVM.OrderHeader.OrderStatus = SD.Pending;
            shoppingCartVM.OrderHeader.PaymentSatuts = SD.Pending;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            shoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;

            CalculateTotalPrice(shoppingCartVM);
            CreateOrderHeader(shoppingCartVM);
            CreateOrderDetails(shoppingCartVM);

            var domain = "http://localhost:1175/";
            var options = CreateSessionOptions(shoppingCartVM, domain);

            var service = new SessionService();
            Session session = service.Create(options);
            shoppingCartVM.OrderHeader.SessionId = session.Id;
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
                orderHeader.PaymentIntentId = session.PaymentIntentId;
                _unitOfWork.complete();
            }

            List<ShopingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetAll(u => u.applicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.ShoppingCart.removeRange(shoppingCarts);
            _unitOfWork.complete();

            return View("Confirm", id);
        }

        [HttpPost]
        public IActionResult Plus(int cartId)
        {
            UpdateCartItemCount(cartId, 1);
            return PartialView("_CartWrapper", GetShoppingCartVM());
        }

        [HttpPost]
        public IActionResult Minus(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.GetByID(u => u.ID == cartId);
            if (cartFromDb.Count > 1)
            {
                UpdateCartItemCount(cartId, -1);
            }
            return PartialView("_CartWrapper", GetShoppingCartVM());
        }

        [HttpPost]
        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.GetByID(u => u.ID == cartId);
            _unitOfWork.ShoppingCart.remove(cartFromDb);
            _unitOfWork.complete();
            return PartialView("_CartWrapper", GetShoppingCartVM());
        }

        [HttpGet]
        public IActionResult GetCartItems()
        {
            var shoppingCartVM = GetShoppingCartVM();
            string cartItemsHtml = RenderViewToString("_CartItems", shoppingCartVM);

            return Json(new
            {
                cartItemsHtml = cartItemsHtml,
                total = shoppingCartVM.TotalCarts,
                itemCount = shoppingCartVM.CartList.Sum(c => c.Count)
            });
        }

        private ShoppingCartVM GetShoppingCartVM()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM shoppingCartVM = new()
            {
                CartList = _unitOfWork.ShoppingCart.GetAll(u => u.applicationUserId == userId, icludeWord: "Product"),
                OrderHeader = new(),
                TotalCarts = 0
            };

            foreach (var cart in shoppingCartVM.CartList)
            {
                shoppingCartVM.TotalCarts += (cart.Product.Price * cart.Count);
            }

            return shoppingCartVM;
        }

        private void PopulateOrderHeader(ShoppingCartVM shoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetByID(x => x.Id == claim.Value);

            shoppingCartVM.OrderHeader.Name = shoppingCartVM.OrderHeader.ApplicationUser.Name;
            shoppingCartVM.OrderHeader.Address = shoppingCartVM.OrderHeader.ApplicationUser.Address;
            shoppingCartVM.OrderHeader.City = shoppingCartVM.OrderHeader.ApplicationUser.City;
            shoppingCartVM.OrderHeader.Phone = shoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;

            foreach (var item in shoppingCartVM.CartList)
            {
                shoppingCartVM.OrderHeader.TotalPrice += (item.Count * item.Product.Price);
            }
        }

        private void CalculateTotalPrice(ShoppingCartVM shoppingCartVM)
        {
            foreach (var item in shoppingCartVM.CartList)
            {
                shoppingCartVM.OrderHeader.TotalPrice += (item.Count * item.Product.Price);
            }
        }

        private void CreateOrderHeader(ShoppingCartVM shoppingCartVM)
        {
            _unitOfWork.OrderHeader.add(shoppingCartVM.OrderHeader);
            _unitOfWork.complete();
        }

        private void CreateOrderDetails(ShoppingCartVM shoppingCartVM)
        {
            foreach (var item in shoppingCartVM.CartList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderId = shoppingCartVM.OrderHeader.Id,
                    Price = item.Product.Price,
                    Count = item.Count
                };

                _unitOfWork.OrderDetail.add(orderDetail);
            }
            _unitOfWork.complete();
        }

        private SessionCreateOptions CreateSessionOptions(ShoppingCartVM shoppingCartVM, string domain)
        {
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + $"customer/cart/index",
            };

            foreach (var item in shoppingCartVM.CartList)
            {
                var sessionLineOption = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price * 100) + (long)((long)(item.Product.Price * 100) * 0.01),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineOption);
            }

            return options;
        }

        private void UpdateCartItemCount(int cartId, int countChange)
        {
            var cartFromDb = _unitOfWork.ShoppingCart.GetByID(u => u.ID == cartId);
            cartFromDb.Count += countChange;
            _unitOfWork.complete();
        }

        private string RenderViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = _viewEngine.FindView(ControllerContext, viewName, false);
                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );
                viewResult.View.RenderAsync(viewContext);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}