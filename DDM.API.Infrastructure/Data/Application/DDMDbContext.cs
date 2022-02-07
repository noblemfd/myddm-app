using DDM.API.Infrastructure.Data.Identiity;
using DDM.API.Infrastructure.Entities.Common;
using DDM.API.Infrastructure.Entities.Models;
using DDM.API.Infrastructure.Entities.Models.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static DDM.API.Infrastructure.Helpers.EnumList;

namespace DDM.API.Infrastructure.Data.Application
{
    public class DDMDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, long, IdentityUserClaim<long>, ApplicationUserRole, IdentityUserLogin<long>, IdentityRoleClaim<long>, IdentityUserToken<long>>
    {
        public DDMDbContext(DbContextOptions<DDMDbContext> options)
            : base(options)
        {
        }

        public DbSet<Mandate> zib_mandates { get; set; }
        public DbSet<MandateDetail> zib_mandate_details { get; set; }
        public DbSet<Merchant> zib_merchants { get; set; }
        public DbSet<StaffMember> zib_staff_members { get; set; }
        public DbSet<MerchantUser> zib_merchant_users { get; set; }
        public DbSet<NotificationLog> zib_notification_logs { get; set; }
        public DbSet<TransactionLog> zib_transaction_logs { get; set; }
        public DbSet<TokenLog> zib_logs { get; set; }
        public DbSet<RefreshToken> zib_refresh_tokens { get; set; }
        public DbSet<AuditTrail> zib_audit_trails { get; set; }
        //public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<ApplicationRole> ApplicationRole { get; set; }
        public DbSet<ApplicationUserRole> ApplicationUserRole { get; set; }

