﻿namespace Karya.Domain.Common;

public class PagedResult<T>
{
	public List<T> Items { get; set; } = [];
	public int TotalCount { get; set; }
	public int PageNumber { get; set; }
	public int PageSize { get; set; }
	public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
	public bool HasPreviousPage => PageNumber > 1;
	public bool HasNextPage => PageNumber < TotalPages;
	public int PreviousPageNumber => HasPreviousPage ? PageNumber - 1 : 1;
	public int NextPageNumber => HasNextPage ? PageNumber + 1 : TotalPages;
	public PagedResult() { }
	public PagedResult(List<T> items, int totalCount, int pageNumber, int pageSize)
	{
		Items = items;
		TotalCount = totalCount;
		PageNumber = pageNumber;
		PageSize = pageSize;
	}
}