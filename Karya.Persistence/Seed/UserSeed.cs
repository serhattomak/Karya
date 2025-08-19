using Karya.Domain.Entities;
using Karya.Domain.Enums;
using Karya.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Karya.Persistence.Seed;

public static class UserSeed
{
	public static async Task SeedAsync(AppDbContext context)
	{
		if (!await context.Users.AnyAsync(u => u.Role == UserRoles.Admin))
		{
			var adminUser = new User
			{
				Username = "admin",
				PasswordHash = HashPassword("admin123"),
				Role = UserRoles.Admin
			};

			await context.Users.AddAsync(adminUser);
			await context.SaveChangesAsync();
		}
	}

	private static string HashPassword(string password)
	{
		using var sha256 = SHA256.Create();
		var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
		return Convert.ToBase64String(hashedBytes);
	}
}