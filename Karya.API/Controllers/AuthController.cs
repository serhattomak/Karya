using Karya.Application.Common.DTOs;
using Karya.Application.Features.User.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Karya.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController(IAuthService service) : CustomBaseController
	{
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto request) =>
			CreateActionResult(await service.RegisterAsync(request));

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto request) =>
			CreateActionResult(await service.LoginAsync(request));

		[HttpPost("logout")]
		public async Task<IActionResult> Logout() =>
			CreateActionResult(await service.LogoutAsync());
	}
}