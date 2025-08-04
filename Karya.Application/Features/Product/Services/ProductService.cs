using AutoMapper;
using Karya.Application.Features.Document.Dto;
using Karya.Application.Features.File.Dto;
using Karya.Application.Features.Product.Dto;
using Karya.Application.Features.Product.Services.Interfaces;
using Karya.Domain.Common;
using Karya.Domain.Enums;
using Karya.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Karya.Application.Features.Product.Services;

public class ProductService(IMapper mapper, IProductRepository repository, IFileRepository fileRepository, IDocumentRepository documentRepository) : IProductService
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

		await LoadProductsRelatedData(productDtos, products);

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
		await LoadProductRelatedData(productDto, product);

		return Result<ProductDto>.Success(productDto);
	}

	public async Task<Result<ProductDto>> GetByNameAsync(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			return Result<ProductDto>.Failure("Product name cannot be empty");

		var product = await repository.GetByNameAsync(name);
		if (product == null)
			return Result<ProductDto>.Failure($"Product with name '{name}' not found");

		var productDto = mapper.Map<ProductDto>(product);
		await LoadProductRelatedData(productDto, product);

		return Result<ProductDto>.Success(productDto);
	}

	public async Task<Result<ProductDto>> GetBySlugAsync(string slug)
	{
		if (string.IsNullOrWhiteSpace(slug))
			return Result<ProductDto>.Failure("Product slug cannot be empty");

		var product = await repository.GetBySlugAsync(slug);
		if (product == null)
			return Result<ProductDto>.Failure($"Product with slug '{slug}' not found");

		var productDto = mapper.Map<ProductDto>(product);
		await LoadProductRelatedData(productDto, product);

		return Result<ProductDto>.Success(productDto);
	}

	public async Task<Result<ProductDto>> CreateAsync(CreateProductDto productDto)
	{
		var existingProductByName = await repository.GetByNameAsync(productDto.Name);
		if (existingProductByName != null)
		{
			return Result<ProductDto>.Failure($"Product with name '{productDto.Name}' already exists");
		}

		var existingProductBySlug = await repository.GetBySlugAsync(productDto.Slug);
		if (existingProductBySlug != null)
		{
			return Result<ProductDto>.Failure($"Product with slug '{productDto.Slug}' already exists");
		}

		if (productDto.DocumentIds != null && productDto.DocumentIds.Any())
		{
			var validationResult = await ValidateDocumentReferences(productDto.DocumentIds);
			if (!validationResult.IsSuccess)
				return Result<ProductDto>.Failure(validationResult.ErrorMessage!);
		}

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

		var existingProductByName = await repository.GetByNameAsync(productDto.Name);
		if (existingProductByName != null && existingProductByName.Id != productDto.Id)
		{
			return Result<ProductDto>.Failure($"Product with name '{productDto.Name}' already exists");
		}

		var existingProductBySlug = await repository.GetBySlugAsync(productDto.Slug);
		if (existingProductBySlug != null && existingProductBySlug.Id != productDto.Id)
		{
			return Result<ProductDto>.Failure($"Product with slug '{productDto.Slug}' already exists");
		}

		if (productDto.DocumentIds != null && productDto.DocumentIds.Any())
		{
			var validationResult = await ValidateDocumentReferences(productDto.DocumentIds);
			if (!validationResult.IsSuccess)
				return Result<ProductDto>.Failure(validationResult.ErrorMessage!);
		}

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

	#region Private Helper Methods

	/// <summary>
	/// Tekil product için related data yükleme
	/// </summary>
	private async Task LoadProductRelatedData(ProductDto productDto, Domain.Entities.Product product)
	{
		await LoadProductsRelatedData([productDto], [product]);
	}

	/// <summary>
	/// Çoklu productlar için related data yükleme (batch işlem)
	/// </summary>
	private async Task LoadProductsRelatedData(List<ProductDto> productDtos, List<Domain.Entities.Product> products)
	{
		if (!productDtos.Any()) return;

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

		var allDocumentIds = products
			.Where(p => p.DocumentIds != null && p.DocumentIds.Any())
			.SelectMany(p => p.DocumentIds!)
			.Distinct()
			.ToList();

		var files = allFileIds.Any()
			? await fileRepository.GetByIdsAsync(allFileIds)
			: new List<Domain.Entities.File>();

		var fileDtos = mapper.Map<List<FileDto>>(files);

		var documents = allDocumentIds.Any()
			? await documentRepository.GetByIdsAsync(allDocumentIds)
			: new List<Domain.Entities.Document>();

		var documentDtos = mapper.Map<List<DocumentDto>>(documents);

		foreach (var productDto in productDtos)
		{
			var product = products.First(p => p.Id == productDto.Id);
			MapProductFiles(productDto, product, fileDtos);
			MapProductDocuments(productDto, product, documentDtos);
		}
	}

	/// <summary>
	/// Product için file mapping
	/// </summary>
	private static void MapProductFiles(ProductDto productDto, Domain.Entities.Product product, List<FileDto> fileDtos)
	{
		// Files
		productDto.Files = product.FileIds?.Any() == true
			? fileDtos.Where(f => product.FileIds.Contains(f.Id)).ToList()
			: [];

		// Document Images
		productDto.DocumentImages = product.DocumentImageIds?.Any() == true
			? fileDtos.Where(f => product.DocumentImageIds.Contains(f.Id)).ToList()
			: [];

		// Product Detail Images
		productDto.ProductImages = product.ProductDetailImageIds?.Any() == true
			? fileDtos.Where(f => product.ProductDetailImageIds.Contains(f.Id)).ToList()
			: [];

		// Product Image (single)
		productDto.ProductImage = product.ProductImageId.HasValue
			? fileDtos.FirstOrDefault(f => f.Id == product.ProductImageId.Value)
			: null;
	}

	/// <summary>
	/// Product için document mapping
	/// </summary>
	private static void MapProductDocuments(ProductDto productDto, Domain.Entities.Product product, List<DocumentDto> documentDtos)
	{
		productDto.Documents = product.DocumentIds?.Any() == true
			? documentDtos.Where(d => product.DocumentIds.Contains(d.Id)).ToList()
			: [];
	}

	/// <summary>
	/// Document referansları validation
	/// </summary>
	private async Task<Result> ValidateDocumentReferences(List<Guid> documentIds)
	{
		var existingDocuments = await documentRepository.GetByIdsAsync(documentIds);
		var existingDocumentIds = existingDocuments.Select(d => d.Id).ToHashSet();

		var invalidDocumentIds = documentIds.Where(id => !existingDocumentIds.Contains(id)).ToList();

		if (invalidDocumentIds.Any())
		{
			return Result.Failure($"Invalid document IDs: {string.Join(", ", invalidDocumentIds)}");
		}

		return Result.Success();
	}

	#endregion
}