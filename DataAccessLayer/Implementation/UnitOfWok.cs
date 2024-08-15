using DataAccessLayer.Data;
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
        public UnitOfWok (ApplicationDbContext context) 
        {
            _context = context;
            Category=new CategoryRepo(context);
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
