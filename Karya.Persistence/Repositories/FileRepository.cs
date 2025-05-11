using Karya.Domain.Enums;
using Karya.Domain.Interfaces;
using Karya.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using File = Karya.Domain.Entities.File;

namespace Karya.Persistence.Repositories;

public class FileRepository(AppIdentityDbContext context) : EfRepository<File>(context), IFileRepository
{
	public async Task<IQueryable<File>> GetAllFilesAsync()
	{
		var files = context.Files.AsNoTracking()
			.Where(x => x.Status != BaseStatuses.Deleted && x.Status != BaseStatuses.Inactive);
		return files;
	}
	public async Task<List<File>> GetByIdsAsync(IEnumerable<Guid> ids)
	{
		return await _context.Files.Where(f => ids.Contains(f.Id)).ToListAsync();
	}
}