using Karya.Domain.Common;
using Karya.Domain.Entities;

namespace Karya.Domain.Interfaces;

public interface IDocumentRepository : IRepository<Document>
{
	Task<IQueryable<Document>> GetAllDocumentsAsync();
	Task<List<Document>> GetByIdsAsync(IEnumerable<Guid> ids);
	Task<Document?> GetByNameAsync(string name);
	Task<Document?> GetBySlugAsync(string slug);
	Task<PagedResult<Document>> GetPagedAsync(PagedRequest request);
	Task<List<Document>> GetByCategoryAsync(string category);
	Task<List<Document>> GetActiveDocumentsAsync();
	Task IncrementDownloadCountAsync(Guid documentId);
	Task<Document?> GetByNameForUpdateAsync(string name);
	Task<Document?> GetBySlugForUpdateAsync(string slug);
}