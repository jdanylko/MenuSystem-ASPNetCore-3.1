using System.Collections.Generic;
using MenuSystemWithIdentity_Core.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MenuSystemWithIdentity_Core.Identity
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<MenuPermission> MenuPermissions { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<ApplicationRoleMenu> RoleMenus { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Additional tables.

            builder.Entity<MenuItem>(item =>
            {
                item.ToTable("AspNetMenu");
                item.HasMany(y => y.Children)
                    .WithOne(r => r.ParentItem)
                    .HasForeignKey(u => u.ParentId);

                item.HasMany(t => t.RoleMenus)
                    .WithOne(u => u.MenuItem)
                    .HasForeignKey(r => r.MenuId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<ApplicationRoleMenu>(roleMenu =>
            {
                roleMenu.ToTable("AspNetRoleMenu");
                
                roleMenu.HasOne(o => o.Role)
                    .WithMany(u => u.RoleMenus)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.NoAction);
                
                roleMenu.HasOne(o => o.MenuItem)
                    .WithMany(u => u.RoleMenus)
                    .HasForeignKey(e => e.MenuId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<MenuPermission>(mp =>
            {
                mp.ToTable("MenuPermission");
                
                mp.HasKey(l => new { l.RoleMenuId, l.PermissionId });

                mp.HasOne(o => o.Permission)
                    .WithMany(i => i.MenuPermissions)
                    .IsRequired();
                
                mp.HasOne(o => o.RoleMenu)
                    .WithMany(i => i.Permissions)
                    .IsRequired();
            });

            builder.Entity<Permission>(mp =>
            {
                mp.ToTable("Permission");
                
                mp.HasKey(l => l.Id);

                mp.HasMany(o => o.MenuPermissions)
                    .WithOne(i => i.Permission)
                    .HasForeignKey(y=> y.PermissionId);
            });

        }
    }
}