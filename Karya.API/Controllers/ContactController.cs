using Karya.Application.Features.Contact.Dto;
using Karya.Application.Features.Contact.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Karya.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ContactController(IContactService service) : CustomBaseController
	{
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id) => CreateActionResult(await service.GetByIdAsync(id));
		[HttpGet]
		public async Task<IActionResult> GetAll() => CreateActionResult(await service.GetAllAsync());
		[HttpPost]
		public async Task<IActionResult> CreateContact([FromBody] CreateContactDto contactDto) => CreateActionResult(await service.CreateContactAsync(contactDto));
		[HttpPut]
		public async Task<IActionResult> UpdateContact([FromBody] UpdateContactDto contactDto) => CreateActionResult(await service.UpdateContactAsync(contactDto));
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteContact(Guid id) => CreateActionResult(await service.DeleteContactAsync(id));
	}
}
