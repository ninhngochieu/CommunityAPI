using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BackendAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Migrations.JsonSeed
{
    public static class SeedData
    {

        public static async Task Seed(CommunityContext context, UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager)
        {
            await SeedRoles(roleManager);
            await SeedUserData(userManager);
            await context.SaveChangesAsync();
        }

        private static async Task SeedRoles(RoleManager<AppRole> roleManager)
        {
            var roles = new List<AppRole>()
            {
                new() {Name = "Member"},
                new() {Name = "Admin"},
                new() {Name = "Moderator"}
            };

            async void Action(AppRole r)
            {
                await roleManager.CreateAsync(r);
            }

            roles.ForEach(Action);
        }

        private static async Task SeedUserData(UserManager<AppUser> userManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var usersJson = await File.ReadAllTextAsync("Migrations/JsonSeed/User.json");

            var users = JsonSerializer.Deserialize<List<AppUser>>(usersJson);

            Debug.Assert(users != null, nameof(users) + " != null");

            async void Action(AppUser user)
            {
                // var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();

                await userManager.CreateAsync(user, "123456");

                await userManager.AddToRoleAsync(user, "Member");

                // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("123456"));
                //
                // user.PasswordSalt = hmac.Key;

                // context.AppUsers.Add(user);
            }

            users.ForEach(Action);
            
            {//Seed Admin
                var admin = new AppUser() {UserName = "admin"};

                await userManager.CreateAsync(admin, "123456");

                await userManager.AddToRolesAsync(admin, new[] {"Admin", "Moderator", "Member"});
            }
            
            {//Seed Moderator
                var mod = new AppUser() {UserName = "mod"};

                await userManager.CreateAsync(mod, "123456");

                await userManager.AddToRolesAsync(mod, new[] {"Moderator", "Member"});
            }
        }
    }
}
