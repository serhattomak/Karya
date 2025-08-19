using Karya.Domain.Common;
using Karya.Domain.Entities;
using Karya.Domain.Enums;
using Karya.Domain.Interfaces;
using Karya.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Karya.Persistence.Repositories;

public class DocumentRepository(AppDbContext context) : EfRepository<Document>(context), IDocumentRepository
{
	public async Task<IQueryable<Document>> GetAllDocumentsAsync()
	{
		var documents = context.Documents.AsNoTracking()
			.Where(x => x.Status != BaseStatuses.Deleted && x.Status != BaseStatuses.Inactive);
		return documents;
	}

	public async Task<List<Document>> GetByIdsAsync(IEnumerable<Guid> ids)
	{
		return await context.Documents
			.AsNoTracking()
			.Where(d => ids.Contains(d.Id) &&
					   d.Status != BaseStatuses.Deleted &&
					   d.Status != BaseStatuses.Inactive)
			.ToListAsync();
	}

	public async Task<Document?> GetByNameAsync(string name)
	{
		return await context.Documents
			.AsNoTracking()
			.FirstOrDefaultAsync(d => d.Name == name &&
									  d.Status != BaseStatuses.Deleted &&
									  d.Status != BaseStatuses.Inactive);
	}

	public async Task<Document?> GetBySlugAsync(string slug)
	{
		return await context.Documents
			.AsNoTracking()
			.FirstOrDefaultAsync(d => d.Slug == slug &&
									  d.Status != BaseStatuses.Deleted &&
									  d.Status != BaseStatuses.Inactive);
	}

	public async Task<PagedResult<Document>> GetPagedAsync(PagedRequest request)
	{
		var query = context.Documents.AsNoTracking()
			.Where(d => d.Status != BaseStatuses.Deleted && d.Status != BaseStatuses.Inactive);

		if (!string.IsNullOrEmpty(request.SortColumn))
		{
			query = request.SortDirection?.ToUpper() == "DESC"
				? query.OrderByDescending(e => EF.Property<object>(e, request.SortColumn))
				: query.OrderBy(e => EF.Property<object>(e, request.SortColumn));
		}
		else
		{
			query = query.OrderBy(d => d.Order ?? int.MaxValue).ThenBy(d => d.Name);
		}

		var totalCount = await query.CountAsync();

		var pageIndex = Math.Max(1, request.PageIndex);
		var pageSize = Math.Max(1, request.PageSize);

		var items = await query
			.Skip((pageIndex - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		return new PagedResult<Document>(items, totalCount, pageIndex, pageSize);
	}

	public async Task<List<Document>> GetByCategoryAsync(string category)
	{
		return await context.Documents
			.AsNoTracking()
			.Where(d => d.Category == category &&
						d.Status != BaseStatuses.Deleted &&
						d.Status != BaseStatuses.Inactive &&
						d.IsActive)
			.OrderBy(d => d.Order ?? int.MaxValue)
			.ThenBy(d => d.Name)
			.ToListAsync();
	}

	public async Task<List<Document>> GetActiveDocumentsAsync()
	{
		return await context.Documents
			.AsNoTracking()
			.Where(d => d.IsActive &&
						d.Status != BaseStatuses.Deleted &&
						d.Status != BaseStatuses.Inactive)
			.OrderBy(d => d.Order ?? int.MaxValue)
			.ThenBy(d => d.Name)
			.ToListAsync();
	}

	public async Task IncrementDownloadCountAsync(Guid documentId)
	{
		var document = await context.Documents.FindAsync(documentId);
		if (document != null)
		{
			document.DownloadCount = (document.DownloadCount ?? 0) + 1;
			document.LastDownloadDate = DateTime.UtcNow;
			await context.SaveChangesAsync();
		}
	}

	public async Task<Document?> GetByNameForUpdateAsync(string name)
	{
		return await context.Documents
			.FirstOrDefaultAsync(d => d.Name == name &&
									  d.Status != BaseStatuses.Deleted &&
									  d.Status != BaseStatuses.Inactive);
	}

	public async Task<Document?> GetBySlugForUpdateAsync(string slug)
	{
		return await context.Documents
			.FirstOrDefaultAsync(d => d.Slug == slug &&
									  d.Status != BaseStatuses.Deleted &&
									  d.Status != BaseStatuses.Inactive);
	}
}