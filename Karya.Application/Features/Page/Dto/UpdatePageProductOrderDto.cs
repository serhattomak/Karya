namespace Karya.Application.Features.Page.Dto;

public record UpdatePageProductOrderDto(
	Guid PageId,
	List<Guid> ProductIds
);