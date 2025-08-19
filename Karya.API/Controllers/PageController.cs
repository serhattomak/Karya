using Karya.Application.Features.Page.Dto;
using Karya.Application.Features.Page.Services.Interfaces;
using Karya.Domain.Common;
using Karya.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karya.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PageController(IPageService service) : CustomBaseController
	{
		[HttpGet("{id}")]
		public async Task<IActionResult> GetPageById(Guid id) => CreateActionResult(await service.GetPageByIdAsync(id));

		[HttpGet("name/{name}")]
		public async Task<IActionResult> GetPageByName(string name) => CreateActionResult(await service.GetPageByNameAsync(name));

		[HttpGet("slug/{slug}")]
		public async Task<IActionResult> GetPageBySlug(string slug) => CreateActionResult(await service.GetPageBySlugAsync(slug));

		[HttpGet]
		public async Task<IActionResult> GetAllPages([FromQuery] PagedRequest request)
			=> CreateActionResult(await service.GetAllPagesAsync(request));

		[HttpGet("type/{type}")]
		public async Task<IActionResult> GetAllPagesByType(PageTypes type, [FromQuery] PagedRequest request)
			=> CreateActionResult(await service.GetAllPagesByTypeAsync(type, request));

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> CreatePage([FromBody] CreatePageDto pageDto) => CreateActionResult(await service.CreatePageAsync(pageDto));

		[HttpPut]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdatePage([FromBody] UpdatePageDto pageDto) => CreateActionResult(await service.UpdatePageAsync(pageDto));

		[HttpPut("product-order")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> UpdatePageProductOrder([FromBody] UpdatePageProductOrderDto updateDto) => CreateActionResult(await service.UpdatePageProductOrderAsync(updateDto));

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> DeletePage(Guid id) => CreateActionResult(await service.DeletePageAsync(id));
	}
}