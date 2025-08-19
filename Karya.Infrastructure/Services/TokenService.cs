using Karya.Application.Features.User.Services.Interfaces;
using Karya.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Karya.Infrastructure.Services;

public class TokenService(IConfiguration configuration) : ITokenService
{
	public string GenerateToken(User user)
	{
		var claims = new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, user.Id.ToString()),
			new(ClaimTypes.Name, user.Username),
			new(ClaimTypes.Role, user.Role.ToString()),
			new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
		};

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: configuration["Jwt:Issuer"],
			audience: configuration["Jwt:Audience"],
			claims: claims,
			expires: DateTime.UtcNow.AddHours(1),
			signingCredentials: credentials
		);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}

	public bool ValidateToken(string token)
	{
		try
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);

			tokenHandler.ValidateToken(token, new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateIssuer = true,
				ValidIssuer = configuration["Jwt:Issuer"],
				ValidateAudience = true,
				ValidAudience = configuration["Jwt:Audience"],
				ValidateLifetime = true,
				ClockSkew = TimeSpan.Zero
			}, out SecurityToken validatedToken);

			return true;
		}
		catch
		{
			return false;
		}
	}

	public Guid? GetUserIdFromToken(string token)
	{
		try
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var jsonToken = tokenHandler.ReadJwtToken(token);

			var userIdClaim = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

			return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId)
				? userId
				: null;
		}
		catch
		{
			return null;
		}
	}
}