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
		var existingProductByName = await repository.GetByNameForUpdateAsync(productDto.Name);
		if (existingProductByName != null)
		{
			return Result<ProductDto>.Failure($"Product with name '{productDto.Name}' already exists");
		}

		var existingProductBySlug = await repository.GetBySlugForUpdateAsync(productDto.Slug);
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

		var existingProductByName = await repository.GetByNameForUpdateAsync(productDto.Name);
		if (existingProductByName != null && existingProductByName.Id != productDto.Id)
		{
			return Result<ProductDto>.Failure($"Product with name '{productDto.Name}' already exists");
		}

		var existingProductBySlug = await repository.GetBySlugForUpdateAsync(productDto.Slug);
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
	/// Çoklu productlar için related data yükleme (batch işlem) - Thread Safe
	/// </summary>
	private async Task LoadProductsRelatedData(List<ProductDto> productDtos, List<Domain.Entities.Product> products)
	{
		if (productDtos.Count == 0) return;

		var allFileIds = new HashSet<Guid>();
		var allDocumentIds = new HashSet<Guid>();
		var allPreviewImageFileIds = new HashSet<Guid>();
		foreach (var product in products)
		{
			if (product.FileIds?.Count > 0)
				foreach (var id in product.FileIds) allFileIds.Add(id);

			if (product.DocumentImageIds?.Count > 0)
				foreach (var id in product.DocumentImageIds) allFileIds.Add(id);

			if (product.ProductDetailImageIds?.Count > 0)
				foreach (var id in product.ProductDetailImageIds) allFileIds.Add(id);

			if (product.ProductImageId.HasValue)
				allFileIds.Add(product.ProductImageId.Value);

			if (product.ProductMainImageId.HasValue)
				allFileIds.Add(product.ProductMainImageId.Value);

			if (product.DocumentIds?.Count > 0)
				foreach (var id in product.DocumentIds) allDocumentIds.Add(id);
		}
		// Collect PreviewImageFileIds from documents
		var documents = allDocumentIds.Count > 0
			? await documentRepository.GetByIdsAsync([.. allDocumentIds])
			: new List<Domain.Entities.Document>();
		foreach (var document in documents)
		{
			if (document.PreviewImageFileId.HasValue)
				allFileIds.Add(document.PreviewImageFileId.Value);
		}
		// Fetch all files (including preview images)
		var files = allFileIds.Count > 0
			? await fileRepository.GetByIdsAsync([.. allFileIds])
			: new List<Domain.Entities.File>();

		var fileDtoLookup = new Dictionary<Guid, FileDto>(files.Count);
		var documentDtoLookup = new Dictionary<Guid, DocumentDto>(documents.Count);

		foreach (var file in files)
		{
			fileDtoLookup[file.Id] = mapper.Map<FileDto>(file);
		}

		foreach (var document in documents)
		{
			var dto = mapper.Map<DocumentDto>(document);
			if (document.PreviewImageFileId.HasValue && fileDtoLookup.TryGetValue(document.PreviewImageFileId.Value, out var previewFileDto))
				dto.PreviewImageFile = previewFileDto;
			documentDtoLookup[document.Id] = dto;
		}

		var productLookup = new Dictionary<Guid, Domain.Entities.Product>(products.Count);
		foreach (var product in products)
		{
			productLookup[product.Id] = product;
		}

		foreach (var productDto in productDtos)
		{
			if (!productLookup.TryGetValue(productDto.Id, out var product)) continue;

			MapProductFilesOptimized(productDto, product, fileDtoLookup);
			MapProductDocumentsOptimized(productDto, product, documentDtoLookup);
		}
	}

	/// <summary>
	/// Product için file mapping - Optimized
	/// </summary>
	private static void MapProductFilesOptimized(ProductDto productDto, Domain.Entities.Product product, Dictionary<Guid, FileDto> fileDtoLookup)
	{
		// Files - List initialization with capacity
		if (product.FileIds?.Count > 0)
		{
			var files = new List<FileDto>(product.FileIds.Count);
			foreach (var id in product.FileIds)
			{
				if (fileDtoLookup.TryGetValue(id, out var fileDto))
					files.Add(fileDto);
			}
			productDto.Files = files;
		}
		else
		{
			productDto.Files = [];
		}

		// Document Images
		if (product.DocumentImageIds?.Count > 0)
		{
			var documentImages = new List<FileDto>(product.DocumentImageIds.Count);
			foreach (var id in product.DocumentImageIds)
			{
				if (fileDtoLookup.TryGetValue(id, out var fileDto))
					documentImages.Add(fileDto);
			}
			productDto.DocumentImages = documentImages;
		}
		else
		{
			productDto.DocumentImages = [];
		}

		// Product Detail Images
		if (product.ProductDetailImageIds?.Count > 0)
		{
			var productImages = new List<FileDto>(product.ProductDetailImageIds.Count);
			foreach (var id in product.ProductDetailImageIds)
			{
				if (fileDtoLookup.TryGetValue(id, out var fileDto))
					productImages.Add(fileDto);
			}
			productDto.ProductImages = productImages;
		}
		else
		{
			productDto.ProductImages = [];
		}

		// Product Main Image (single)
		productDto.ProductMainImage = product.ProductMainImageId.HasValue && fileDtoLookup.TryGetValue(product.ProductMainImageId.Value, out var productMainImage)
			? productMainImage
			: null;

		// Product Image (single)
		productDto.ProductImage = product.ProductImageId.HasValue && fileDtoLookup.TryGetValue(product.ProductImageId.Value, out var productImage)
			? productImage
			: null;
	}

	/// <summary>
	/// Product için document mapping - Optimized
	/// </summary>
	private static void MapProductDocumentsOptimized(ProductDto productDto, Domain.Entities.Product product, Dictionary<Guid, DocumentDto> documentDtoLookup)
	{
		if (product.DocumentIds?.Count > 0)
		{
			var documents = new List<DocumentDto>(product.DocumentIds.Count);
			foreach (var id in product.DocumentIds)
			{
				if (documentDtoLookup.TryGetValue(id, out var documentDto))
					documents.Add(documentDto);
			}
			productDto.Documents = documents;
		}
		else
		{
			productDto.Documents = [];
		}
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