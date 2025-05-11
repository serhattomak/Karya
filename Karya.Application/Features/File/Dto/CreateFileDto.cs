namespace Karya.Application.Features.File.Dto;

public record CreateFileDto(string Name, string Path, string ContentType, long Size);