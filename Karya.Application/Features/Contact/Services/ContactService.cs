using AutoMapper;
using Karya.Application.Features.Contact.Dto;
using Karya.Application.Features.Contact.Services.Interfaces;
using Karya.Domain.Common;
using Karya.Domain.Enums;
using Karya.Domain.Interfaces;

namespace Karya.Application.Features.Contact.Services;

public class ContactService(IContactRepository repository, IMapper mapper) : IContactService
{
	public async Task<Result<ContactDto>> GetByIdAsync(Guid id)
	{
		var contact = await repository.GetByIdAsync(id);
		if (contact == null)
			return Result<ContactDto>.Failure("Contact not found");
		return Result<ContactDto>.Success(mapper.Map<ContactDto>(contact));
	}

	public async Task<Result<List<ContactDto>>> GetAllAsync()
	{
		var contacts = await repository.GetAllAsync();
		return Result<List<ContactDto>>.Success(mapper.Map<List<ContactDto>>(contacts));
	}

	public async Task<Result<ContactDto>> CreateContactAsync(CreateContactDto contactDto)
	{
		var contact = mapper.Map<Domain.Entities.Contact>(contactDto);
		await repository.AddAsync(contact);
		await repository.SaveChangesAsync();
		return Result<ContactDto>.Success(mapper.Map<ContactDto>(contact));
	}

	public async Task<Result<ContactDto>> UpdateContactAsync(UpdateContactDto contactDto)
	{
		var contact = await repository.GetByIdAsync(contactDto.Id);
		if (contact == null)
			return Result<ContactDto>.Failure("Contact not found");
		mapper.Map(contactDto, contact);
		contact.ModifiedDate = DateTime.UtcNow;
		repository.UpdateAsync(contact);
		await repository.SaveChangesAsync();
		return Result<ContactDto>.Success(mapper.Map<ContactDto>(contact));
	}

	public async Task<Result<ContactDto>> DeleteContactAsync(Guid id)
	{
		var contact = await repository.GetByIdAsync(id);
		if (contact == null)
			return Result<ContactDto>.Failure("Contact not found");
		contact.Status = BaseStatuses.Deleted;
		contact.ModifiedDate = DateTime.UtcNow;
		repository.UpdateAsync(contact);
		await repository.SaveChangesAsync();
		return Result<ContactDto>.Success(mapper.Map<ContactDto>(contact));
	}
}