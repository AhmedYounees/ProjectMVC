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
    public class ApplicationUserRepo : GenaricRepo<ApplicationUser>,IApplicationUserRepo
    {
        private readonly ApplicationDbContext _context;
        public ApplicationUserRepo(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

       
    }
}
