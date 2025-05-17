namespace Karya.Domain.Common;

public class PagedRequest
{
	public int PageIndex { get; set; } = 1;
	public int PageSize { get; set; } = 10;
	public string? SortColumn { get; set; }
	public string? SortDirection { get; set; } = "ASC";
}