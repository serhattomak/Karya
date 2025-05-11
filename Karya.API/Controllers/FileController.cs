using Karya.Application.Features.File.Dto;
using Karya.Application.Features.File.Services.Interfaces;
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
		public async Task<IActionResult> Upload([FromForm] IFormFile file) => CreateActionResult(await service.SaveFileAsync(file));

		[HttpPost]
		public async Task<IActionResult> Create(CreateFileDto fileDto) => CreateActionResult(await service.CreateAsync(fileDto));

		[HttpPut]
		public async Task<IActionResult> Update(UpdateFileDto fileDto) => CreateActionResult(await service.UpdateAsync(fileDto));

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id) => CreateActionResult(await service.DeleteAsync(id));
	}
}
