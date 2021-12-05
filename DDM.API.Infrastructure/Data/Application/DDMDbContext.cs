using DDM.API.Infrastructure.Data.Identiity;
using DDM.API.Infrastructure.Entities.Models;
using DDM.API.Infrastructure.Entities.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DDM.API.Infrastructure.Data.Application
{
    public class DDMDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long, IdentityUserClaim<long>, ApplicationUserRole, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>
    {
        public DDMDbContext(DbContextOptions<DDMDbContext> options)
            : base(options)
        {
        }

        public DbSet<Log> zib_logs { get; set; }
        public DbSet<RefreshToken> zib_refresh_tokens { get; set; }

        //Dedails About Under Method-https://entityframework.net/knowledge-base/39798317/identityuserlogin-string---requires-a-primary-key-to-be-defined-error-while-adding-migration
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });
        }
    }
}
