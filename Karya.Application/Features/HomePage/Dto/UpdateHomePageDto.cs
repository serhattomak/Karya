namespace Karya.Application.Features.HomePage.Dto;

public record UpdateHomePageDto(
	Guid Id,
	string Title,
	string Description,
	string BackgroundImageUrl,
	List<Guid> FileIds,
	List<Guid> ProductIds
);