using Karya.Domain.Common;

namespace Karya.Domain.Entities;

public class File : BaseEntity
{
	public string Name { get; set; } = string.Empty;
	public string Path { get; set; } = string.Empty;
	public string ContentType { get; set; } = string.Empty;
	public long Size { get; set; }
	public string Hash { get; set; } = string.Empty;
}