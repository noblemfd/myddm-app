using DDM.API.Core.EntityMappe.v1;
using DDM.API.Core.EntityMapper.v1;
using DDM.API.Core.ProfileMapping.v1;
using DDM.API.Core.Services.v1.Abstract;
using DDM.API.Core.Services.v1.Concrete;
using DDM.API.Web.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDM.API.Web.Helpers.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            // caching needs to be singleton for any request
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IMerchantService, MerchantService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = acttionContext =>
                {
                    var errors = acttionContext.ModelState
                                               .Where(e => e.Value.Errors.Count > 0)
                                               .SelectMany(x => x.Value.Errors)
                                               .Select(x => x.ErrorMessage).ToArray();
                    var errorResponse = new APIValidationError
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(errorResponse);
                };
            });

            return services;
        }
        public static void AddApplicationLayer(this IServiceCollection services)
        {
            //  services.AddAutoMapper(typeof(Startup));
            services.AddAutoMapper(typeof(AuthMapperProfile));
            services.AddAutoMapper(typeof(AdminMapperProfile));
            services.AddAutoMapper(typeof(MerchantMapperProfile));
            //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            //services.AddMediatR(Assembly.GetExecutingAssembly());
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        }
    }
}
