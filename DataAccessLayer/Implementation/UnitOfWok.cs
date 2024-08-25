using DataAccessLayer.Data;
using Entities.Models;
using Entities.Reposatories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Implementation
{
    public class UnitOfWok : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public ICategoryRepo Category { get; private set; }
        public IProductRepo Product { get; private set; }
        public IShopingCartRepo ShoppingCart { get; private set; }
        public IOrderHeaderRepo OrderHeader { get; private set; }
        public IOrderDetailRepo OrderDetail { get; private set; }
        public IApplicationUserRepo ApplicationUser { get; private set; }
        public UnitOfWok (ApplicationDbContext context) 
        {
            _context = context;
            Category=new CategoryRepo(context);
            Product=new ProductRepo(context);
            ShoppingCart=new ShoppingCartRepo(context);
            OrderHeader=new OredrHeaderRepo(context);   
            OrderDetail=new OredrDetailRepo(context);
            ApplicationUser=new ApplicationUserRepo(context);
        }
        

        public int complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
