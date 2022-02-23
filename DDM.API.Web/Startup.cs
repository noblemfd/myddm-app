using DDM.API.Core.EntityMappe.v1;
using DDM.API.Core.EntityMapper.v1;
using DDM.API.Core.Helpers;
using DDM.API.Core.ProfileMapping.v1;
using DDM.API.Core.Services.v1.Abstract;
using DDM.API.Core.Services.v1.Concrete;
using DDM.API.Infrastructure.Data.Application;
using DDM.API.Infrastructure.Data.Identiity;
using DDM.API.Infrastructure.Entities.Roles;
using DDM.API.Web.Helpers.Extensions;
using DDM.API.Web.Helpers.Filters;
using DDM.API.Web.Middleware;
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
          //  services.AddControllers();
            services.AddControllers().AddJsonOptions(options =>
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = false);
            #region Auto-Mapper
            services.AddApplicationLayer();
            #endregion
            services.AddTransient<UserResolverService>();
            services.AddDb(Configuration);
            services.AddJwtAuthentication(Configuration);
            services.AddMvcCoreFramework(Configuration);
            services.AddAppAuthorization(Configuration);
            #region Api Versioning
            // Add API Versioning to the Project
            services.AddVersioning();
            #endregion
            #region Swagger
            services.AddSwagger();
            #endregion

            // Dependency Injection
            services.AddAppServices(Configuration);

            // Routing to Lowercase
            services.AddRouting(options => options.LowercaseUrls = true);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, IApiVersionDescriptionProvider provider, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            app.UseMiddleware<ExceptionMiddleware>();
            //app.UseDeveloperExceptionPage();
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            #region Swagger
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseVersionedSwagger(provider);
            #endregion
            #region CORS
          //  app.UseCors("MyCorsImplementationPolicy");
           // app.UseCors("AllowAll");
            app.UseCors("AllowAllOrigins");
            //  app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            #endregion
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //DB Seeding
            DDMDbInitializer.SeedData(userManager, roleManager);
        }
    }
}
