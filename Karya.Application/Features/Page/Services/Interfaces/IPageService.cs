using Karya.Application.Features.Page.Dto;
using Karya.Domain.Common;
using Karya.Domain.Enums;

namespace Karya.Application.Features.Page.Services.Interfaces;

public interface IPageService
{
	Task<Result<PageDto>> GetPageByIdAsync(Guid id);
	Task<Result<PageDto>> GetPageByNameAsync(string name);
	Task<Result<PageDto>> GetPageBySlugAsync(string slug);
	Task<Result<PagedResult<PageDto>>> GetAllPagesByTypeAsync(PageTypes type, PagedRequest request);
	Task<Result<PagedResult<PageDto>>> GetAllPagesAsync(PagedRequest request);
	Task<Result<PageDto>> CreatePageAsync(CreatePageDto pageDto);
	Task<Result<PageDto>> UpdatePageAsync(UpdatePageDto pageDto);
	Task<Result> DeletePageAsync(Guid id);
	Task<Result<PageDto>> UpdatePageProductOrderAsync(UpdatePageProductOrderDto updateDto);
}