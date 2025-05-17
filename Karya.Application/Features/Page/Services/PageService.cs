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
		var pageDto = mapper.Map<PageDto>(page);
		if (page.ProductIds != null && page.ProductIds.Any())
		{
			var products = await productRepository.GetByIdsAsync(page.ProductIds);
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
			pageDto.Products = productDtos;
		}

		if (page.FileIds != null && page.FileIds.Any())
		{
			var files = await fileRepository.GetByIdsAsync(page.FileIds);
			pageDto.Files = mapper.Map<List<FileDto>>(files);
		}

		return Result<PageDto>.Success(pageDto);
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
			if (page.ProductIds != null && page.ProductIds.Any())
			{
				var products = await productRepository.GetByIdsAsync(page.ProductIds);
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
				pageDto.Products = productDtos;
			}
			if (page.FileIds != null && page.FileIds.Any())
			{
				var files = await fileRepository.GetByIdsAsync(page.FileIds);
				pageDto.Files = mapper.Map<List<FileDto>>(files);
			}
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
			if (page.ProductIds != null && page.ProductIds.Any())
			{
				var products = await productRepository.GetByIdsAsync(page.ProductIds);
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
				pageDto.Products = productDtos;
			}
			if (page.FileIds != null && page.FileIds.Any())
			{
				var files = await fileRepository.GetByIdsAsync(page.FileIds);
				pageDto.Files = mapper.Map<List<FileDto>>(files);
			}
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
}