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
    public class ProductRepo : GenaricRepo<Product>, IProductRepo
    {
        private readonly ApplicationDbContext _context;
        public ProductRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void update(Product product)
        {
            var productinDB =_context.Products.FirstOrDefault(x=>x.Id==product.Id);
            if (productinDB != null) { 
            productinDB.Name = product.Name;
            productinDB.Description = product.Description;
            productinDB.Price=product.Price;
            productinDB.CategoryId=product.CategoryId;
            productinDB.img = product.img;
            
            
            }
        }
    }
}
