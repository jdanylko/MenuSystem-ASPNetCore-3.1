using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MenuSystemWithIdentity_Core.Identity;
using MenuSystemWithIdentity_Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MenuSystemWithIdentity_Core.Middleware
{
    public class SeedData: IMiddleware
    {
        private readonly ApplicationUserManager _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        public SeedData(ApplicationUserManager manager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext dbContext)
        {
            _userManager = manager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var adminRoleId = "79AC1299-E87C-446C-AD69-ECDFE1A5EEA1";
            var devRoleId = "B6275249-B4F1-4738-963D-7C40B23AF8B7";
            var userRoleId = "7AAFEC08-0A31-4588-A290-62B12F888920";

            if (await _dbContext.Permissions.CountAsync() == 0)
            {
                await _dbContext.Permissions.AddRangeAsync(new List<Permission>
                {
                    new Permission {Name = "View"},
                    new Permission {Name = "Create"},
                    new Permission {Name = "Update"},
                    new Permission {Name = "Delete"},
                    new Permission {Name = "Upload"},
                    new Permission {Name = "Publish"}
                });
                await _dbContext.SaveChangesAsync();
            }

            if (await _dbContext.MenuItems.CountAsync() == 0)
            {
                var setupItem = await _dbContext.MenuItems.AddAsync(new MenuItem
                    {Title = "Setup", ParentId = null, Icon = "wrench", Url = null});
                await _dbContext.SaveChangesAsync();

                await _dbContext.MenuItems.AddRangeAsync(new List<MenuItem>
                {
                    new MenuItem {Title = "Users", ParentId = setupItem.Entity.Id, Icon = "user", Url = "/Setup/Users"},
                    new MenuItem {Title = "Security", ParentId = setupItem.Entity.Id, Icon = "lock", Url = "/Setup/Security"},
                    new MenuItem {Title = "Menu Management", ParentId = setupItem.Entity.Id, Icon = "menu", Url = "/Setup/Menu"}
                });
                await _dbContext.SaveChangesAsync();
            }
            
            if (await _roleManager.Roles.CountAsync() == 0)
            {
                await _roleManager.CreateAsync(new ApplicationRole { Id = adminRoleId, Name = "Administrator" });
                await _roleManager.CreateAsync(new ApplicationRole { Id = userRoleId, Name = "User" });
            }

            if (await _dbContext.RoleMenus.CountAsync() == 0)
            {
                await _dbContext.RoleMenus.AddRangeAsync(new List<ApplicationRoleMenu>
                {
                    // Admin
                    new ApplicationRoleMenu {RoleId=adminRoleId, MenuId=1},
                    new ApplicationRoleMenu {RoleId=adminRoleId, MenuId=2},
                    new ApplicationRoleMenu {RoleId=adminRoleId, MenuId=3},
                    new ApplicationRoleMenu {RoleId=adminRoleId, MenuId=4},
                    // User
                    new ApplicationRoleMenu {RoleId=userRoleId, MenuId=1},
                    new ApplicationRoleMenu {RoleId=userRoleId, MenuId=2},

                });
                await _dbContext.SaveChangesAsync();
            }


            var samId = "C316633F-7DE3-48BE-A117-807AF8F7BE70";
            var ralphId = "102EBBD8-EFDA-462C-B7C2-1F5D88C80456";

            // Create Sam
            var sam = await _userManager.FindByEmailAsync("sam@gmail.com");
            if (sam == null)
            {
                sam = new ApplicationUser
                {
                    Id = samId,
                    Email = "sam@gmail.com",
                    UserName = "sam@gmail.com",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    NormalizedUserName = "SAM",
                    NormalizedEmail="SAM@GMAIL.COM",
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                await _userManager.CreateAsync(sam, "Password123!");
                await _userManager.AddToRoleAsync(sam, "Administrator");
            }

            // Create Ralph
            var ralph = await _userManager.FindByEmailAsync("ralph@gmail.com");
            if (ralph == null)
            {
                ralph = new ApplicationUser
                {
                    Id = ralphId,
                    Email = "ralph@gmail.com",
                    UserName = "ralph@gmail.com",
                    EmailConfirmed = true,
                    NormalizedUserName = "RALPH",
                    NormalizedEmail = "RALPH@GMAIL.COM",
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                await _userManager.CreateAsync(ralph, "Password456!");
                await _userManager.AddToRoleAsync(ralph, "User");
            }

            // If you want this by user, add the claims for each user.
            var samClaims = await _userManager.GetClaimsAsync(sam);
            if (samClaims.Count(y=> y.Value =="View") == 0)
            {
                await _userManager.AddClaimsAsync(sam, new List<Claim>
                {
                    new Claim(1.ToString(), "View"),
                    new Claim(2.ToString(), "View"),
                    new Claim(3.ToString(), "View"),
                    new Claim(4.ToString(), "View")
                });
            }

            var ralphClaims = await _userManager.GetClaimsAsync(ralph);
            if (ralphClaims.Count(y => y.Value == "View") == 0)
            {
                await _userManager.AddClaimsAsync(ralph, new List<Claim>
                {
                    new Claim(1.ToString(), "View"),
                    new Claim(2.ToString(), "View")
                });

            }

            // Call the next delegate/middleware in the pipeline
            await next(context);
        }
    }
}