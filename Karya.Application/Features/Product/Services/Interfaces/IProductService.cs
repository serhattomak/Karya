using Karya.Application.Features.Product.Dto;
using Karya.Domain.Common;

namespace Karya.Application.Features.Product.Services.Interfaces;

public interface IProductService
{
	Task<Result<PagedResult<ProductDto>>> GetAllAsync(PagedRequest request);
	Task<Result<ProductDto>> GetByIdAsync(Guid id);
	Task<Result<ProductDto>> GetByNameAsync(string name);
	Task<Result<ProductDto>> GetBySlugAsync(string slug);
	Task<Result<ProductDto>> CreateAsync(CreateProductDto productDto);
	Task<Result<ProductDto>> UpdateAsync(UpdateProductDto productDto);
	Task<Result> DeleteAsync(Guid id);
}