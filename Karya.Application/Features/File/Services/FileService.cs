using AutoMapper;
using Karya.Application.Features.File.Dto;
using Karya.Application.Features.File.Services.Interfaces;
using Karya.Domain.Common;
using Karya.Domain.Enums;
using Karya.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Karya.Application.Features.File.Services;

public class FileService(IMapper mapper, IFileRepository repository, IProductRepository productRepository) : IFileService
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
			Size = file.Length
		};

		await repository.AddAsync(fileEntity);
		await repository.SaveChangesAsync();

		var result = new FileDto
		{
			Id = fileEntity.Id,
			Name = fileEntity.Name,
			Path = relativePath,
			ContentType = file.ContentType,
			Size = fileEntity.Size
		};
		return Result<FileDto>.Success(result);
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
		foreach (var product in products.Where(p => p.FileIds != null && p.FileIds.Contains(id)))
		{
			product.FileIds!.Remove(id);
			productRepository.UpdateAsync(product);
		}
		file.Status = BaseStatuses.Deleted;
		file.ModifiedDate = DateTime.UtcNow;
		repository.UpdateAsync(file);
		await repository.SaveChangesAsync();
		return Result.Success(HttpStatusCode.NoContent);
	}
}