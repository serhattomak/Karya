using Karya.Application.Features.Document.Dto;
using Karya.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Karya.Application.Features.Document.Services.Interfaces;

public interface IDocumentService
{
	Task<Result<PagedResult<DocumentDto>>> GetAllAsync(PagedRequest request);
	Task<Result<DocumentDto>> GetByIdAsync(Guid id);
	Task<Result<DocumentDto>> GetByNameAsync(string name);
	Task<Result<DocumentDto>> GetBySlugAsync(string slug);
	Task<Result<List<DocumentDto>>> GetByCategoryAsync(string category);
	Task<Result<List<DocumentDto>>> GetActiveDocumentsAsync();
	Task<Result<DocumentDto>> CreateAsync(CreateDocumentDto documentDto);
	Task<Result<DocumentDto>> UpdateAsync(UpdateDocumentDto documentDto);
	Task<Result> DeleteAsync(Guid id);
	Task<Result<DocumentDto>> UploadDocumentAsync(IFormFile file, CreateDocumentDto documentDto);
	Task<Result> DownloadAsync(Guid id);
}