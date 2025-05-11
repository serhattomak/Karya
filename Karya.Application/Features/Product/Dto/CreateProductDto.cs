namespace Karya.Application.Features.Product.Dto;

public record CreateProductDto(string Name, List<string> Titles, List<string> Descriptions, List<string>? Urls, List<Guid>? FileIds);