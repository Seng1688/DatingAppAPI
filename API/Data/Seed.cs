using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class Seed
    {
        public async static Task SeedUsers(DataContext context)
        {
            if (await context.Users.AnyAsync()) return;

            var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true};

            // Turn json format to c# object
            var users = JsonSerializer.Deserialize<List<AppUser>>(userData, options);

            // modify particular data in user object before save to database
            foreach (var user in users)
            {
                using var hmac = new HMACSHA512();
                
                user.UserName = user.UserName.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("Pa$$w0rd"));
                user.PasswordSalt = hmac.Key;

                context.Users.Add(user);

            }
            await context.SaveChangesAsync();
        }
    }
}