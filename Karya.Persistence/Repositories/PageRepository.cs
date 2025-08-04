using Karya.Domain.Common;
using Karya.Domain.Entities;
using Karya.Domain.Enums;
using Karya.Domain.Interfaces;
using Karya.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Karya.Persistence.Repositories;

public class PageRepository(AppDbContext context) : EfRepository<Page>(context), IPageRepository
{
	public async Task<List<Page>> GetAllByTypeAsync(PageTypes type)
	{
		var pages = context.Pages.AsNoTracking()
			.Where(x => x.Status != BaseStatuses.Deleted && x.Status != BaseStatuses.Inactive && x.PageType == type);
		return await pages.ToListAsync();
	}

	public async Task<PagedResult<Page>> GetPagedByTypeAsync(PageTypes type, PagedRequest request)
	{
		var query = context.Pages
			.AsNoTracking()
			.Where(p => p.PageType == type &&
					   p.Status != BaseStatuses.Deleted &&
					   p.Status != BaseStatuses.Inactive);

		if (!string.IsNullOrEmpty(request.SortColumn))
		{
			if (request.SortColumn == "Name")
			{
				query = request.SortDirection == "DESC"
					? query.OrderByDescending(p => p.Name)
					: query.OrderBy(p => p.Name);
			}
		}

		var totalCount = await query.CountAsync();
		var items = await query
			.Skip((request.PageIndex - 1) * request.PageSize)
			.Take(request.PageSize)
			.ToListAsync();

		return new PagedResult<Page>(items, totalCount, request.PageIndex, request.PageSize);
	}

	public async Task<PagedResult<Page>> GetPagedAsync(PagedRequest request)
	{
		var query = context.Pages.AsNoTracking()
			.Where(p => p.Status != BaseStatuses.Deleted && p.Status != BaseStatuses.Inactive);

		if (!string.IsNullOrEmpty(request.SortColumn))
		{
			if (request.SortColumn == "Name")
			{
				query = request.SortDirection == "DESC"
					? query.OrderByDescending(p => p.Name)
					: query.OrderBy(p => p.Name);
			}
		}

		var totalCount = await query.CountAsync();
		var items = await query
			.Skip((request.PageIndex - 1) * request.PageSize)
			.Take(request.PageSize)
			.ToListAsync();

		return new PagedResult<Page>(items, totalCount, request.PageIndex, request.PageSize);
	}

	public async Task<Page?> GetByNameAsync(string name)
	{
		return await context.Pages
			.AsNoTracking()
			.FirstOrDefaultAsync(p => p.Name == name &&
									p.Status != BaseStatuses.Deleted &&
									p.Status != BaseStatuses.Inactive);
	}

	public async Task<Page?> GetBySlugAsync(string slug)
	{
		return await context.Pages
			.AsNoTracking()
			.FirstOrDefaultAsync(p => p.Slug == slug &&
									p.Status != BaseStatuses.Deleted &&
									p.Status != BaseStatuses.Inactive);
	}

	public async Task<Page?> GetByNameForUpdateAsync(string name)
	{
		return await context.Pages
			.FirstOrDefaultAsync(p => p.Name == name &&
									p.Status != BaseStatuses.Deleted &&
									p.Status != BaseStatuses.Inactive);
	}

	public async Task<Page?> GetBySlugForUpdateAsync(string slug)
	{
		return await context.Pages
			.FirstOrDefaultAsync(p => p.Slug == slug &&
									p.Status != BaseStatuses.Deleted &&
									p.Status != BaseStatuses.Inactive);
	}
}