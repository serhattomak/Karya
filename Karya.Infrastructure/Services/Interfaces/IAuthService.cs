using Karya.Application.Common.DTOs;
using Karya.Domain.Common;

namespace Karya.Infrastructure.Services.Interfaces;

public interface IAuthService
{
	Task<Result> RegisterAsync(RegisterDto registerDto);
	Task<Result<AuthDto>> LoginAsync(LoginDto request);
	Task<Result> LogoutAsync();
}