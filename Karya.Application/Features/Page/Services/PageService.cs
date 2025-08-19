using AutoMapper;
using Karya.Application.Features.Document.Dto;
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
	IDocumentRepository documentRepository,
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
		if (pagedPages == null || pagedPages.Items.Count == 0)
			return Result<PagedResult<PageDto>>.Failure("No pages found");

		var pageDtos = mapper.Map<List<PageDto>>(pagedPages.Items);

		// Batch loading for better performance
		await LoadPagesRelatedDataBatch(pageDtos, pagedPages.Items);

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
		if (pagedPages == null || pagedPages.Items.Count == 0)
			return Result<PagedResult<PageDto>>.Failure("No pages found");

		var pageDtos = mapper.Map<List<PageDto>>(pagedPages.Items);

		// Batch loading for better performance
		await LoadPagesRelatedDataBatch(pageDtos, pagedPages.Items);

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
		var existingPageByName = await repository.GetByNameForUpdateAsync(pageDto.Name);
		if (existingPageByName != null)
		{
			return Result<PageDto>.Failure($"Page with name '{pageDto.Name}' already exists");
		}

		var existingPageBySlug = await repository.GetBySlugForUpdateAsync(pageDto.Slug);
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

		var existingPageByName = await repository.GetByNameForUpdateAsync(pageDto.Name);
		if (existingPageByName != null && existingPageByName.Id != pageDto.Id)
		{
			return Result<PageDto>.Failure($"Page with name '{pageDto.Name}' already exists");
		}

		var existingPageBySlug = await repository.GetBySlugForUpdateAsync(pageDto.Slug);
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

		if (updateDto.ProductIds?.Count > 0)
		{
			var existingProducts = await productRepository.GetByIdsAsync(updateDto.ProductIds);
			var existingProductIds = new HashSet<Guid>(existingProducts.Select(p => p.Id));

			var invalidProductIds = new List<Guid>();
			foreach (var id in updateDto.ProductIds)
			{
				if (!existingProductIds.Contains(id))
					invalidProductIds.Add(id);
			}

			if (invalidProductIds.Count > 0)
				return Result<PageDto>.Failure($"Invalid product IDs: {string.Join(", ", invalidProductIds)}");
		}

		page.ProductIds = updateDto.ProductIds;
		page.ModifiedDate = DateTime.UtcNow;

		repository.UpdateAsync(page);
		await repository.SaveChangesAsync();

		return await GetPageByIdAsync(page.Id);
	}

	#region Private Helper Methods

	private async Task<Result<PageDto>> MapPageToDto(Domain.Entities.Page page)
	{
		var pageDto = mapper.Map<PageDto>(page);
		await LoadPageRelatedDataOptimized(page, pageDto);
		return Result<PageDto>.Success(pageDto);
	}

	/// <summary>
	/// Tek page için optimized related data loading
	/// </summary>
	private async Task LoadPageRelatedDataOptimized(Domain.Entities.Page page, PageDto pageDto)
	{
		// Collect all required IDs
		var allProductIds = new HashSet<Guid>();
		var allFileIds = new HashSet<Guid>();
		var allDocumentIds = new HashSet<Guid>();

		// Page level IDs
		if (page.ProductIds?.Count > 0)
			foreach (var id in page.ProductIds) allProductIds.Add(id);

		if (page.FileIds?.Count > 0)
			foreach (var id in page.FileIds) allFileIds.Add(id);

		if (page.MainImageId.HasValue)
			allFileIds.Add(page.MainImageId.Value);

		if (page.DocumentIds?.Count > 0)
			foreach (var id in page.DocumentIds) allDocumentIds.Add(id);

		// Ardışık data fetching
		var products = allProductIds.Count > 0
			? await productRepository.GetByIdsAsync(allProductIds)
			: new List<Domain.Entities.Product>();

		var files = allFileIds.Count > 0
			? await fileRepository.GetByIdsAsync(allFileIds)
			: new List<Domain.Entities.File>();

		var documents = allDocumentIds.Count > 0
			? await documentRepository.GetByIdsAsync(allDocumentIds)
			: new List<Domain.Entities.Document>();

		// Create lookup dictionaries
		var productLookup = new Dictionary<Guid, Domain.Entities.Product>(products.Count);
		foreach (var product in products)
		{
			productLookup[product.Id] = product;
		}

		var fileLookup = new Dictionary<Guid, FileDto>(files.Count);
		foreach (var file in files)
		{
			fileLookup[file.Id] = mapper.Map<FileDto>(file);
		}

		var documentLookup = new Dictionary<Guid, DocumentDto>(documents.Count);
		foreach (var document in documents)
		{
			documentLookup[document.Id] = mapper.Map<DocumentDto>(document);
		}

		// Collect product file IDs (include ProductImageId and ProductMainImageId)
		var productFileIds = new HashSet<Guid>();
		foreach (var product in products)
		{
			if (product.FileIds?.Count > 0)
				foreach (var id in product.FileIds) productFileIds.Add(id);
			if (product.ProductImageId.HasValue)
				productFileIds.Add(product.ProductImageId.Value);
			if (product.ProductMainImageId.HasValue)
				productFileIds.Add(product.ProductMainImageId.Value);
		}

		// Fetch product files if needed (ardışık)
		if (productFileIds.Count > 0)
		{
			var productFiles = await fileRepository.GetByIdsAsync(productFileIds);
			foreach (var file in productFiles)
			{
				if (!fileLookup.ContainsKey(file.Id))
					fileLookup[file.Id] = mapper.Map<FileDto>(file);
			}
		}

		// Map products with ordered result
		if (page.ProductIds?.Count > 0)
		{
			var orderedProductDtos = new List<ProductDto>(page.ProductIds.Count);
			foreach (var productId in page.ProductIds)
			{
				if (productLookup.TryGetValue(productId, out var product))
				{
					var productDto = mapper.Map<ProductDto>(product);
					// Map product files
					if (product.FileIds?.Count > 0)
					{
						var productFileDtos = new List<FileDto>(product.FileIds.Count);
						foreach (var fileId in product.FileIds)
						{
							if (fileLookup.TryGetValue(fileId, out var fileDto))
								productFileDtos.Add(fileDto);
						}
						productDto.Files = productFileDtos;
					}
					else
					{
						productDto.Files = [];
					}
					// Map ProductMainImage
					productDto.ProductMainImage = product.ProductMainImageId.HasValue && fileLookup.TryGetValue(product.ProductMainImageId.Value, out var productMainImageDto)
						? productMainImageDto
						: null;
					// Map ProductImage
					productDto.ProductImage = product.ProductImageId.HasValue && fileLookup.TryGetValue(product.ProductImageId.Value, out var productImageDto)
						? productImageDto
						: null;
					orderedProductDtos.Add(productDto);
				}
			}
			pageDto.Products = orderedProductDtos;
		}
		else
		{
			pageDto.Products = [];
		}
		// Map page files
		if (page.FileIds?.Count > 0)
		{
			var pageFileDtos = new List<FileDto>(page.FileIds.Count);
			foreach (var fileId in page.FileIds)
			{
				if (fileLookup.TryGetValue(fileId, out var fileDto))
					pageFileDtos.Add(fileDto);
			}
			pageDto.Files = pageFileDtos;
		}
		else
		{
			pageDto.Files = [];
		}
		// Map documents
		if (page.DocumentIds?.Count > 0)
		{
			var pageDocumentDtos = new List<DocumentDto>(page.DocumentIds.Count);
			foreach (var documentId in page.DocumentIds)
			{
				if (documentLookup.TryGetValue(documentId, out var documentDto))
					pageDocumentDtos.Add(documentDto);
			}
			pageDto.Documents = pageDocumentDtos;
		}
		else
		{
			pageDto.Documents = [];
		}
		// Map MainImage
		pageDto.MainImage = page.MainImageId.HasValue && fileLookup.TryGetValue(page.MainImageId.Value, out var pageMainImageDto)
			? pageMainImageDto
			: null;
	}

	/// <summary>
	/// Çoklu page'ler için batch related data loading
	/// </summary>
	private async Task LoadPagesRelatedDataBatch(List<PageDto> pageDtos, List<Domain.Entities.Page> pages)
	{
		if (pageDtos.Count == 0) return;

		// Collect all IDs from all pages
		var allProductIds = new HashSet<Guid>();
		var allFileIds = new HashSet<Guid>();
		var allDocumentIds = new HashSet<Guid>();

		foreach (var page in pages)
		{
			if (page.ProductIds?.Count > 0)
				foreach (var id in page.ProductIds) allProductIds.Add(id);

			if (page.FileIds?.Count > 0)
				foreach (var id in page.FileIds) allFileIds.Add(id);

			if (page.MainImageId.HasValue)
				allFileIds.Add(page.MainImageId.Value);

			if (page.DocumentIds?.Count > 0)
				foreach (var id in page.DocumentIds) allDocumentIds.Add(id);
		}

		// Ardışık data fetching
		var products = allProductIds.Count > 0
			? await productRepository.GetByIdsAsync(allProductIds)
			: new List<Domain.Entities.Product>();

		var files = allFileIds.Count > 0
			? await fileRepository.GetByIdsAsync(allFileIds)
			: new List<Domain.Entities.File>();

		var documents = allDocumentIds.Count > 0
			? await documentRepository.GetByIdsAsync(allDocumentIds)
			: new List<Domain.Entities.Document>();

		// Collect product file IDs
		var productFileIds = new HashSet<Guid>();
		foreach (var product in products)
		{
			if (product.FileIds?.Count > 0)
				foreach (var id in product.FileIds) productFileIds.Add(id);
		}

		// Fetch additional product files (ardışık)
		var additionalFiles = productFileIds.Count > 0
			? await fileRepository.GetByIdsAsync(productFileIds)
			: new List<Domain.Entities.File>();

		// Combine all files
		var allFiles = new List<Domain.Entities.File>(files.Count + additionalFiles.Count);
		allFiles.AddRange(files);
		foreach (var file in additionalFiles)
		{
			if (!files.Any(f => f.Id == file.Id))
				allFiles.Add(file);
		}

		// Create lookup dictionaries
		var productLookup = new Dictionary<Guid, Domain.Entities.Product>(products.Count);
		foreach (var product in products)
		{
			productLookup[product.Id] = product;
		}

		var fileLookup = new Dictionary<Guid, FileDto>(allFiles.Count);
		foreach (var file in allFiles)
		{
			fileLookup[file.Id] = mapper.Map<FileDto>(file);
		}

		var documentLookup = new Dictionary<Guid, DocumentDto>(documents.Count);
		foreach (var document in documents)
		{
			documentLookup[document.Id] = mapper.Map<DocumentDto>(document);
		}

		var pageLookup = new Dictionary<Guid, Domain.Entities.Page>(pages.Count);
		foreach (var page in pages)
		{
			pageLookup[page.Id] = page;
		}

		// Map each page
		foreach (var pageDto in pageDtos)
		{
			if (!pageLookup.TryGetValue(pageDto.Id, out var page)) continue;

			MapPageRelatedDataOptimized(pageDto, page, productLookup, fileLookup, documentLookup);
		}
	}

	/// <summary>
	/// Single page mapping with pre-loaded lookup dictionaries
	/// </summary>
	private static void MapPageRelatedDataOptimized(
		PageDto pageDto,
		Domain.Entities.Page page,
		Dictionary<Guid, Domain.Entities.Product> productLookup,
		Dictionary<Guid, FileDto> fileLookup,
		Dictionary<Guid, DocumentDto> documentLookup)
	{
		// Map products with order preservation
		if (page.ProductIds?.Count > 0)
		{
			var orderedProductDtos = new List<ProductDto>(page.ProductIds.Count);
			foreach (var productId in page.ProductIds)
			{
				if (productLookup.TryGetValue(productId, out var product))
				{
					var productDto = new ProductDto { Id = product.Id, Name = product.Name, Slug = product.Slug };

					// Map product files
					if (product.FileIds?.Count > 0)
					{
						var productFileDtos = new List<FileDto>(product.FileIds.Count);
						foreach (var fileId in product.FileIds)
						{
							if (fileLookup.TryGetValue(fileId, out var fileDto))
								productFileDtos.Add(fileDto);
						}
						productDto.Files = productFileDtos;
					}
					else
					{
						productDto.Files = [];
					}
					// Map ProductMainImage
					productDto.ProductMainImage = product.ProductMainImageId.HasValue && fileLookup.TryGetValue(product.ProductMainImageId.Value, out var productMainImageDto)
						? productMainImageDto
						: null;
					// Map ProductImage
					productDto.ProductImage = product.ProductImageId.HasValue && fileLookup.TryGetValue(product.ProductImageId.Value, out var productImageDto)
						? productImageDto
						: null;
					orderedProductDtos.Add(productDto);
				}
			}
			pageDto.Products = orderedProductDtos;
		}
		else
		{
			pageDto.Products = [];
		}
		// Map page files
		if (page.FileIds?.Count > 0)
		{
			var pageFileDtos = new List<FileDto>(page.FileIds.Count);
			foreach (var fileId in page.FileIds)
			{
				if (fileLookup.TryGetValue(fileId, out var fileDto))
					pageFileDtos.Add(fileDto);
			}
			pageDto.Files = pageFileDtos;
		}
		else
		{
			pageDto.Files = [];
		}
		// Map documents
		if (page.DocumentIds?.Count > 0)
		{
			var pageDocumentDtos = new List<DocumentDto>(page.DocumentIds.Count);
			foreach (var documentId in page.DocumentIds)
			{
				if (documentLookup.TryGetValue(documentId, out var documentDto))
					pageDocumentDtos.Add(documentDto);
			}
			pageDto.Documents = pageDocumentDtos;
		}
		else
		{
			pageDto.Documents = [];
		}
		// Map MainImage
		pageDto.MainImage = page.MainImageId.HasValue && fileLookup.TryGetValue(page.MainImageId.Value, out var pageMainImageDto)
			? pageMainImageDto
			: null;
	}

	#endregion
}