namespace Articles.Shared.Abstraction;

public class PagedData<T>(IEnumerable<T> items, int totalCount, int page, int pageSize)
{
	public IEnumerable<T> Items { get; set; } = items;

	public int TotalCount { get; set; } = totalCount;

	public int PageNumber { get; set; } = page;

	public int PageSize { get; set; } = pageSize;

	public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

	public bool HasNextPage => PageNumber < TotalPages;
}
