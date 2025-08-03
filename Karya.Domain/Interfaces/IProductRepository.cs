using Karya.Domain.Entities;

namespace Karya.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
	Task<IQueryable<Product>> GetAllProductsAsync();
	Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids);
	Task<Product?> GetByNameAsync(string name);
	Task<Product?> GetBySlugAsync(string slug);
}