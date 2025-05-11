using Karya.Domain.Entities;
using Karya.Domain.Enums;
using Karya.Domain.Interfaces;
using Karya.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Karya.Persistence.Repositories;

public class ProductRepository(AppIdentityDbContext context) : EfRepository<Product>(context), IProductRepository
{
	public async Task<IQueryable<Product>> GetAllProductsAsync()
	{
		var products = context.Products.AsNoTracking()
			.Where(x => x.Status != BaseStatuses.Deleted && x.Status != BaseStatuses.Inactive);
		return products;
	}
}