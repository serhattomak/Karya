using AutoMapper;
using Karya.Application.Features.AboutPage.Dto;
using Karya.Application.Features.AboutPage.Services.Interfaces;
using Karya.Application.Features.File.Dto;
using Karya.Domain.Common;
using Karya.Domain.Enums;
using Karya.Domain.Interfaces;

namespace Karya.Application.Features.AboutPage.Services;

public class AboutPageService(IMapper mapper, IAboutPageRepository repository, IFileRepository fileRepository) : IAboutPageService
{
	public async Task<Result<AboutPageDto>> GetByIdAsync(Guid id)
	{
		var aboutPage = await repository.GetByIdAsync(id);
		if (aboutPage == null)
			return Result<AboutPageDto>.Failure("About page not found");
		var aboutPageDto = mapper.Map<AboutPageDto>(aboutPage);
		if (aboutPage.FileIds != null && aboutPage.FileIds.Any())
		{
			var files = await fileRepository.GetByIdsAsync(aboutPage.FileIds);
			aboutPageDto.Files = mapper.Map<List<FileDto>>(files);
		}
		return Result<AboutPageDto>.Success(aboutPageDto);
	}

	public async Task<Result<List<AboutPageDto>>> GetAllAsync()
	{
		var aboutPages = await repository.GetAllAsync();
		var aboutPageDtos = mapper.Map<List<AboutPageDto>>(aboutPages);
		foreach (var aboutPage in aboutPages)
		{
			if (aboutPage.FileIds != null && aboutPage.FileIds.Any())
			{
				var files = await fileRepository.GetByIdsAsync(aboutPage.FileIds);
				var aboutPageDto = aboutPageDtos.FirstOrDefault(x => x.Id == aboutPage.Id);
				if (aboutPageDto != null)
					aboutPageDto.Files = mapper.Map<List<FileDto>>(files);
			}
		}
		return Result<List<AboutPageDto>>.Success(aboutPageDtos);
	}

	public async Task<Result<AboutPageDto>> CreateAboutPageAsync(CreateAboutPageDto aboutPageDto)
	{
		var aboutPage = mapper.Map<Domain.Entities.AboutPage>(aboutPageDto);
		await repository.AddAsync(aboutPage);
		await repository.SaveChangesAsync();
		return Result<AboutPageDto>.Success(mapper.Map<AboutPageDto>(aboutPage));
	}

	public async Task<Result<AboutPageDto>> UpdateAboutPageAsync(UpdateAboutPageDto aboutPageDto)
	{
		var aboutPage = await repository.GetByIdAsync(aboutPageDto.Id);
		if (aboutPage == null)
			return Result<AboutPageDto>.Failure("About page not found");
		mapper.Map(aboutPageDto, aboutPage);
		aboutPage.ModifiedDate = DateTime.UtcNow;
		repository.UpdateAsync(aboutPage);
		await repository.SaveChangesAsync();
		return Result<AboutPageDto>.Success(mapper.Map<AboutPageDto>(aboutPage));
	}

	public async Task<Result<AboutPageDto>> DeleteAboutPageAsync(Guid id)
	{
		var aboutPage = await repository.GetByIdAsync(id);
		if (aboutPage == null)
			return Result<AboutPageDto>.Failure("About page not found");
		aboutPage.Status = BaseStatuses.Deleted;
		aboutPage.ModifiedDate = DateTime.UtcNow;
		repository.UpdateAsync(aboutPage);
		await repository.SaveChangesAsync();
		return Result<AboutPageDto>.Success(mapper.Map<AboutPageDto>(aboutPage));
	}
}