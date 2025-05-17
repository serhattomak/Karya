using AutoMapper;
using Karya.Application.Features.ContactPage.Dto;
using Karya.Application.Features.ContactPage.Services.Interfaces;
using Karya.Domain.Common;
using Karya.Domain.Enums;
using Karya.Domain.Interfaces;

namespace Karya.Application.Features.ContactPage.Services;

public class ContactPageService(IContactPageRepository repository, IMapper mapper) : IContactPageService
{
	public async Task<Result<ContactPageDto>> GetByIdAsync(Guid id)
	{
		var contactPage = await repository.GetByIdAsync(id);
		if (contactPage == null)
			return Result<ContactPageDto>.Failure("Contact page not found");
		return Result<ContactPageDto>.Success(mapper.Map<ContactPageDto>(contactPage));
	}

	public async Task<Result<List<ContactPageDto>>> GetAllAsync()
	{
		var contactPages = await repository.GetAllAsync();
		return Result<List<ContactPageDto>>.Success(mapper.Map<List<ContactPageDto>>(contactPages));
	}

	public async Task<Result<ContactPageDto>> CreateContactPageAsync(CreateContactPageDto contactPageDto)
	{
		var contactPage = mapper.Map<Domain.Entities.ContactPage>(contactPageDto);
		await repository.AddAsync(contactPage);
		await repository.SaveChangesAsync();
		return Result<ContactPageDto>.Success(mapper.Map<ContactPageDto>(contactPage));
	}

	public async Task<Result<ContactPageDto>> UpdateContactPageAsync(UpdateContactPageDto contactPageDto)
	{
		var contactPage = await repository.GetByIdAsync(contactPageDto.Id);
		if (contactPage == null)
			return Result<ContactPageDto>.Failure("Contact page not found");
		mapper.Map(contactPageDto, contactPage);
		contactPage.ModifiedDate = DateTime.UtcNow;
		repository.UpdateAsync(contactPage);
		await repository.SaveChangesAsync();
		return Result<ContactPageDto>.Success(mapper.Map<ContactPageDto>(contactPage));
	}

	public async Task<Result<ContactPageDto>> DeleteContactPageAsync(Guid id)
	{
		var contactPage = await repository.GetByIdAsync(id);
		if (contactPage == null)
			return Result<ContactPageDto>.Failure("Contact page not found");
		contactPage.Status = BaseStatuses.Deleted;
		contactPage.ModifiedDate = DateTime.UtcNow;
		repository.UpdateAsync(contactPage);
		await repository.SaveChangesAsync();
		return Result<ContactPageDto>.Success(mapper.Map<ContactPageDto>(contactPage));
	}
}