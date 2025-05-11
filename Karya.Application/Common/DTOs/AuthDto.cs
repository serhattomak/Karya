namespace Karya.Application.Common.DTOs;

public class AuthDto
{
	public Guid Id { get; set; }
	public string Email { get; set; } = string.Empty;
	public string Username { get; set; } = string.Empty;
	public string Token { get; set; } = string.Empty;
	public List<string>? Roles { get; set; }
}