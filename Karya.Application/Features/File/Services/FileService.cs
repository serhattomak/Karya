using AutoMapper;
using Karya.Application.Features.File.Dto;
using Karya.Application.Features.File.Services.Interfaces;
using Karya.Domain.Common;
using Karya.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Security.Cryptography;

namespace Karya.Application.Features.File.Services;

public class FileService(IMapper mapper, IFileRepository repository, IProductRepository productRepository, IPageRepository pageRepository) : IFileService
{
	public async Task<Result<List<FileDto>>> GetAllAsync()
	{
		var files = await repository.GetAllFilesAsync();
		if (files == null || !files.Any())
			return Result<List<FileDto>>.Failure("No files found");
		var fileDtos = mapper.Map<List<FileDto>>(files);
		return Result<List<FileDto>>.Success(fileDtos);
	}

	public async Task<Result<FileDto>> GetByIdAsync(Guid id)
	{
		var file = await repository.GetByIdAsync(id);
		if (file == null)
			return Result<FileDto>.Failure("File not found");
		var fileDto = mapper.Map<FileDto>(file);
		return Result<FileDto>.Success(fileDto);
	}

	public async Task<Result<FileDto>> SaveFileAsync(IFormFile file)
	{
		string fileHash;
		using (var stream = file.OpenReadStream())
		{
			fileHash = await ComputeFileHashAsync(stream);
		}

		var existingFile = await repository.GetByHashAsync(fileHash);
		if (existingFile != null)
		{
			var existingFileDto = mapper.Map<FileDto>(existingFile);
			var result = Result<FileDto>.Success(existingFileDto, HttpStatusCode.OK);
			result.ErrorMessage = ["Dosya zaten sistemde mevcut, otomatik olarak seçildi."];
			return result;
		}

		var rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
		if (!Directory.Exists(rootPath))
			Directory.CreateDirectory(rootPath);

		var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
		var fullPath = Path.Combine(rootPath, fileName);

		using (var stream = new FileStream(fullPath, FileMode.Create))
		{
			await file.CopyToAsync(stream);
		}

		var relativePath = Path.Combine("uploads", fileName).Replace("\\", "/");

		var fileEntity = new Domain.Entities.File
		{
			Name = file.FileName,
			Path = relativePath,
			ContentType = file.ContentType,
			Size = file.Length,
			Hash = fileHash
		};

		await repository.AddAsync(fileEntity);
		await repository.SaveChangesAsync();

		var resultDto = new FileDto
		{
			Id = fileEntity.Id,
			Name = fileEntity.Name,
			Path = relativePath,
			ContentType = file.ContentType,
			Size = fileEntity.Size
		};
		return Result<FileDto>.Success(resultDto);
	}

	public async Task<Result<FileDto>> CreateAsync(CreateFileDto fileDto)
	{
		var file = mapper.Map<Domain.Entities.File>(fileDto);
		await repository.AddAsync(file);
		await repository.SaveChangesAsync();
		return Result<FileDto>.Success(mapper.Map<FileDto>(file));
	}

	public async Task<Result<FileDto>> UpdateAsync(UpdateFileDto fileDto)
	{
		var file = await repository.GetByIdAsync(fileDto.Id);
		if (file == null)
			return Result<FileDto>.Failure("File not found");
		mapper.Map(fileDto, file);
		file.ModifiedDate = DateTime.UtcNow;
		repository.UpdateAsync(file);
		await repository.SaveChangesAsync();
		return Result<FileDto>.Success(mapper.Map<FileDto>(file));
	}

	public async Task<Result> DeleteAsync(Guid id)
	{
		var file = await repository.GetByIdAsync(id);
		if (file == null)
			return Result.Failure("File not found");
		var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.Path);
		if (System.IO.File.Exists(filePath))
		{
			System.IO.File.Delete(filePath);
		}
		var products = (await productRepository.GetAllProductsAsync()).ToList();
		foreach (var product in products)
		{
			bool updated = false;
			if (product.FileIds != null && product.FileIds.Contains(id))
			{
				product.FileIds.Remove(id);
				updated = true;
			}
			if (product.ProductDetailImageIds != null && product.ProductDetailImageIds.Contains(id))
			{
				product.ProductDetailImageIds.Remove(id);
				updated = true;
			}
			if (updated)
				productRepository.UpdateAsync(product);
		}
		var pages = await pageRepository.GetPagedAsync(new PagedRequest { PageIndex = 1, PageSize = int.MaxValue });
		foreach (var page in pages.Items.Where(p => p.FileIds != null && p.FileIds.Contains(id)))
		{
			page.FileIds!.Remove(id);
			pageRepository.UpdateAsync(page);
		}
		repository.DeleteAsync(file);
		await repository.SaveChangesAsync();
		return Result.Success(HttpStatusCode.NoContent);
	}

	public async Task<Result<List<FileDto>>> SaveFilesAsync(List<IFormFile> files)
	{
		var fileDtos = new List<FileDto>();
		var errorMessages = new List<string>();
		foreach (var file in files)
		{
			var result = await SaveFileAsync(file);
			if (result.IsSuccess && result.Data != null)
			{
				fileDtos.Add(result.Data);
				if (result.ErrorMessage != null && result.ErrorMessage.Count > 0)
					errorMessages.AddRange(result.ErrorMessage);
			}
			else if (result.ErrorMessage != null)
			{
				errorMessages.AddRange(result.ErrorMessage);
			}
		}
		var finalResult = Result<List<FileDto>>.Success(fileDtos);
		if (errorMessages.Count > 0)
			finalResult.ErrorMessage = errorMessages;
		return finalResult;
	}

	private static async Task<string> ComputeFileHashAsync(Stream stream)
	{
		using var sha256 = SHA256.Create();
		var hashBytes = await Task.Run(() => sha256.ComputeHash(stream));
		return Convert.ToHexString(hashBytes);
	}
}