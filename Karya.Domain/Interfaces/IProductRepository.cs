using Karya.Domain.Entities;

namespace Karya.Domain.Interfaces;

public interface IProductRepository : IRepository<Product>
{
	Task<IQueryable<Product>> GetAllProductsAsync();
}