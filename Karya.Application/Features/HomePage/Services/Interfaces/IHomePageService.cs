using Karya.Application.Features.HomePage.Dto;
using Karya.Domain.Common;

namespace Karya.Application.Features.HomePage.Services.Interfaces;

public interface IHomePageService
{
	Task<Result<HomePageDto>> GetByIdAsync(Guid id);
	Task<Result<List<HomePageDto>>> GetAllAsync();
	Task<Result<HomePageDto>> CreateHomePageAsync(CreateHomePageDto homePageDto);
	Task<Result<HomePageDto>> UpdateHomePageAsync(UpdateHomePageDto homePageDto);
	Task<Result<HomePageDto>> DeleteHomePageAsync(Guid id);
}