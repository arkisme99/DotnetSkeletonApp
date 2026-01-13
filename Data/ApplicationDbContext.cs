using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using DotnetSkeletonApp.Models;
using DotnetSkeletonApp.Models.Interface;
using DotnetSkeletonApp.Models.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DotnetSkeletonApp.Data
{
    public class ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IHttpContextAccessor? _httpContextAccessor
        )
        : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
    {
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationRolePermission> RolePermissions { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }

        public DbSet<Notification> Notifications { get; set; }


        public override Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
        {
            LogChanges();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void LogChanges()
        {

            var user = _httpContextAccessor?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";

            var entries = ChangeTracker
                .Entries()
                .Where(e => (e.Entity is AuditableEntity || e.Entity is IAuditableEntity) &&
                            (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted))
                .ToList();

            var logs = new List<ActivityLog>();

            foreach (var entry in entries)
            {
                //Skip ActivityLog
                if (entry.Entity is ActivityLog) continue;

                var changes = entry.State switch
                {
                    EntityState.Added => entry.CurrentValues.ToObject() ?? "data",
                    EntityState.Modified => new { Original = entry.GetDatabaseValuesAsync().Result!.ToObject(), Current = entry.CurrentValues.ToObject() },
                    EntityState.Deleted => entry.CurrentValues.ToObject(),
                    _ => null
                };
                var stringAction = entry.State.ToString();
                if (entry.State == EntityState.Added)
                {
                    entry.Property(nameof(AuditableEntity.CreatedAt)).CurrentValue = DateTime.Now;
                    entry.Property(nameof(AuditableEntity.UpdatedAt)).CurrentValue = DateTime.Now;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property(nameof(AuditableEntity.CreatedAt)).IsModified = false;
                    entry.Property(nameof(AuditableEntity.UpdatedAt)).CurrentValue = DateTime.Now;
                }
                else if (entry.State == EntityState.Deleted)
                {
                    stringAction = "SoftDeleted";
                    entry.State = EntityState.Modified;

                    entry.Property(nameof(AuditableEntity.IsDeleted)).CurrentValue = true;
                    entry.Property(nameof(AuditableEntity.DeletedAt)).CurrentValue = DateTime.Now;
                }


                logs.Add(new ActivityLog
                {
                    EntityName = entry.Entity.GetType().Name,
                    Action = stringAction,
                    ChangedBy = user,
                    EntityId = GetPrimaryKey(entry),
                    Changes = JsonSerializer.Serialize(changes),
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });

            }

            // Tambahkan setelah looping selesai → aman
            if (logs.Count != 0)
            {
                ActivityLogs.AddRange(logs);
            }
        }


        private static string GetPrimaryKey(EntityEntry entry)
        {
            // ArgumentNullException.ThrowIfNull(entry);
            var key = entry.Properties.FirstOrDefault(p => p.Metadata.IsPrimaryKey());
            return key?.CurrentValue?.ToString() ?? "";
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            /* builder.Entity<ApplicationUser>(user =>
            {
                user.HasMany(e => e.UserRoles)
                    .WithOne()
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            builder.Entity<IdentityUserRole<string>>(userRole =>
                {
                    userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                    userRole.HasOne<ApplicationRole>() // Map ke class Role Anda
                        .WithMany()
                        .HasForeignKey(ur => ur.RoleId);
                }); */

            builder.Entity<ApplicationRolePermission>()
                .HasKey(rp => new { rp.RoleId, rp.PermissionId });

            builder.Entity<ApplicationRolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany() // atau .WithMany(r => r.RolePermissions) kalau kamu tambahkan koleksi di ApplicationRole
                .HasForeignKey(rp => rp.RoleId);

            builder.Entity<ApplicationRolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId);

            //dinamis sotf deleted model
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                // cek apakah entity punya properti "IsDeleted"
                var prop = entityType.ClrType.GetProperty("IsDeleted");
                if (prop != null && prop.PropertyType == typeof(bool))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var property = Expression.Property(parameter, "IsDeleted");
                    var filter = Expression.Lambda(
                        Expression.Equal(property, Expression.Constant(false)),
                        parameter
                    );

                    builder.Entity(entityType.ClrType).HasQueryFilter(filter);
                }
            }
        }
    }
}