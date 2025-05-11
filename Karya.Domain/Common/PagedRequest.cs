namespace Karya.Domain.Common;

public abstract class PagedRequest
{
	public int Page { get; set; } = 1;
	public int PageSize { get; set; } = 10;
	public string? SortColumn { get; set; }
	public string? SortDirection { get; set; } = "ASC";
}