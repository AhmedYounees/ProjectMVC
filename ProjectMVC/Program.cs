using DataAccessLayer.Data;
using DataAccessLayer.Implementation;
using Entities.Reposatories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Utilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;

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
                option.UseSqlServer(builder.Configuration.GetConnectionString("cs"),
                    b => b.MigrationsAssembly("DataAccessLayer"));

            });
            builder.Services.Configure<StripeData>(builder.Configuration.GetSection("stripe"));
          // builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ApplicationDbContext>();
           builder.Services.AddDefaultIdentity<IdentityUser>
                (options => options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5)).AddDefaultUI()
                .AddRoles<IdentityRole>()
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();
            builder.Services.AddScoped<ApplicationDbContext>();
            builder.Services.AddScoped<IUnitOfWork,UnitOfWok>();
            builder.Services.AddSingleton<IEmailSender, EmailSender>();
            //builder.Services.AddScoped<IGeneircRepository<Category>,CategoryRepository>();
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();
            StripeConfiguration.ApiKey = builder.Configuration.GetSection("stripe:Secretkey").Get<string>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.MapRazorPages();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Admin}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "Customer",
                pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

            
            app.Run();
        }
    }
}
