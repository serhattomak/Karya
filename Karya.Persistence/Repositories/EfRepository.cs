using Karya.Domain.Interfaces;
using Karya.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Karya.Persistence.Repositories;

public class EfRepository<T>(AppDbContext context) : IRepository<T>
	where T : class
{
	protected readonly AppDbContext _context = context;
	private readonly DbSet<T> _entities = context.Set<T>();

	public async Task<T?> GetByIdAsync(Guid id)
	{
		return await _entities.FindAsync(id);
	}

	public async Task<List<T>> GetAllAsync()
	{
		return await _entities.ToListAsync();
	}

	public async Task AddAsync(T entity)
	{
		await _entities.AddAsync(entity);
	}

	public void UpdateAsync(T entity)
	{
		_entities.Update(entity);
	}

	public void DeleteAsync(T entity)
	{
		_entities.Remove(entity);
	}

	public async Task SaveChangesAsync()
	{
		await _context.SaveChangesAsync();
	}
}