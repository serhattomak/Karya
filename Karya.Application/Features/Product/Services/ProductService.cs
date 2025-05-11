using AutoMapper;
using Karya.Application.Features.File.Dto;
using Karya.Application.Features.Product.Dto;
using Karya.Application.Features.Product.Services.Interfaces;
using Karya.Domain.Common;
using Karya.Domain.Enums;
using Karya.Domain.Interfaces;
using System.Net;

namespace Karya.Application.Features.Product.Services;

public class ProductService(IMapper mapper, IProductRepository repository, IFileRepository fileRepository) : IProductService
{
	public async Task<Result<List<ProductDto>>> GetAllAsync()
	{
		var products = await repository.GetAllProductsAsync();
		if (products == null || !products.Any())
			return Result<List<ProductDto>>.Failure("No products found");
		var productDtos = mapper.Map<List<ProductDto>>(products);
		return Result<List<ProductDto>>.Success(productDtos);
	}

	public async Task<Result<ProductDto>> GetByIdAsync(Guid id)
	{
		var product = await repository.GetByIdAsync(id);
		if (product == null)
			return Result<ProductDto>.Failure("Product not found");

		var productDto = mapper.Map<ProductDto>(product);

		if (product.FileIds != null && product.FileIds.Any())
		{
			var files = await fileRepository.GetByIdsAsync(product.FileIds);
			productDto.Files = mapper.Map<List<FileDto>>(files);
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
