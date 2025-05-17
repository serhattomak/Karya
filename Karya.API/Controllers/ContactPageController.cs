using Karya.Application.Features.ContactPage.Dto;
using Karya.Application.Features.ContactPage.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Karya.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ContactPageController(IContactPageService service) : CustomBaseController
	{
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id) => CreateActionResult(await service.GetByIdAsync(id));
		[HttpGet]
		public async Task<IActionResult> GetAll() => CreateActionResult(await service.GetAllAsync());
		[HttpPost]
		public async Task<IActionResult> CreateContactPage([FromBody] CreateContactPageDto contactPageDto) => CreateActionResult(await service.CreateContactPageAsync(contactPageDto));
		[HttpPut]
		public async Task<IActionResult> UpdateContactPage([FromBody] UpdateContactPageDto contactPageDto) => CreateActionResult(await service.UpdateContactPageAsync(contactPageDto));
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteContactPage(Guid id) => CreateActionResult(await service.DeleteContactPageAsync(id));
	}
}
