namespace Karya.Application.Features.Product.Dto;

public record UpdateProductDto(Guid Id, string Name, List<string> Titles, List<string>? Subtitles, List<string>? Descriptions, List<string>? ListItems, List<string>? Urls, List<Guid>? FileIds);