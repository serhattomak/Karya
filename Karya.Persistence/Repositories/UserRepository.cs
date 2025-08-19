using Karya.Domain.Entities;
using Karya.Domain.Interfaces;
using Karya.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Karya.Persistence.Repositories;

public class UserRepository(AppDbContext context) : EfRepository<User>(context), IUserRepository
{
	public async Task<User?> GetByUsernameAsync(string username)
	{
		return await _context.Users
			.FirstOrDefaultAsync(u => u.Username == username);
	}

	public async Task<bool> ExistsAsync(string username)
	{
		return await _context.Users
			.AnyAsync(u => u.Username == username);
	}
}