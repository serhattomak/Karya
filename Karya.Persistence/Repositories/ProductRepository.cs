using Karya.Domain.Entities;
using Karya.Domain.Enums;
using Karya.Domain.Interfaces;
using Karya.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Karya.Persistence.Repositories;

public class ProductRepository(AppDbContext context) : EfRepository<Product>(context), IProductRepository
{
	public async Task<IQueryable<Product>> GetAllProductsAsync()
	{
		var products = context.Products.AsNoTracking()
			.Where(x => x.Status != BaseStatuses.Deleted && x.Status != BaseStatuses.Inactive);
		return products;
	}

	public async Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids)
	{
		return await context.Products
			.AsNoTracking()
			.Where(p => ids.Contains(p.Id) &&
					   p.Status != BaseStatuses.Deleted &&
					   p.Status != BaseStatuses.Inactive)
			.ToListAsync();
	}

	public async Task<Product?> GetByNameAsync(string name)
	{
		return await context.Products
			.AsNoTracking()
			.FirstOrDefaultAsync(p => p.Name == name &&
									  p.Status != BaseStatuses.Deleted &&
									  p.Status != BaseStatuses.Inactive);
	}

	public async Task<Product?> GetBySlugAsync(string slug)
	{
		return await context.Products
			.AsNoTracking()
			.FirstOrDefaultAsync(p => p.Slug == slug &&
									  p.Status != BaseStatuses.Deleted &&
									  p.Status != BaseStatuses.Inactive);
	}

	public async Task<Product?> GetByIdForUpdateAsync(Guid id)
	{
		return await context.Products
			.FirstOrDefaultAsync(p => p.Id == id &&
									  p.Status != BaseStatuses.Deleted &&
									  p.Status != BaseStatuses.Inactive);
	}

	public async Task<Product?> GetByNameForUpdateAsync(string name)
	{
		return await context.Products
			.FirstOrDefaultAsync(p => p.Name == name &&
									  p.Status != BaseStatuses.Deleted &&
									  p.Status != BaseStatuses.Inactive);
	}

	public async Task<Product?> GetBySlugForUpdateAsync(string slug)
	{
		return await context.Products
			.FirstOrDefaultAsync(p => p.Slug == slug &&
									  p.Status != BaseStatuses.Deleted &&
									  p.Status != BaseStatuses.Inactive);
	}
}