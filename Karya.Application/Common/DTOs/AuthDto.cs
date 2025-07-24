using Karya.Domain.Enums;

namespace Karya.Application.Common.DTOs;

public class AuthDto
{
	public Guid UserId { get; set; }
	public string Username { get; set; } = string.Empty;
	public string Token { get; set; } = string.Empty;
	public UserRoles Role { get; set; }
}