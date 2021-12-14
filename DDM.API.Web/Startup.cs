using DDM.API.Core.Services.v1.Abstract;
using DDM.API.Core.Services.v1.Concrete;
using DDM.API.Infrastructure.Data.Application;
using DDM.API.Infrastructure.Data.Identiity;
using DDM.API.Infrastructure.Entities.Roles;
using DDM.API.Web.Helpers.Extensions;
using DDM.API.Web.Helpers.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDM.API.Web
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

            services.AddControllers();
            services.AddDb(Configuration);
            services.AddJwtAuthentication(Configuration);
            services.AddMvcCoreFramework(Configuration);
         //   services.AddAppServices(Configuration);
            services.AddAppAuthorization(Configuration);
            services.AddVersioning();
            services.AddSwagger();

            // Authentication

            // Adding JWT

            // Auto mapper
            services.AddAutoMapper(typeof(Startup));

            // Dependency Injection
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAdminService, AdminService>();
            //services.AddScoped<IMerchantService, MerchantService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // CORS (not safe but what the hell)

            // Routing to Lowercase
            services.AddRouting(options => options.LowercaseUrls = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseVersionedSwagger(provider);
            //Enable CORS
            app.UseCors("AllowAllOrigins");
            //   app.UseCors("AllowAll");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            //app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller}/{action=Index}/{id?}");
            });

            //DB Seeding
            //CreateRoles(serviceProvider);
            // DDMDbInitializer.SeedRoles(app).Wait();
        }

        private void CreateRoles(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            Task<IdentityResult> roleResult;
            string email = "admin@admin.com";
            string username = "admin";

            //Check that there is an Administrator role and create if not
            Task<bool> hasAdminRole = roleManager.RoleExistsAsync(UserRoles.Admin);
            hasAdminRole.Wait();

            if (!hasAdminRole.Result)
            {
                ApplicationRole roleCreate = new ApplicationRole();
                roleCreate.Name = UserRoles.Admin;
                roleResult = roleManager.CreateAsync(roleCreate);
                roleResult.Wait();
            }

            //Check if the admin user exists and create it if not
            //Add to the Administrator role

            Task<ApplicationUser> testUser = userManager.FindByEmailAsync(email);
            testUser.Wait();

            if (testUser.Result == null)
            {
                ApplicationUser administrator = new ApplicationUser();
                administrator.Email = email;
                administrator.UserName = username;

                Task<IdentityResult> newUser = userManager.CreateAsync(administrator, "12345");
                newUser.Wait();

                if (newUser.Result.Succeeded)
                {
                    Task<IdentityResult> newUserRole = userManager.AddToRoleAsync(administrator, UserRoles.Admin);
                    newUserRole.Wait();
                }
            }

            //create New Role Merchant
            Task<IdentityResult> roleResult0;
            //Check that there is an Merchant role and create if not
            Task<bool> hasMerchantRole = roleManager.RoleExistsAsync(UserRoles.Merchant);
            hasMerchantRole.Wait();

            if (!hasMerchantRole.Result)
            {
                ApplicationRole roleCreate = new ApplicationRole();
                roleCreate.Name = UserRoles.Merchant;
                roleResult0 = roleManager.CreateAsync(roleCreate);
                roleResult0.Wait();
            }

            //create New Role Staff
            Task<IdentityResult> roleResult1;
            //Check that there is an Staff role and create if not
            Task<bool> hasStaffRole = roleManager.RoleExistsAsync(UserRoles.Staff);
            hasStaffRole.Wait();

            if (!hasStaffRole.Result)
            {
                ApplicationRole roleCreate = new ApplicationRole();
                roleCreate.Name = UserRoles.Staff;
                roleResult1 = roleManager.CreateAsync(roleCreate);
                roleResult1.Wait();
            }

            //create New Role User
            Task<IdentityResult> roleResult2;
            //Check that there is an User role and create if not
            Task<bool> hasUserRole = roleManager.RoleExistsAsync(UserRoles.User);
            hasUserRole.Wait();

            if (!hasStaffRole.Result)
            {
                ApplicationRole roleCreate = new ApplicationRole();
                roleCreate.Name = UserRoles.User;
                roleResult2 = roleManager.CreateAsync(roleCreate);
                roleResult2.Wait();
            }
        }
    }
}
