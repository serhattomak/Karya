using Karya.Application.Features.AboutPage.Dto;
using Karya.Domain.Common;

namespace Karya.Application.Features.AboutPage.Services.Interfaces;

public interface IAboutPageService
{
	Task<Result<AboutPageDto>> GetByIdAsync(Guid id);
	Task<Result<List<AboutPageDto>>> GetAllAsync();
	Task<Result<AboutPageDto>> CreateAboutPageAsync(CreateAboutPageDto aboutPageDto);
	Task<Result<AboutPageDto>> UpdateAboutPageAsync(UpdateAboutPageDto aboutPageDto);
	Task<Result<AboutPageDto>> DeleteAboutPageAsync(Guid id);
}