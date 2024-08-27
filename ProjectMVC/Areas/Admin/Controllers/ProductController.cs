using Entities.Models;
using Entities.ViewModels;

using Microsoft.AspNetCore.Mvc;

using Entities.Reposatories;
using Microsoft.AspNetCore.Mvc.Rendering;
using Utilities;
using Microsoft.AspNetCore.Authorization;

namespace ProjectMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]

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
            

            return View("Index");
        }
        public IActionResult getData()
        {
            var product = _unitOfWork.Product.GetAll(icludeWord:"Category");
            return Json(new {data=product});
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
                    var upload = Path.Combine(RootPath, @"");
                    var ext = Path.GetExtension(file.FileName);
                    using (var filestream = new FileStream(Path.Combine(upload, fileName + ext), FileMode.Create))
                    {
                        file.CopyTo(filestream);
                    }
                    productFReq.img = @"" + fileName + ext;
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
            if (id == null|id == 0)
            {
                return NotFound();
            }
            //ProductVM VM = new ProductVM()
            //{
            //    Product = _unitOfWork.Product.GetByID(x => x.Id == id),
            //    Categories = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
            //    {
            //        Text = x.name,
            //        Value = x.id.ToString()
            //    })

            //};

            var product = _unitOfWork.Product.GetByID(x=>x.Id==id);
            var categorylist=_unitOfWork.Category.GetAll();
            ViewBag.category=categorylist;
            return View(product);
              
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Product product,IFormFile? file)
        {

            string RootPath = _webHostEnvironment.WebRootPath;
            if (file != null)
            {
                string fileName = Guid.NewGuid().ToString();
                var upload = Path.Combine(RootPath, @"Images\Products");
                var ext = Path.GetExtension(file.FileName);

                if (product.img != null) { 
                
                    var oldimg= Path.Combine(RootPath,product.img.TrimStart('\\'));
                    if (System.IO.File.Exists(oldimg))
                    {
                        System.IO.File.Delete(oldimg);
                    }
                
                }


                using (var filestream = new FileStream(Path.Combine(upload, fileName + ext), FileMode.Create))
                {
                    file.CopyTo(filestream);
                }
                product.img = @"Images\Products\" + fileName + ext;
            }

            _unitOfWork.Product.update(product);
            _unitOfWork.complete();
            TempData["Type"] = "info";
            TempData["message"] = "Updated successfully";
            return RedirectToAction("Index");

        }
        //[HttpGet]
        //public IActionResult Delete(int id)
        //{
        //    if (id == null | id == 0)
        //    {
        //        NotFound();
        //    }
        //    Product ProductFromDataBase = _unitOfWork.Product.GetByID(x => x.Id == id);
        //    return View(ProductFromDataBase);


        //}
        [HttpDelete]
        public IActionResult DeleteProduct(int id)
        {
            var productDB = _unitOfWork.Product.GetByID(x => x.Id == id);
            if (productDB == null)
            {
                Json(new { success = false , message = "Error While Deleting"});
            }
            _unitOfWork.Product.remove(productDB);
            var oldimg = Path.Combine(_webHostEnvironment.WebRootPath, productDB.img.TrimStart('\\'));
            if (System.IO.File.Exists(oldimg))
            {
                System.IO.File.Delete(oldimg);
            }
            _unitOfWork.complete();
            //TempData["Type"] = "error";
            return Json(new { success = true, message = "file has Deleting" });
            //return RedirectToAction("Index");
        }

    }
}
