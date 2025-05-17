using Karya.Application.Features.Contact.Dto;
using Karya.Domain.Common;

namespace Karya.Application.Features.Contact.Services.Interfaces;

public interface IContactService
{
	Task<Result<ContactDto>> GetByIdAsync(Guid id);
	Task<Result<List<ContactDto>>> GetAllAsync();
	Task<Result<ContactDto>> CreateContactAsync(CreateContactDto contactDto);
	Task<Result<ContactDto>> UpdateContactAsync(UpdateContactDto contactDto);
	Task<Result<ContactDto>> DeleteContactAsync(Guid id);
}