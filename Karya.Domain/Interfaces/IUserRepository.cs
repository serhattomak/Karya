using Karya.Domain.Entities;

namespace Karya.Domain.Interfaces;

public interface IUserRepository : IRepository<User>
{
	Task<User?> GetByUsernameAsync(string username);
	Task<bool> ExistsAsync(string username);
}