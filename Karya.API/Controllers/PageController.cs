using Karya.Application.Features.Page.Dto;
using Karya.Application.Features.Page.Services.Interfaces;
using Karya.Domain.Common;
using Karya.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Karya.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PageController(IPageService service) : CustomBaseController
	{
		[HttpGet("{id}")]
		public async Task<IActionResult> GetPageById(Guid id) => CreateActionResult(await service.GetPageByIdAsync(id));

		[HttpGet]
		public async Task<IActionResult> GetAllPages([FromQuery] PagedRequest request)
			=> CreateActionResult(await service.GetAllPagesAsync(request));

		[HttpGet("type/{type}")]
		public async Task<IActionResult> GetAllPagesByType(PageTypes type, [FromQuery] PagedRequest request)
			=> CreateActionResult(await service.GetAllPagesByTypeAsync(type, request));

		[HttpPost]
		public async Task<IActionResult> CreatePage([FromBody] CreatePageDto pageDto) => CreateActionResult(await service.CreatePageAsync(pageDto));

		[HttpPut]
		public async Task<IActionResult> UpdatePage([FromBody] UpdatePageDto pageDto) => CreateActionResult(await service.UpdatePageAsync(pageDto));

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePage(Guid id) => CreateActionResult(await service.DeletePageAsync(id));
	}
}
