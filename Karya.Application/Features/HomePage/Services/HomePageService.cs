using AutoMapper;
using Karya.Application.Features.File.Dto;
using Karya.Application.Features.HomePage.Dto;
using Karya.Application.Features.HomePage.Services.Interfaces;
using Karya.Application.Features.Product.Dto;
using Karya.Domain.Common;
using Karya.Domain.Enums;
using Karya.Domain.Interfaces;

namespace Karya.Application.Features.HomePage.Services;

public class HomePageService(IHomePageRepository repository, IProductRepository productRepository, IFileRepository fileRepository, IMapper mapper) : IHomePageService
{
	public async Task<Result<HomePageDto>> GetByIdAsync(Guid id)
	{
		var homePage = await repository.GetByIdAsync(id);
		if (homePage == null)
			return Result<HomePageDto>.Failure("Home page not found");

		var homePageDto = mapper.Map<HomePageDto>(homePage);

		if (homePage.ProductIds != null && homePage.ProductIds.Any())
		{
			var products = await productRepository.GetByIdsAsync(homePage.ProductIds);
			var productDtos = mapper.Map<List<ProductDto>>(products);

			foreach (var productDto in productDtos)
			{
				var product = products.First(p => p.Id == productDto.Id);
				if (product.FileIds != null && product.FileIds.Any())
				{
					var files = await fileRepository.GetByIdsAsync(product.FileIds);
					productDto.Files = mapper.Map<List<FileDto>>(files);
				}
			}

			homePageDto.Products = productDtos;
		}

		if (homePage.FileIds != null && homePage.FileIds.Any())
		{
			var files = await fileRepository.GetByIdsAsync(homePage.FileIds);
			homePageDto.Files = mapper.Map<List<FileDto>>(files);
		}

		return Result<HomePageDto>.Success(homePageDto);
	}


	public async Task<Result<List<HomePageDto>>> GetAllAsync()
	{
		var homePages = await repository.GetAllAsync();
		var homePageDtos = mapper.Map<List<HomePageDto>>(homePages);

		foreach (var homePageDto in homePageDtos)
		{
			var homePage = homePages.First(h => h.Id == homePageDto.Id);

			if (homePage.ProductIds != null && homePage.ProductIds.Any())
			{
				var products = await productRepository.GetByIdsAsync(homePage.ProductIds);
				var productDtos = mapper.Map<List<ProductDto>>(products);

				foreach (var productDto in productDtos)
				{
					var product = products.First(p => p.Id == productDto.Id);
					if (product.FileIds != null && product.FileIds.Any())
					{
						var files = await fileRepository.GetByIdsAsync(product.FileIds);
						productDto.Files = mapper.Map<List<FileDto>>(files);
					}
				}

				homePageDto.Products = productDtos;
			}

			if (homePage.FileIds != null && homePage.FileIds.Any())
			{
				var files = await fileRepository.GetByIdsAsync(homePage.FileIds);
				homePageDto.Files = mapper.Map<List<FileDto>>(files);
			}
		}

		return Result<List<HomePageDto>>.Success(homePageDtos);
	}


	public async Task<Result<HomePageDto>> CreateHomePageAsync(CreateHomePageDto homePageDto)
	{
		var homePage = mapper.Map<Domain.Entities.HomePage>(homePageDto);
		await repository.AddAsync(homePage);
		await repository.SaveChangesAsync();
		return Result<HomePageDto>.Success(mapper.Map<HomePageDto>(homePage));
	}

	public async Task<Result<HomePageDto>> UpdateHomePageAsync(UpdateHomePageDto homePageDto)
	{
		var homePage = await repository.GetByIdAsync(homePageDto.Id);
		if (homePage == null)
			return Result<HomePageDto>.Failure("Home page not found");
		mapper.Map(homePageDto, homePage);
		homePage.ModifiedDate = DateTime.UtcNow;
		repository.UpdateAsync(homePage);
		await repository.SaveChangesAsync();
		return Result<HomePageDto>.Success(mapper.Map<HomePageDto>(homePage));
	}

	public async Task<Result<HomePageDto>> DeleteHomePageAsync(Guid id)
	{
		var homePage = await repository.GetByIdAsync(id);
		if (homePage == null)
			return Result<HomePageDto>.Failure("Home page not found");
		homePage.Status = BaseStatuses.Deleted;
		homePage.ModifiedDate = DateTime.UtcNow;
		repository.UpdateAsync(homePage);
		await repository.SaveChangesAsync();
		return Result<HomePageDto>.Success(mapper.Map<HomePageDto>(homePage));
	}
}