using Karya.Domain.Common;

namespace Karya.Domain.Entities;

public class User : BaseEntity
{
	public string Username { get; set; } = string.Empty;
	public string PasswordHash { get; set; } = string.Empty;
}