using Karya.Domain.Common;
using Karya.Domain.Entities;
using Karya.Domain.Enums;

namespace Karya.Domain.Interfaces;

public interface IPageRepository : IRepository<Page>
{
	Task<List<Page>> GetAllByTypeAsync(PageTypes type);
	Task<PagedResult<Page>> GetPagedByTypeAsync(PageTypes type, PagedRequest request);
	Task<PagedResult<Page>> GetPagedAsync(PagedRequest request);
	Task<Page?> GetByNameAsync(string name);
	Task<Page?> GetBySlugAsync(string slug);
	Task<Page?> GetByNameForUpdateAsync(string name);
	Task<Page?> GetBySlugForUpdateAsync(string slug);
}