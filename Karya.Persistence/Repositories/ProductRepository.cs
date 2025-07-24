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
			.Where(p => ids.Contains(p.Id))
			.ToListAsync();
	}
}