        //Dedails About Under Method-https://entityframework.net/knowledge-base/39798317/identityuserlogin-string---requires-a-primary-key-to-be-defined-error-while-adding-migration
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.Id).ValueGeneratedOnAdd();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.UserName).IsUnique();
                entity.Ignore(u => u.AccessFailedCount);
                entity.Ignore(u => u.LockoutEnabled);
                entity.Ignore(u => u.TwoFactorEnabled);
                entity.Ignore(u => u.ConcurrencyStamp);
                entity.Ignore(u => u.LockoutEnd);
                entity.Ignore(u => u.EmailConfirmed);
                entity.Ignore(u => u.TwoFactorEnabled);
                entity.Ignore(u => u.AccessFailedCount);
                entity.Ignore(u => u.PhoneNumberConfirmed);
                entity.Property(u => u.IsPasswordChanged).HasDefaultValue(false);
                entity.Property(u => u.IsDeleted).HasDefaultValue(false);
            });
            builder.Entity<ApplicationRole>(entity =>
            {
                entity.Property(r => r.Id).ValueGeneratedOnAdd();
                entity.HasIndex(r => r.Name).IsUnique();
            });
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
            builder.Entity<Merchant>(entity =>
            {
                entity.Property(m => m.Id).ValueGeneratedOnAdd();
                entity.Property(m => m.MerchantStatus).HasDefaultValue((MerchantStatus)1);
                entity.Property(m => m.WhoToCharge).HasDefaultValue((WhoToCharge)1);
                entity.Property(m => m.NotificationRequired).HasDefaultValue((NotificationRequired)0);
                entity.Property(m => m.ChargeRequired).HasDefaultValue((ChargeRequired)0);
                entity.HasIndex(m => m.MerchantName).IsUnique();
                entity.HasIndex(m => m.AccountNumber).IsUnique();
                entity.Property(m => m.IsDeleted).HasDefaultValue(false);
                entity.Property(m => m.CreatedDate).HasColumnType("datetime");
                entity.Property(m => m.LastUpdatedDate).HasColumnType("datetime");
            });
            builder.Entity<MerchantUser>(entity =>
            {
                entity.Property(m => m.Id).ValueGeneratedOnAdd();
                entity.HasIndex(m => new { m.MerchantId, m.UserId }).IsUnique();
                entity.Property(m => m.IsMerchantAdmin).HasDefaultValue(false);
                entity.Property(m => m.IsDeleted).HasDefaultValue(false);
                entity.Property(m => m.CreatedDate).HasColumnType("datetime");
                entity.Property(m => m.LastUpdatedDate).HasColumnType("datetime");
            });
            builder.Entity<Mandate>(entity =>
            {
                entity.Property(m => m.Id).ValueGeneratedOnAdd();
                entity.HasIndex(m => new { m.MerchantId, m.ReferenceNumber }).IsUnique();
                entity.Property(m => m.StartDate).HasColumnType("date");
                entity.Property(m => m.EndDate).HasColumnType("date");
                entity.Property(m => m.DueDate).HasColumnType("date");
                entity.Property(m => m.IsApproved).HasDefaultValue(false);
                entity.Property(m => m.IsCancelled).HasDefaultValue(false);
                entity.Property(m => m.IsDeleted).HasDefaultValue(false);
                entity.Property(m => m.CreatedDate).HasColumnType("datetime");
                entity.Property(m => m.LastUpdatedDate).HasColumnType("datetime");
              //  entity.HasOne<TransactionLog>(s => s.TransactionLog)
               // .WithOne(ad => ad.Mandate)
            });
            builder.Entity<MandateDetail>(entity =>
            {
                entity.Property(m => m.Id).ValueGeneratedOnAdd();
                entity.Property(m => m.MandateStatus).HasDefaultValue((MandateStatus)0);
                entity.Property(m => m.IsNotified).HasDefaultValue(false);
                entity.Property(m => m.StartDate).HasColumnType("date");
                entity.Property(m => m.EndDate).HasColumnType("date");
                entity.Property(m => m.DueDate).HasColumnType("date");
                entity.Property(m => m.IsDeleted).HasDefaultValue(false);
                entity.Property(m => m.CreatedDate).HasColumnType("datetime");
                entity.Property(m => m.LastUpdatedDate).HasColumnType("datetime");
            });
            builder.Entity<NotificationLog>(entity =>
            {
                entity.Property(n => n.Id).ValueGeneratedOnAdd();
                entity.Property(n => n.NotificationResponse).HasDefaultValue((NotificationResponse)1);
                entity.Property(n => n.NotificationType).HasDefaultValue((NotificationType)1);
                entity.Property(n => n.IsRead).HasDefaultValue(false);
                entity.Property(n => n.IsDeleted).HasDefaultValue(false);
                entity.Property(n => n.CreatedDate).HasColumnType("datetime");
                entity.Property(n => n.LastUpdatedDate).HasColumnType("datetime");
            });
            builder.Entity<TransactionLog>(entity =>
            {
                entity.Property(t => t.Id).ValueGeneratedOnAdd();
                entity.Property(t => t.IsDeleted).HasDefaultValue(false);
                entity.Property(t => t.CreatedDate).HasColumnType("datetime");
                entity.Property(t => t.LastUpdatedDate).HasColumnType("datetime");
            });
            builder.Entity<StaffMember>(entity =>
            {
                entity.Property(m => m.Id).ValueGeneratedOnAdd();
                entity.Property(m => m.IsAdmin).HasDefaultValue(false);
                entity.Property(m => m.IsDeleted).HasDefaultValue(false);
                entity.Property(m => m.CreatedDate).HasColumnType("datetime");
                entity.Property(m => m.LastUpdatedDate).HasColumnType("datetime");
            });
            builder.Entity<TokenLog>(entity =>
            {
                entity.Property(l => l.Id).ValueGeneratedOnAdd();
                entity.Property(l => l.IsDeleted).HasDefaultValue(false);
                entity.Property(l => l.CreatedDate).HasColumnType("datetime");
                entity.Property(l => l.LastUpdatedDate).HasColumnType("datetime");
            });
            builder.Entity<RefreshToken>(entity =>
            {
                entity.Property(t => t.Id).ValueGeneratedOnAdd();
                entity.Property(t => t.IsDeleted).HasDefaultValue(false);
                entity.Property(t => t.CreatedDate).HasColumnType("datetime");
                entity.Property(t => t.LastUpdatedDate).HasColumnType("datetime");
            });
            builder.Entity<AuditTrail>(entity =>
            {
                entity.Property(t => t.Id).ValueGeneratedOnAdd();
                entity.Property(t => t.IsDeleted).HasDefaultValue(false);
                entity.Property(t => t.CreatedDate).HasColumnType("datetime");
                entity.Property(t => t.LastUpdatedDate).HasColumnType("datetime");
            });

            foreach (var property in builder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                // EF Core 5
                property.SetPrecision(18);
                property.SetScale(2);
            }
        }
        public override int SaveChanges()
        {
            var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is AuditableBaseEntity && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

            //var currentUsername = !string.IsNullOrEmpty(HttpContext.Current?.User?.Identity?.Name) ? HttpContext.Current.User.Identity.Name : "Anonymous";

            foreach (var entityEntry in entries)
            {
                ((AuditableBaseEntity)entityEntry.Entity).LastUpdatedDate = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((AuditableBaseEntity)entityEntry.Entity).CreatedDate = DateTime.Now;
                }
            }
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is AuditableBaseEntity && (
                    e.State == EntityState.Added
                    || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                ((AuditableBaseEntity)entityEntry.Entity).LastUpdatedDate = DateTime.Now;

                if (entityEntry.State == EntityState.Added)
                {
                    ((AuditableBaseEntity)entityEntry.Entity).CreatedDate = DateTime.Now;
                }
            }
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}
