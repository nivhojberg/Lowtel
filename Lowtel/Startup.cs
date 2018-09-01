using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EF.AspNetCore.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Lowtel
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);        

            // Get DB connection string from appsetting json and replace the DB file path,
            // with the current project folder.
            string connection = Configuration.GetConnectionString("LowtelContext");            
            connection = connection.Replace("{current_dir}", Environment.CurrentDirectory);
            
            services.AddDbContext<LotelContext>(options => options.UseSqlServer(connection));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "RoomsCreate",
                    template: "{controller=Rooms/Create}/{action=Create}");

                routes.MapRoute(
                    name: "RoomsEdit",
                    template: "{controller=Rooms/Edit}/{action=Edit}/{id?}/{hotelId?}");

                routes.MapRoute(
                    name: "RoomsDetails",
                    template: "{controller=Rooms/Details}/{action=Details}/{id?}/{hotelId?}");
            });
        }
    }
}
