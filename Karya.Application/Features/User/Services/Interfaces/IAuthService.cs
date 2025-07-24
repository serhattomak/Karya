using Karya.Application.Common.DTOs;
using Karya.Domain.Common;

namespace Karya.Application.Features.User.Services.Interfaces;

public interface IAuthService
{
	Task<Result> RegisterAsync(RegisterDto registerDto);
	Task<Result<AuthDto>> LoginAsync(LoginDto loginDto);
	Task<Result> LogoutAsync();
}