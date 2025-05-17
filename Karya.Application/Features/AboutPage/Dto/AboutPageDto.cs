using Karya.Application.Features.File.Dto;

namespace Karya.Application.Features.AboutPage.Dto;

public class AboutPageDto
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public List<string> Titles { get; set; } = [];
	public List<string> Descriptions { get; set; } = [];
	public List<string>? Urls { get; set; } = [];
	public List<Guid> FileIds { get; set; } = [];
	public List<FileDto> Files { get; set; } = [];
}