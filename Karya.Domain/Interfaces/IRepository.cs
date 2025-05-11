namespace Karya.Domain.Interfaces;

public interface IRepository<T> where T : class
{
	Task<T?> GetByIdAsync(Guid id);
	Task<List<T>> GetAllAsync();
	Task AddAsync(T entity);
	void UpdateAsync(T entity);
	void DeleteAsync(T entity);
	Task SaveChangesAsync();
}