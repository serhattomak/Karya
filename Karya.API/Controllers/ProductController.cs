using Karya.Application.Features.Product.Dto;
using Karya.Application.Features.Product.Services.Interfaces;
using Karya.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace Karya.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductController(IProductService service) : CustomBaseController
	{
		[HttpGet]
		public async Task<IActionResult> GetAll([FromQuery] PagedRequest request)
			=> CreateActionResult(await service.GetAllAsync(request));

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id) => CreateActionResult(await service.GetByIdAsync(id));

		[HttpPost]
		public async Task<IActionResult> Create(CreateProductDto productDto) => CreateActionResult(await service.CreateAsync(productDto));

		[HttpPut]
		public async Task<IActionResult> Update(UpdateProductDto productDto) => CreateActionResult(await service.UpdateAsync(productDto));

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id) => CreateActionResult(await service.DeleteAsync(id));
	}
}
