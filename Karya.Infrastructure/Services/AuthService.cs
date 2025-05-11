using AutoMapper;
using Karya.Application.Common.DTOs;
using Karya.Domain.Common;
using Karya.Infrastructure.Entities;
using Karya.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Karya.Infrastructure.Services;

public class AuthService(
	IMapper mapper,
	UserManager<AppUser> userManager,
	RoleManager<AppRole> roleManager,
	SignInManager<AppUser> signInManager,
	IConfiguration configuration) : IAuthService
{
	public async Task<Result> RegisterAsync(RegisterDto registerDto)
	{
		var user = mapper.Map<AppUser>(registerDto);
		var result = await userManager.CreateAsync(user, registerDto.Password);
		if (!result.Succeeded) return Result.Failure(result.Errors.Select(e => e.Description).ToList());
		await roleManager.CreateAsync(new AppRole { Name = "User" });
		await userManager.AddToRoleAsync(user, "User");
		return Result.Success();
	}

	public async Task<Result<AuthDto>> LoginAsync(LoginDto request)
	{
		var user = await userManager.FindByEmailAsync(request.Email);
		if (user == null) return Result<AuthDto>.Failure("User not found");
		var result = await signInManager.PasswordSignInAsync(user, request.Password, false, false);
		if (!result.Succeeded) return Result<AuthDto>.Failure("Invalid credentials");
		var response = new AuthDto
		{
			Token = CreateToken(user, await userManager.GetRolesAsync(user)),
			Email = user.Email,
			Username = user.UserName,
			Id = user.Id,
			Roles = (await userManager.GetRolesAsync(user)).ToList()
		};
		return Result<AuthDto>.Success(response);
	}

	public async Task<Result> LogoutAsync()
	{
		await signInManager.SignOutAsync();
		return Result.Success();
	}

	private string CreateToken(AppUser user, IList<string> roles)
	{
		var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Email, user.Email),
			new Claim(ClaimTypes.Name, user.UserName),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
		};

		foreach (var role in roles)
		{
			claims.Add(new Claim(ClaimTypes.Role, role));
		}

		var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
		var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var token = new JwtSecurityToken(
			issuer: configuration["Jwt:Issuer"],
			audience: configuration["Jwt:Audience"],
			expires: DateTime.UtcNow.AddHours(1),
			claims: claims,
			signingCredentials: creds);
		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}