using Karya.Application.Features.File.Dto;
using Karya.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Karya.Application.Features.File.Services.Interfaces;

public interface IFileService
{
	Task<Result<List<FileDto>>> GetAllAsync();
	Task<Result<FileDto>> GetByIdAsync(Guid id);
	Task<Result<FileDto>> SaveFileAsync(IFormFile file);
	Task<Result<List<FileDto>>> SaveFilesAsync(List<IFormFile> files); // Çoklu dosya yükleme
	Task<Result<FileDto>> CreateAsync(CreateFileDto fileDto);
	Task<Result<FileDto>> UpdateAsync(UpdateFileDto fileDto);
	Task<Result> DeleteAsync(Guid id);
}