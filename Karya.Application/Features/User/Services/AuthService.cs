using Karya.Application.Common.DTOs;
using Karya.Application.Features.User.Services.Interfaces;
using Karya.Domain.Common;
using Karya.Domain.Enums;
using Karya.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Karya.Application.Features.User.Services;

public class AuthService(IUserRepository userRepository, ITokenService tokenService) : IAuthService
{
	public async Task<Result> RegisterAsync(RegisterDto registerDto)
	{
		try
		{
			// Password validation
			if (string.IsNullOrEmpty(registerDto.Password) || registerDto.Password.Length < 6)
			{
				return Result.Failure("Password must be at least 6 characters long.");
			}

			// Username validation
			if (string.IsNullOrEmpty(registerDto.Username) || registerDto.Username.Length < 3)
			{
				return Result.Failure("Username must be at least 3 characters long.");
			}

			// Check if username already exists
			if (await userRepository.ExistsAsync(registerDto.Username))
			{
				return Result.Failure("Username already exists.");
			}

			// Hash the password
			string passwordHash = HashPassword(registerDto.Password);

			// Create new user
			var user = new Domain.Entities.User
			{
				Username = registerDto.Username,
				PasswordHash = passwordHash,
				Role = UserRoles.User // Default role
			};

			// Save user to database
			await userRepository.AddAsync(user);
			await userRepository.SaveChangesAsync();

			return Result.Success();
		}
		catch (Exception ex)
		{
			return Result.Failure($"Registration failed: {ex.Message}");
		}
	}

	public async Task<Result<AuthDto>> LoginAsync(LoginDto loginDto)
	{
		try
		{
			// Input validation
			if (string.IsNullOrEmpty(loginDto.Username) || string.IsNullOrEmpty(loginDto.Password))
			{
				return Result<AuthDto>.Failure("Username and password are required.");
			}

			// Get user from database
			var user = await userRepository.GetByUsernameAsync(loginDto.Username);

			if (user == null)
			{
				return Result<AuthDto>.Failure("Invalid username or password.");
			}

			// Verify password
			if (!VerifyPassword(loginDto.Password, user.PasswordHash))
			{
				return Result<AuthDto>.Failure("Invalid username or password.");
			}

			// Generate token
			var token = tokenService.GenerateToken(user);

			// Create auth response
			var authDto = new AuthDto
			{
				UserId = user.Id,
				Username = user.Username,
				Token = token,
				Role = user.Role
			};

			return Result<AuthDto>.Success(authDto);
		}
		catch (Exception ex)
		{
			return Result<AuthDto>.Failure($"Login failed: {ex.Message}");
		}
	}

	public Task<Result> LogoutAsync()
	{
		try
		{
			// In a JWT-based system, logout is typically handled client-side
			// by simply discarding the token. However, you could implement
			// token blacklisting here if needed.

			return Task.FromResult(Result.Success());
		}
		catch (Exception ex)
		{
			return Task.FromResult(Result.Failure($"Logout failed: {ex.Message}"));
		}
	}

	private static string HashPassword(string password)
	{
		using var sha256 = SHA256.Create();
		var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
		return Convert.ToBase64String(hashedBytes);
	}

	private static bool VerifyPassword(string password, string passwordHash)
	{
		var hashedInput = HashPassword(password);
		return hashedInput == passwordHash;
	}
}