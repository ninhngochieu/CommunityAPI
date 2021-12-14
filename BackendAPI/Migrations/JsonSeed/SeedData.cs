using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BackendAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Migrations.JsonSeed
{
    public static class SeedData
    {

        public static async Task SeedUsers(CommunityContext context)
        {
            if (await context.AppUsers.AnyAsync()) return;

            var usersJson = await File.ReadAllTextAsync("Migrations/JsonSeed/User.json");

            var users = JsonSerializer.Deserialize<List<AppUser>>(usersJson);

            Debug.Assert(users != null, nameof(users) + " != null");
            
            users.ForEach(user =>
            {
                var hmac = new HMACSHA512();
                user.UserName = user.UserName.ToLower();

                // user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("123456"));
                //
                // user.PasswordSalt = hmac.Key;

                context.AppUsers.Add(user);
            });

            await context.SaveChangesAsync();
        }

    }
}
