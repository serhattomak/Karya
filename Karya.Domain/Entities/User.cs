using Karya.Domain.Common;
using Karya.Domain.Enums;

namespace Karya.Domain.Entities;

public class User : BaseEntity
{
	public string Username { get; set; } = string.Empty;
	public string PasswordHash { get; set; } = string.Empty;
	public UserRoles Role { get; set; } = UserRoles.User;
}