namespace Karya.Application.Features.HomePage.Dto;

public record CreateHomePageDto(
	string Title,
	string Description,
	string BackgroundImageUrl,
	List<Guid> FileIds,
	List<Guid> ProductIds
);