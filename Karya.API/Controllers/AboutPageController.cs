using Karya.Application.Features.AboutPage.Dto;
using Karya.Application.Features.AboutPage.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Karya.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AboutPageController(IAboutPageService service) : CustomBaseController
	{
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id) => CreateActionResult(await service.GetByIdAsync(id));

		[HttpGet]
		public async Task<IActionResult> GetAll() => CreateActionResult(await service.GetAllAsync());

		[HttpPost]
		public async Task<IActionResult> CreateAboutPage([FromBody] CreateAboutPageDto aboutPageDto) => CreateActionResult(await service.CreateAboutPageAsync(aboutPageDto));

		[HttpPut]
		public async Task<IActionResult> UpdateAboutPage([FromBody] UpdateAboutPageDto aboutPageDto) => CreateActionResult(await service.UpdateAboutPageAsync(aboutPageDto));

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAboutPage(Guid id) => CreateActionResult(await service.DeleteAboutPageAsync(id));
	}
}
