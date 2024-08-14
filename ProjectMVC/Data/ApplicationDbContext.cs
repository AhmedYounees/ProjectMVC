using Microsoft.EntityFrameworkCore;
using ProjectMVC.Models;

namespace ProjectMVC.Data
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options) 
        {
            
        }

        public DbSet<Category>categories { get; set; }
    }
}
