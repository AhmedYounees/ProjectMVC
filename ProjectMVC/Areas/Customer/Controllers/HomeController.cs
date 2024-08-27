using Entities.Reposatories;
using Microsoft.AspNetCore.Mvc;
using Entities.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DataAccessLayer.Data;
using Utilities;
using System.Linq;
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
            var viewModel = new HomeViewModel()
            {
                FeaturedCategories = unitOfWork.Category.GetAll().Take(6).ToList(),
                TopOffers = unitOfWork.Product.GetAll(p => p.Price < 20000, icludeWord: "Category").Take(4).ToList(),
                NewArrivals = unitOfWork.Product.GetAll(icludeWord: "Category").OrderByDescending(p => p.Id).Take(8).ToList(),
                PopularProducts = unitOfWork.Product.GetAll(icludeWord: "Category").Take(4).ToList()
            };

            return View(viewModel);
        }

        public IActionResult Home(string categoryName, string searchTerm, decimal? minPrice, decimal? maxPrice, string sortBy, int page = 1, int pageSize = 12)
        {
            IEnumerable<Product> products = unitOfWork.Product.GetAll((p) => true, icludeWord: "Category");

            if (!string.IsNullOrEmpty(categoryName))
            {
                products = products.Where(p => p.Category.name == categoryName);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                products = products.Where(p => p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                                            || (p.Description != null && p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
            }

            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value);
            }

            switch (sortBy)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "name_asc":
                    products = products.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.Name);
                    break;
                default:
                    products = products.OrderBy(p => p.Id);
                    break;
            }

            var total = products.Count();
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);
            var result = products.Skip((page - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToList();

            ViewBag.TotalPages = totalPages;
            ViewBag.CurrentPage = page;
            ViewBag.CurrentCategory = categoryName;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.SortBy = sortBy;
            ViewBag.Categories = unitOfWork.Category.GetAll().Select(c => c.name).Distinct().ToList();

            return View(result);
        }

        // Other actions remain the same

        [HttpPost]
        [Authorize]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;

            var cartItem = unitOfWork.ShoppingCart.GetByID(
                u => u.applicationUserId == userId && u.ProductId == productId);

            if (cartItem == null)
            {
                cartItem = new ShopingCart
                {
                    ProductId = productId,
                    applicationUserId = userId,
                    Count = quantity
                };
                unitOfWork.ShoppingCart.add(cartItem);
            }
            else
            {
                unitOfWork.ShoppingCart.IncreaseCount(cartItem, quantity);
            }

            unitOfWork.complete();

            return Json(new { success = true, message = "Product added to cart successfully." });
        }

        public IActionResult Detalis(int ProductID)
        {
            ShopingCart shopincartvm = new ShopingCart()
            {
                ProductId = ProductID,
                Product = unitOfWork.Product.GetByID(p => p.Id == ProductID, icludeWord: "Category"),
                Count = 1,
            };
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
                u => u.applicationUserId == claim.Value && u.ProductId == shoppinCart.ProductId);

            if (cartFromDatabase == null)
            {
                unitOfWork.ShoppingCart.add(shoppinCart);
                HttpContext.Session.SetInt32(SD.SessionKey,
                    unitOfWork.ShoppingCart.GetAll(x => x.applicationUserId == claim.Value).ToList().Count()
                );
                unitOfWork.complete();
            }
            else
            {
                unitOfWork.ShoppingCart.IncreaseCount(cartFromDatabase, shoppinCart.Count);
                unitOfWork.complete();
            }

            return RedirectToAction("index","Home");
        }
    }
}