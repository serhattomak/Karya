using Karya.Application.Features.File.Dto;
using Karya.Application.Features.File.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Karya.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FileController(IFileService service) : CustomBaseController
	{
		[HttpGet]
		public async Task<IActionResult> GetAll() => CreateActionResult(await service.GetAllAsync());

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id) => CreateActionResult(await service.GetByIdAsync(id));

		[HttpPost("upload")]
		[Authorize(Roles = "Admin")]
		[RequestSizeLimit(100_000_000)]
		[RequestFormLimits(MultipartBodyLengthLimit = 100_000_000)]
		public async Task<IActionResult> Upload([FromForm] IFormFile file) => CreateActionResult(await service.SaveFileAsync(file));

		[HttpPost("upload-multiple")]
		[Authorize(Roles = "Admin")]
		[RequestSizeLimit(100_000_000)]
		[RequestFormLimits(MultipartBodyLengthLimit = 100_000_000)]
		public async Task<IActionResult> UploadMultiple([FromForm] List<IFormFile> files) => CreateActionResult(await service.SaveFilesAsync(files));

		[HttpPost]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Create(CreateFileDto fileDto) => CreateActionResult(await service.CreateAsync(fileDto));

		[HttpPut]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Update(UpdateFileDto fileDto) => CreateActionResult(await service.UpdateAsync(fileDto));

		[HttpDelete("{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(Guid id) => CreateActionResult(await service.DeleteAsync(id));
	}
}