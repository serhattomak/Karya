namespace Karya.Domain.Common;

public class PagedRequest
{
	private int _pageIndex = 1;
	private int _pageSize = 10;

	public int PageIndex
	{
		get => _pageIndex;
		set => _pageIndex = Math.Max(1, value);
	}

	public int PageSize
	{
		get => _pageSize;
		set => _pageSize = Math.Max(1, Math.Min(100, value));
	}

	public string? SortColumn { get; set; }
	public string? SortDirection { get; set; }
}