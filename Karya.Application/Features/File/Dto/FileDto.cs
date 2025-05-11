namespace Karya.Application.Features.File.Dto;

public class FileDto()
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Path { get; set; } = string.Empty;
	public string ContentType { get; set; } = string.Empty;
	public long Size { get; set; }
}