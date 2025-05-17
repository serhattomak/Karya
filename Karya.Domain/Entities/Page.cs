using Karya.Domain.Common;
using Karya.Domain.Enums;

namespace Karya.Domain.Entities;

public class Page : BaseEntity
{
	public PageTypes PageType { get; set; }
	public string Name { get; set; } = string.Empty;
	public List<string> Titles { get; set; } = [];
	public List<string> Descriptions { get; set; } = [];
	public List<string>? Urls { get; set; } = [];
	public List<Guid>? FileIds { get; set; } = [];
	public List<Guid>? ProductIds { get; set; } = [];
	public List<string>? AdditionalFields { get; set; } = [];
}