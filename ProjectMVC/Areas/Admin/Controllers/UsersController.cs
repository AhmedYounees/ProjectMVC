using DataAccessLayer.Data;
using Entities.Reposatories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;
using System.Security.Claims;
using Utilities;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace ProjectMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.AdminRole)]
    public class UsersController : Controller
    {
        ApplicationDbContext _context;

        public UsersController(ApplicationDbContext applicationDb)
        {
            _context = applicationDb;
        }
        public IActionResult Index()
        {
            var claimsidentity =(ClaimsIdentity)User.Identity;
            var claim = claimsidentity.FindFirst(ClaimTypes.NameIdentifier);
            string userId = claim.Value;
            return View(_context.applicationUsers.Where(x =>x.Id!=userId).ToList());
        }
        public IActionResult LockUnlock(string? id)
        {
            var user = _context.applicationUsers.FirstOrDefault(x => x.Id==id);
            if (user==null)
            {
                return NotFound();
            }
            if(user.LockoutEnd== null || user.LockoutEnd<DateTime.Now)
            {
                user.LockoutEnd = DateTime.Now.AddDays(21);

            }
            else
            {
                user.LockoutEnd= DateTime.Now;

            }
            _context.SaveChanges();
            return RedirectToAction("Index","Users",new {area = "Admin"});
        }
        public IActionResult Delete(string id)
        {
            var user=_context.applicationUsers.FirstOrDefault(x=>x.Id==id);
            _context.Remove(user);
            _context.SaveChanges();     
            return RedirectToAction("index");
        }
    }
}
