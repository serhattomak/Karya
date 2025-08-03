using AutoMapper;
using Karya.Application.Features.File.Dto;
using Karya.Application.Features.Page.Dto;
using Karya.Application.Features.Page.Services.Interfaces;
using Karya.Application.Features.Product.Dto;
using Karya.Domain.Common;
using Karya.Domain.Enums;
using Karya.Domain.Interfaces;
using System.Net;

namespace Karya.Application.Features.Page.Services;

public class PageService(
	IPageRepository repository,
	IProductRepository productRepository,
	IFileRepository fileRepository,
	IMapper mapper) : IPageService
{
	public async Task<Result<PageDto>> GetPageByIdAsync(Guid id)
	{
		var page = await repository.GetByIdAsync(id);
		if (page == null)
			return Result<PageDto>.Failure("Page not found");

		return await MapPageToDto(page);
	}

	public async Task<Result<PageDto>> GetPageByNameAsync(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			return Result<PageDto>.Failure("Page name cannot be empty");

		var page = await repository.GetByNameAsync(name);
		if (page == null)
			return Result<PageDto>.Failure($"Page with name '{name}' not found");

		return await MapPageToDto(page);
	}

	public async Task<Result<PageDto>> GetPageBySlugAsync(string slug)
	{
		if (string.IsNullOrWhiteSpace(slug))
			return Result<PageDto>.Failure("Page slug cannot be empty");

		var page = await repository.GetBySlugAsync(slug);
		if (page == null)
			return Result<PageDto>.Failure($"Page with slug '{slug}' not found");

		return await MapPageToDto(page);
	}

	public async Task<Result<PagedResult<PageDto>>> GetAllPagesByTypeAsync(PageTypes type, PagedRequest request)
	{
		var pagedPages = await repository.GetPagedByTypeAsync(type, request);
		if (pagedPages == null || !pagedPages.Items.Any())
			return Result<PagedResult<PageDto>>.Failure("No pages found");

		var pageDtos = mapper.Map<List<PageDto>>(pagedPages.Items);

		foreach (var pageDto in pageDtos)
		{
			var page = pagedPages.Items.First(p => p.Id == pageDto.Id);
			await LoadPageRelatedData(page, pageDto);
		}

		var pagedResult = new PagedResult<PageDto>(
			pageDtos,
			pagedPages.TotalCount,
			pagedPages.PageNumber,
			pagedPages.PageSize
		);

		return Result<PagedResult<PageDto>>.Success(pagedResult);
	}

	public async Task<Result<PagedResult<PageDto>>> GetAllPagesAsync(PagedRequest request)
	{
		var pagedPages = await repository.GetPagedAsync(request);
		if (pagedPages == null || !pagedPages.Items.Any())
			return Result<PagedResult<PageDto>>.Failure("No pages found");

		var pageDtos = mapper.Map<List<PageDto>>(pagedPages.Items);

		foreach (var pageDto in pageDtos)
		{
			var page = pagedPages.Items.First(p => p.Id == pageDto.Id);
			await LoadPageRelatedData(page, pageDto);
		}

		var pagedResult = new PagedResult<PageDto>(
			pageDtos,
			pagedPages.TotalCount,
			pagedPages.PageNumber,
			pagedPages.PageSize
		);

		return Result<PagedResult<PageDto>>.Success(pagedResult);
	}

	public async Task<Result<PageDto>> CreatePageAsync(CreatePageDto pageDto)
	{
		var existingPageByName = await repository.GetByNameAsync(pageDto.Name);
		if (existingPageByName != null)
		{
			return Result<PageDto>.Failure($"Page with name '{pageDto.Name}' already exists");
		}

		var existingPageBySlug = await repository.GetBySlugAsync(pageDto.Slug);
		if (existingPageBySlug != null)
		{
			return Result<PageDto>.Failure($"Page with slug '{pageDto.Slug}' already exists");
		}

		var page = mapper.Map<Domain.Entities.Page>(pageDto);
		await repository.AddAsync(page);
		await repository.SaveChangesAsync();
		return Result<PageDto>.Success(mapper.Map<PageDto>(page));
	}

	public async Task<Result<PageDto>> UpdatePageAsync(UpdatePageDto pageDto)
	{
		var page = await repository.GetByIdAsync(pageDto.Id);
		if (page == null)
			return Result<PageDto>.Failure("Page not found");

		var existingPageByName = await repository.GetByNameAsync(pageDto.Name);
		if (existingPageByName != null && existingPageByName.Id != pageDto.Id)
		{
			return Result<PageDto>.Failure($"Page with name '{pageDto.Name}' already exists");
		}

		var existingPageBySlug = await repository.GetBySlugAsync(pageDto.Slug);
		if (existingPageBySlug != null && existingPageBySlug.Id != pageDto.Id)
		{
			return Result<PageDto>.Failure($"Page with slug '{pageDto.Slug}' already exists");
		}

		mapper.Map(pageDto, page);
		page.ModifiedDate = DateTime.UtcNow;
		repository.UpdateAsync(page);
		await repository.SaveChangesAsync();
		return Result<PageDto>.Success(mapper.Map<PageDto>(page));
	}

	public async Task<Result> DeletePageAsync(Guid id)
	{
		var page = await repository.GetByIdAsync(id);
		if (page == null)
			return Result.Failure("Page not found");
		page.Status = BaseStatuses.Deleted;
		page.ModifiedDate = DateTime.UtcNow;
		repository.UpdateAsync(page);
		await repository.SaveChangesAsync();
		return Result.Success(HttpStatusCode.NoContent);
	}

	public async Task<Result<PageDto>> UpdatePageProductOrderAsync(UpdatePageProductOrderDto updateDto)
	{
		var page = await repository.GetByIdAsync(updateDto.PageId);
		if (page == null)
			return Result<PageDto>.Failure("Page not found");

		if (updateDto.ProductIds != null && updateDto.ProductIds.Any())
		{
			var existingProducts = await productRepository.GetByIdsAsync(updateDto.ProductIds);
			var existingProductIds = existingProducts.Select(p => p.Id).ToHashSet();

			var invalidProductIds = updateDto.ProductIds.Where(id => !existingProductIds.Contains(id)).ToList();
			if (invalidProductIds.Any())
				return Result<PageDto>.Failure($"Invalid product IDs: {string.Join(", ", invalidProductIds)}");
		}

		page.ProductIds = updateDto.ProductIds;
		page.ModifiedDate = DateTime.UtcNow;

		repository.UpdateAsync(page);
		await repository.SaveChangesAsync();

		return await GetPageByIdAsync(page.Id);
	}

	private async Task<Result<PageDto>> MapPageToDto(Domain.Entities.Page page)
	{
		var pageDto = mapper.Map<PageDto>(page);
		await LoadPageRelatedData(page, pageDto);
		return Result<PageDto>.Success(pageDto);
	}

	private async Task LoadPageRelatedData(Domain.Entities.Page page, PageDto pageDto)
	{
		if (page.ProductIds != null && page.ProductIds.Any())
		{
			var products = await productRepository.GetByIdsAsync(page.ProductIds);
			var productDtos = mapper.Map<List<ProductDto>>(products);

			var orderedProductDtos = new List<ProductDto>();
			foreach (var productId in page.ProductIds)
			{
				var productDto = productDtos.FirstOrDefault(p => p.Id == productId);
				if (productDto != null)
				{
					var product = products.First(p => p.Id == productDto.Id);
					if (product.FileIds != null && product.FileIds.Any())
					{
						var files = await fileRepository.GetByIdsAsync(product.FileIds);
						productDto.Files = mapper.Map<List<FileDto>>(files);
					}
					orderedProductDtos.Add(productDto);
				}
			}
			pageDto.Products = orderedProductDtos;
		}

		if (page.FileIds != null && page.FileIds.Any())
		{
			var files = await fileRepository.GetByIdsAsync(page.FileIds);
			pageDto.Files = mapper.Map<List<FileDto>>(files);
		}
	}
}