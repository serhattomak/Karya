using AutoMapper;
using Karya.Application.Features.Document.Dto;
using Karya.Application.Features.Document.Services.Interfaces;
using Karya.Application.Features.File.Dto;
using Karya.Application.Features.File.Services.Interfaces;
using Karya.Domain.Common;
using Karya.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Karya.Application.Features.Document.Services;

public class DocumentService(
	IMapper mapper,
	IDocumentRepository repository,
	IFileRepository fileRepository,
	IFileService fileService,
	IProductRepository productRepository,
	IPageRepository pageRepository) : IDocumentService
{
	public async Task<Result<PagedResult<DocumentDto>>> GetAllAsync(PagedRequest request)
	{
		var pagedDocuments = await repository.GetPagedAsync(request);
		if (pagedDocuments == null || !pagedDocuments.Items.Any())
			return Result<PagedResult<DocumentDto>>.Failure("No documents found");

		var documentDtos = mapper.Map<List<DocumentDto>>(pagedDocuments.Items);

		await LoadDocumentFiles(documentDtos);

		var pagedResult = new PagedResult<DocumentDto>(
			documentDtos,
			pagedDocuments.TotalCount,
			pagedDocuments.PageNumber,
			pagedDocuments.PageSize
		);

		return Result<PagedResult<DocumentDto>>.Success(pagedResult);
	}

	public async Task<Result<DocumentDto>> GetByIdAsync(Guid id)
	{
		var document = await repository.GetByIdAsync(id);
		if (document == null)
			return Result<DocumentDto>.Failure("Document not found");

		var documentDto = mapper.Map<DocumentDto>(document);
		await LoadDocumentFiles([documentDto]);

		return Result<DocumentDto>.Success(documentDto);
	}

	public async Task<Result<DocumentDto>> GetByNameAsync(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			return Result<DocumentDto>.Failure("Document name cannot be empty");

		var document = await repository.GetByNameAsync(name);
		if (document == null)
			return Result<DocumentDto>.Failure($"Document with name '{name}' not found");

		var documentDto = mapper.Map<DocumentDto>(document);
		await LoadDocumentFiles([documentDto]);

		return Result<DocumentDto>.Success(documentDto);
	}

	public async Task<Result<DocumentDto>> GetBySlugAsync(string slug)
	{
		if (string.IsNullOrWhiteSpace(slug))
			return Result<DocumentDto>.Failure("Document slug cannot be empty");

		var document = await repository.GetBySlugAsync(slug);
		if (document == null)
			return Result<DocumentDto>.Failure($"Document with slug '{slug}' not found");

		var documentDto = mapper.Map<DocumentDto>(document);
		await LoadDocumentFiles([documentDto]);

		return Result<DocumentDto>.Success(documentDto);
	}

	public async Task<Result<List<DocumentDto>>> GetByCategoryAsync(string category)
	{
		if (string.IsNullOrWhiteSpace(category))
			return Result<List<DocumentDto>>.Failure("Category cannot be empty");

		var documents = await repository.GetByCategoryAsync(category);
		var documentDtos = mapper.Map<List<DocumentDto>>(documents);
		await LoadDocumentFiles(documentDtos);

		return Result<List<DocumentDto>>.Success(documentDtos);
	}

	public async Task<Result<List<DocumentDto>>> GetActiveDocumentsAsync()
	{
		var documents = await repository.GetActiveDocumentsAsync();
		var documentDtos = mapper.Map<List<DocumentDto>>(documents);
		await LoadDocumentFiles(documentDtos);

		return Result<List<DocumentDto>>.Success(documentDtos);
	}

	public async Task<Result<DocumentDto>> CreateAsync(CreateDocumentDto documentDto)
	{
		var existingDocumentByName = await repository.GetByNameForUpdateAsync(documentDto.Name);
		if (existingDocumentByName != null)
		{
			return Result<DocumentDto>.Failure($"Document with name '{documentDto.Name}' already exists");
		}

		var existingDocumentBySlug = await repository.GetBySlugForUpdateAsync(documentDto.Slug);
		if (existingDocumentBySlug != null)
		{
			return Result<DocumentDto>.Failure($"Document with slug '{documentDto.Slug}' already exists");
		}

		if (documentDto.FileId.HasValue)
		{
			var file = await fileRepository.GetByIdAsync(documentDto.FileId.Value);
			if (file == null)
				return Result<DocumentDto>.Failure("Referenced file not found");
		}

		if (documentDto.PreviewImageFileId.HasValue)
		{
			var previewFile = await fileRepository.GetByIdAsync(documentDto.PreviewImageFileId.Value);
			if (previewFile == null)
				return Result<DocumentDto>.Failure("Referenced preview image file not found");
		}

		var document = mapper.Map<Domain.Entities.Document>(documentDto);
		await repository.AddAsync(document);
		await repository.SaveChangesAsync();

		return Result<DocumentDto>.Success(mapper.Map<DocumentDto>(document));
	}

	public async Task<Result<DocumentDto>> UpdateAsync(UpdateDocumentDto documentDto)
	{
		var document = await repository.GetByIdAsync(documentDto.Id);
		if (document == null)
			return Result<DocumentDto>.Failure("Document not found");

		var existingDocumentByName = await repository.GetByNameForUpdateAsync(documentDto.Name);
		if (existingDocumentByName != null && existingDocumentByName.Id != documentDto.Id)
		{
			return Result<DocumentDto>.Failure($"Document with name '{documentDto.Name}' already exists");
		}

		var existingDocumentBySlug = await repository.GetBySlugForUpdateAsync(documentDto.Slug);
		if (existingDocumentBySlug != null && existingDocumentBySlug.Id != documentDto.Id)
		{
			return Result<DocumentDto>.Failure($"Document with slug '{documentDto.Slug}' already exists");
		}

		mapper.Map(documentDto, document);
		document.ModifiedDate = DateTime.UtcNow;

		repository.UpdateAsync(document);
		await repository.SaveChangesAsync();

		return Result<DocumentDto>.Success(mapper.Map<DocumentDto>(document));
	}

	public async Task<Result<DocumentDto>> UploadDocumentAsync(IFormFile file, CreateDocumentDto documentDto)
	{
		var allowedTypes = new[] {
			"application/pdf",
			"application/vnd.openxmlformats-officedocument.wordprocessingml.document",
			"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
			"application/msword",
			"application/vnd.ms-excel",
			"text/plain"
		};

		if (!allowedTypes.Contains(file.ContentType))
		{
			return Result<DocumentDto>.Failure("Invalid file type. Only PDF, DOCX, XLSX, DOC, XLS and TXT files are allowed.");
		}

		if (file.Length > 50 * 1024 * 1024)
		{
			return Result<DocumentDto>.Failure("File size cannot exceed 50MB.");
		}

		var fileUploadResult = await fileService.SaveFileAsync(file);
		if (!fileUploadResult.IsSuccess)
		{
			return Result<DocumentDto>.Failure($"File upload failed: {string.Join(", ", fileUploadResult.ErrorMessage ?? new List<string>())}");
		}

		var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "documents");
		if (!Directory.Exists(rootPath))
			Directory.CreateDirectory(rootPath);

		var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
		var fullPath = Path.Combine(rootPath, fileName);

		using (var stream = new FileStream(fullPath, FileMode.Create))
		{
			await file.CopyToAsync(stream);
		}

		var relativePath = Path.Combine("uploads", "documents", fileName).Replace("\\", "/");

		string fileHash;
		using (var fileStream = file.OpenReadStream())
		{
			using var sha256 = System.Security.Cryptography.SHA256.Create();
			var hashBytes = sha256.ComputeHash(fileStream);
			fileHash = Convert.ToHexString(hashBytes);
		}

		var existingFile = await fileRepository.GetByHashForUpdateAsync(fileHash);
		Guid fileEntityId;

		if (existingFile != null)
		{
			if (System.IO.File.Exists(fullPath))
			{
				System.IO.File.Delete(fullPath);
			}
			fileEntityId = existingFile.Id;
		}
		else
		{
			var fileEntity = new Domain.Entities.File
			{
				Name = file.FileName,
				Path = relativePath,
				ContentType = file.ContentType,
				Size = file.Length,
				Hash = fileHash
			};

			await fileRepository.AddAsync(fileEntity);
			await fileRepository.SaveChangesAsync();
			fileEntityId = fileEntity.Id;
		}

		var createDto = documentDto with
		{
			FileId = fileEntityId,
			MimeType = file.ContentType,
			FileSize = file.Length
		};

		return await CreateAsync(createDto);
	}

	public async Task<Result> DeleteAsync(Guid id)
	{
		var document = await repository.GetByIdAsync(id);
		if (document == null)
			return Result.Failure("Document not found");

		var products = (await productRepository.GetAllProductsAsync()).ToList();
		foreach (var product in products.Where(p => p.DocumentIds != null && p.DocumentIds.Contains(id)))
		{
			product.DocumentIds!.Remove(id);
			productRepository.UpdateAsync(product);
		}

		var pages = await pageRepository.GetPagedAsync(new PagedRequest { PageIndex = 1, PageSize = int.MaxValue });
		foreach (var page in pages.Items.Where(p => p.DocumentIds != null && p.DocumentIds.Contains(id)))
		{
			page.DocumentIds!.Remove(id);
			pageRepository.UpdateAsync(page);
		}

		repository.DeleteAsync(document);
		await repository.SaveChangesAsync();

		return Result.Success(HttpStatusCode.NoContent);
	}

	public async Task<Result> DownloadAsync(Guid id)
	{
		var document = await repository.GetByIdAsync(id);
		if (document == null)
			return Result.Failure("Document not found");

		await repository.IncrementDownloadCountAsync(id);

		return Result.Success();
	}

	private async Task LoadDocumentFiles(List<DocumentDto> documentDtos)
	{
		var fileIds = documentDtos
			.Where(d => d.FileId.HasValue)
			.Select(d => d.FileId!.Value)
			.ToList();

		var previewFileIds = documentDtos
			.Where(d => d.PreviewImageFileId.HasValue)
			.Select(d => d.PreviewImageFileId!.Value)
			.ToList();

		var allFileIds = fileIds.Concat(previewFileIds).Distinct().ToList();

		if (allFileIds.Any())
		{
			var files = await fileRepository.GetByIdsAsync(allFileIds);
			var fileDtos = mapper.Map<List<FileDto>>(files);

			foreach (var documentDto in documentDtos)
			{
				if (documentDto.FileId.HasValue)
				{
					documentDto.File = fileDtos.FirstOrDefault(f => f.Id == documentDto.FileId.Value);
				}

				if (documentDto.PreviewImageFileId.HasValue)
				{
					documentDto.PreviewImageFile = fileDtos.FirstOrDefault(f => f.Id == documentDto.PreviewImageFileId.Value);
				}
			}
		}
	}
}