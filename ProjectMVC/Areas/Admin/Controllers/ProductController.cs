using Entities.Models;
using Entities.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Entities.Reposatories;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var ProductList = _unitOfWork.Product.GetAll();

            return View(ProductList);
        }

        [HttpGet]
        public IActionResult Create()
        {

            var categorySelectList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem
            {
                Value = c.id.ToString(),
                Text = c.name 
            }).ToList();
    
   

            ViewData["CategorySelectList"] = categorySelectList;
            return View("Create", new Product());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Product productFReq,IFormFile file)
        {
           
            if (ModelState.IsValid)
            {
                string RootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var upload = Path.Combine(RootPath, @"Images\Products");
                    var ext = Path.GetExtension(file.FileName);
                    using (var filestream = new FileStream(Path.Combine(upload, fileName + ext), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    productFReq.img = @"Images\Products\" + fileName + ext;
                }

                _unitOfWork.Product.add(productFReq);
                _unitOfWork.complete();
                TempData["Type"] = "success";
                TempData["message"] = "created successfully";
                return RedirectToAction("Index");

            }


            return View(productFReq);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            else
            {
                Product ProductFromDataBase = _unitOfWork.Product.GetByID(x => x.Id == id);
                // products.Update(id, ProductFromDataBase);
                // products.Save();
                return View(ProductFromDataBase);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product)
        {

            int IDFromDataBase = product.Id;


            _unitOfWork.Product.update(product);
            _unitOfWork.complete();
            TempData["Type"] = "info";
            TempData["message"] = "Updated successfully";
            return RedirectToAction("Index");

        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == null | id == 0)
            {
                NotFound();
            }
            Product ProductFromDataBase = _unitOfWork.Product.GetByID(x => x.Id == id);
            return View(ProductFromDataBase);


        }
        [HttpPost]
        public IActionResult DeleteProduct(int id)
        {
            var productDB = _unitOfWork.Product.GetByID(x => x.Id == id);
            if (productDB == null)
            {
                NotFound();
            }
            _unitOfWork.Product.remove(productDB);
            _unitOfWork.complete();
            TempData["Type"] = "error";
            TempData["message"] = "Deleted successfully";
            return RedirectToAction("index");
        }

    }
}
