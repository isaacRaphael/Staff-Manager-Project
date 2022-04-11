using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StaffManagement.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using StaffManagement.Models;
using StaffManagement.Contracts;
using StaffManagement.Repositories;
using StaffManagement.Services;

namespace StaffManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("default")));
            //services.AddIdentity<Staff, IdentityRole>().AddDefaultTokenProviders();
            services.AddIdentity<Staff, IdentityRole>( options =>
            {
                options.Password.RequiredLength = 5;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
            }
                ).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            
            services.AddScoped<IStaffRepository, StaffRepository>();
            services.AddSingleton<IEmailService>(x => new EmailService(
                Configuration.GetSection("smtp")["email"], 
                Configuration.GetSection("smtp")["password"],
                Configuration.GetSection("smtp")["host"]
                )
            );
            services.AddScoped<IImageService>(x => new ImageService(
                Configuration.GetSection("Cloudinary")["Name"],
                Configuration.GetSection("Cloudinary")["Key"],
                Configuration.GetSection("Cloudinary")["Secret"]));
            services.ConfigureApplicationCookie(options => options.AccessDeniedPath = "/Unauthorized/Denied");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, 
                    IWebHostEnvironment env,
                    AppDbContext db,
                    RoleManager<IdentityRole> roleManager,
                    UserManager<Staff> userManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            SeedAdmin.Seed(db, roleManager, userManager).Wait();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Staff}/{action=Login}/{id?}");
            });
        }
    }
}
