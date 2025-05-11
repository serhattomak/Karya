namespace Karya.Application.Features.File.Dto;

public record UpdateFileDto(Guid Id, string Name, string Path, string ContentType, long Size);