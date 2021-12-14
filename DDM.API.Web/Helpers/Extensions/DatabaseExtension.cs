using DDM.API.Infrastructure.Data.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDM.API.Web.Helpers.Extensions
{
    public static class DatabaseExtension
    {
        public static void AddDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DDMDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DDMDbConnectionString")));
        }
    }
}
