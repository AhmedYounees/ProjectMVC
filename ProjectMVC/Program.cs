using DataAccessLayer.Data;
using DataAccessLayer.Implementation;
using Entities.Models;
using Entities.Reposatories;
using Microsoft.EntityFrameworkCore;
using Entities.Reposatories ;

namespace ProjectMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<ApplicationDbContext>(option =>
            {
                option.UseSqlServer(builder.Configuration.GetConnectionString("cs"));
            });
            builder.Services.AddScoped<ApplicationDbContext>();
            builder.Services.AddScoped<IUnitOfWork,UnitOfWok>();
            //builder.Services.AddScoped<IGeneircRepository<Category>,CategoryRepository>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
