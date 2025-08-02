using AutoMapper;
using Karya.Application.Features.File.Dto;
using Karya.Application.Features.Product.Dto;
using Karya.Application.Features.Product.Services.Interfaces;
using Karya.Domain.Common;
using Karya.Domain.Enums;
using Karya.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Karya.Application.Features.Product.Services;

public class ProductService(IMapper mapper, IProductRepository repository, IFileRepository fileRepository) : IProductService
{
	public async Task<Result<PagedResult<ProductDto>>> GetAllAsync(PagedRequest request)
	{
		var queryable = await repository.GetAllProductsAsync();

		if (!string.IsNullOrEmpty(request.SortColumn))
		{
			queryable = request.SortDirection?.ToUpper() == "DESC"
				? queryable.OrderByDescending(e => EF.Property<object>(e, request.SortColumn))
				: queryable.OrderBy(e => EF.Property<object>(e, request.SortColumn));
		}

		var totalCount = await queryable.CountAsync();

		var products = await queryable
			.Skip((request.PageIndex - 1) * request.PageSize)
			.Take(request.PageSize)
			.ToListAsync();

		var productDtos = mapper.Map<List<ProductDto>>(products);

		var allFileIds = products
			.SelectMany(p => new List<Guid>[]
			{
				p.FileIds ?? [],
				p.DocumentImageIds ?? [],
				p.ProductDetailImageIds ?? []
			})
			.SelectMany(ids => ids)
			.Concat(products.Where(p => p.ProductImageId.HasValue).Select(p => p.ProductImageId!.Value))
			.Distinct()
			.ToList();

		var files = allFileIds.Any()
			? await fileRepository.GetByIdsAsync(allFileIds)
			: new List<Domain.Entities.File>();

		var fileDtos = mapper.Map<List<FileDto>>(files);

		foreach (var productDto in productDtos)
		{
			var product = products.First(p => p.Id == productDto.Id);

			if (productDto.FileIds != null && productDto.FileIds.Any())
			{
				productDto.Files = fileDtos
					.Where(f => productDto.FileIds.Contains(f.Id))
					.ToList();
			}
			else
			{
				productDto.Files = [];
			}

			if (productDto.DocumentImageIds != null && productDto.DocumentImageIds.Any())
			{
				productDto.DocumentImages = fileDtos
					.Where(f => productDto.DocumentImageIds.Contains(f.Id))
					.ToList();
			}
			else
			{
				productDto.DocumentImages = [];
			}

			if (productDto.ProductDetailImageIds != null && productDto.ProductDetailImageIds.Any())
			{
				productDto.ProductImages = fileDtos
					.Where(f => productDto.ProductDetailImageIds.Contains(f.Id))
					.ToList();
			}
			else
			{
				productDto.ProductImages = [];
			}

			if (product.ProductImageId.HasValue)
			{
				productDto.ProductImage = fileDtos.FirstOrDefault(f => f.Id == product.ProductImageId.Value);
			}
		}

		var pagedResult = new PagedResult<ProductDto>(
			productDtos,
			totalCount,
			request.PageIndex,
			request.PageSize
		);

		return Result<PagedResult<ProductDto>>.Success(pagedResult);
	}

	public async Task<Result<ProductDto>> GetByIdAsync(Guid id)
	{
		var product = await repository.GetByIdAsync(id);
		if (product == null)
			return Result<ProductDto>.Failure("Product not found");

		var productDto = mapper.Map<ProductDto>(product);

		var allFileIds = new List<Guid>();

		if (product.FileIds != null && product.FileIds.Any())
			allFileIds.AddRange(product.FileIds);

		if (product.DocumentImageIds != null && product.DocumentImageIds.Any())
			allFileIds.AddRange(product.DocumentImageIds);

		if (product.ProductDetailImageIds != null && product.ProductDetailImageIds.Any())
			allFileIds.AddRange(product.ProductDetailImageIds);

		if (product.ProductImageId.HasValue)
			allFileIds.Add(product.ProductImageId.Value);

		if (allFileIds.Any())
		{
			var files = await fileRepository.GetByIdsAsync(allFileIds.Distinct());
			var fileDtos = mapper.Map<List<FileDto>>(files);

			if (product.FileIds != null && product.FileIds.Any())
			{
				productDto.Files = fileDtos
					.Where(f => product.FileIds.Contains(f.Id))
					.ToList();
			}

			if (product.DocumentImageIds != null && product.DocumentImageIds.Any())
			{
				productDto.DocumentImages = fileDtos
					.Where(f => product.DocumentImageIds.Contains(f.Id))
					.ToList();
			}

			if (product.ProductDetailImageIds != null && product.ProductDetailImageIds.Any())
			{
				productDto.ProductImages = fileDtos
					.Where(f => product.ProductDetailImageIds.Contains(f.Id))
					.ToList();
			}

			if (product.ProductImageId.HasValue)
			{
				productDto.ProductImage = fileDtos.FirstOrDefault(f => f.Id == product.ProductImageId.Value);
			}
		}

		return Result<ProductDto>.Success(productDto);
	}

	public async Task<Result<ProductDto>> CreateAsync(CreateProductDto productDto)
	{
		var product = mapper.Map<Domain.Entities.Product>(productDto);
		await repository.AddAsync(product);
		await repository.SaveChangesAsync();

		return Result<ProductDto>.Success(mapper.Map<ProductDto>(product));
	}

	public async Task<Result<ProductDto>> UpdateAsync(UpdateProductDto productDto)
	{
		var product = await repository.GetByIdAsync(productDto.Id);
		if (product == null)
			return Result<ProductDto>.Failure("Product not found");

		mapper.Map(productDto, product);
		product.ModifiedDate = DateTime.UtcNow;

		repository.UpdateAsync(product);
		await repository.SaveChangesAsync();

		return Result<ProductDto>.Success(mapper.Map<ProductDto>(product));
	}

	public async Task<Result> DeleteAsync(Guid id)
	{
		var product = await repository.GetByIdAsync(id);
		if (product == null)
			return Result.Failure("Product not found");

		product.Status = BaseStatuses.Deleted;
		product.ModifiedDate = DateTime.UtcNow;

		repository.UpdateAsync(product);
		await repository.SaveChangesAsync();

		return Result.Success(HttpStatusCode.NoContent);
	}
}