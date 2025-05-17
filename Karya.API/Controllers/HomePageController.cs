using Karya.Application.Features.HomePage.Dto;
using Karya.Application.Features.HomePage.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Karya.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class HomePageController(IHomePageService service) : CustomBaseController
	{
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id) => CreateActionResult(await service.GetByIdAsync(id));
		[HttpGet]
		public async Task<IActionResult> GetAll() => CreateActionResult(await service.GetAllAsync());
		[HttpPost]
		public async Task<IActionResult> CreateHomePage([FromBody] CreateHomePageDto homePageDto) => CreateActionResult(await service.CreateHomePageAsync(homePageDto));
		[HttpPut]
		public async Task<IActionResult> UpdateHomePage([FromBody] UpdateHomePageDto homePageDto) => CreateActionResult(await service.UpdateHomePageAsync(homePageDto));
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteHomePage(Guid id) => CreateActionResult(await service.DeleteHomePageAsync(id));
	}
}
