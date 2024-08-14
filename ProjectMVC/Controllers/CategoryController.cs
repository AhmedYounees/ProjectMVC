using Microsoft.AspNetCore.Mvc;
using ProjectMVC.Models;
using ProjectMVC.Repository;

namespace ProjectMVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly IGeneircRepository<Category> categorys;

        public CategoryController(IGeneircRepository<Category> categorys)
        {
            this.categorys = categorys;
        }
        public IActionResult Index()
        {
            var CategoryList=categorys.GetAll();

            return View(CategoryList);
        }

        [HttpGet]
        public IActionResult Create()
        {


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if(ModelState.IsValid)
            {
                categorys.Add(category);
                categorys.Save();
                return RedirectToAction("Index");

            }


            return View(category);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
             if(id==0)
            {
                return NotFound();
            }
            else
            {
                Category CategoryFromDataBase = categorys.GetById(id);
               // categorys.Update(id, CategoryFromDataBase);
               // categorys.Save();
                return View(CategoryFromDataBase);
            }

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {

             int IDFromDataBase=category.id;

            categorys.Update(IDFromDataBase,category);
            categorys.Save();
            return RedirectToAction("Index");

        }
        [HttpGet]
        public IActionResult Delete(int id)
        {
             Category CategoryFromDataBase=categorys.GetById(id);
            return View(CategoryFromDataBase);


        }
        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            categorys.Delete(id);
            categorys.Save();
            return RedirectToAction("index");
        }

    }
}
