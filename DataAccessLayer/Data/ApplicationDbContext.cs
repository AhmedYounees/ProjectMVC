using Microsoft.EntityFrameworkCore;
using Entities.Models;

namespace DataAccessLayer.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options) 
        {
            
        }

        public DbSet<Category> categories { get; set; }
    }
}
