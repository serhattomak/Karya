using Karya.Application.Features.ContactPage.Dto;
using Karya.Domain.Common;

namespace Karya.Application.Features.ContactPage.Services.Interfaces;

public interface IContactPageService
{
	Task<Result<ContactPageDto>> GetByIdAsync(Guid id);
	Task<Result<List<ContactPageDto>>> GetAllAsync();
	Task<Result<ContactPageDto>> CreateContactPageAsync(CreateContactPageDto contactPageDto);
	Task<Result<ContactPageDto>> UpdateContactPageAsync(UpdateContactPageDto contactPageDto);
	Task<Result<ContactPageDto>> DeleteContactPageAsync(Guid id);
}