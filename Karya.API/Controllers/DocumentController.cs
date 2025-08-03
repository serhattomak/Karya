using Karya.Application.Features.Document.Dto;
using Karya.Application.Features.Document.Services.Interfaces;
using Karya.Domain.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karya.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class DocumentController(IDocumentService service) : CustomBaseController
	{
		[HttpGet]
		public async Task<IActionResult> GetAll([FromQuery] PagedRequest request)
		{
			if (request.PageIndex <= 0) request.PageIndex = 1;
			if (request.PageSize <= 0) request.PageSize = 10;
			if (request.PageSize > 100) request.PageSize = 100;

			return CreateActionResult(await service.GetAllAsync(request));
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id) => CreateActionResult(await service.GetByIdAsync(id));

		[HttpGet("name/{name}")]
		public async Task<IActionResult> GetByName(string name) => CreateActionResult(await service.GetByNameAsync(name));

		[HttpGet("slug/{slug}")]
		public async Task<IActionResult> GetBySlug(string slug) => CreateActionResult(await service.GetBySlugAsync(slug));

		[HttpGet("category/{category}")]
		public async Task<IActionResult> GetByCategory(string category) => CreateActionResult(await service.GetByCategoryAsync(category));

		[HttpGet("active")]
		public async Task<IActionResult> GetActiveDocuments() => CreateActionResult(await service.GetActiveDocumentsAsync());

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Create(CreateDocumentDto documentDto) => CreateActionResult(await service.CreateAsync(documentDto));

		[HttpPost("upload")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Upload([FromForm] IFormFile file, [FromForm] CreateDocumentDto documentDto)
			=> CreateActionResult(await service.UploadDocumentAsync(file, documentDto));

		[HttpPut]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Update(UpdateDocumentDto documentDto) => CreateActionResult(await service.UpdateAsync(documentDto));

		[HttpPost("{id}/download")]
		public async Task<IActionResult> Download(Guid id) => CreateActionResult(await service.DownloadAsync(id));

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(Guid id) => CreateActionResult(await service.DeleteAsync(id));
	}
}