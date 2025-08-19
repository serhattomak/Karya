using File = Karya.Domain.Entities.File;

namespace Karya.Domain.Interfaces;

public interface IFileRepository : IRepository<File>
{
	Task<IQueryable<File>> GetAllFilesAsync();
	Task<List<File>> GetByIdsAsync(IEnumerable<Guid> ids);
	Task<File?> GetByHashAsync(string hash);
	Task<File?> GetByHashForUpdateAsync(string hash);
}