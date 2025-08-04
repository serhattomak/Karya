using Karya.Domain.Enums;
using Karya.Domain.Interfaces;
using Karya.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using File = Karya.Domain.Entities.File;

namespace Karya.Persistence.Repositories;

public class FileRepository(AppDbContext context) : EfRepository<File>(context), IFileRepository
{
	public async Task<IQueryable<File>> GetAllFilesAsync()
	{
		var files = context.Files.AsNoTracking()
			.Where(x => x.Status != BaseStatuses.Deleted && x.Status != BaseStatuses.Inactive);
		return files;
	}

	public async Task<List<File>> GetByIdsAsync(IEnumerable<Guid> ids)
	{
		return await context.Files
			.AsNoTracking()
			.Where(f => ids.Contains(f.Id) &&
						f.Status != BaseStatuses.Deleted &&
						f.Status != BaseStatuses.Inactive)
			.ToListAsync();
	}

	public async Task<File?> GetByHashAsync(string hash)
	{
		return await context.Files
			.AsNoTracking()
			.FirstOrDefaultAsync(f => f.Hash == hash &&
									  f.Status != BaseStatuses.Deleted &&
									  f.Status != BaseStatuses.Inactive);
	}

	public async Task<File?> GetByHashForUpdateAsync(string hash)
	{
		return await context.Files
			.FirstOrDefaultAsync(f => f.Hash == hash &&
									  f.Status != BaseStatuses.Deleted &&
									  f.Status != BaseStatuses.Inactive);
	}
}