using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public class ApplicationDbContext:IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext>options):base(options) 
        {
            
        }

        public DbSet<Category> categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ApplicationUser> applicationUsers { get; set;}
        public DbSet<ShopingCart> shopingCarts { get; set;}        
        public DbSet<OrderHeader> OrderHeaders { get; set;}
        public DbSet<OrderDetail> OrderDetails { get; set;}
    }
}
