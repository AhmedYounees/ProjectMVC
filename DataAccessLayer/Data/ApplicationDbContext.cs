using Microsoft.EntityFrameworkCore;
using Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

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
        public DbSet<review> reviews { get; set;}
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<OrderDetail>()
        //        .HasKey(od => od.OrderId); // This should be correctly set up

        //    modelBuilder.Entity<OrderDetail>()
        //        .Property(od => od.OrderId)
        //        .ValueGeneratedOnAdd(); // Ensure it's marked as an identity column
        //    modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
        //    {
        //        entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
        //        // Other configurations if needed
        //    });
        //}
    }
}
