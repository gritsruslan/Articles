using Articles.Shared.Result;

namespace Articles.Shared.Abstraction;

public class PagedRequest
{
	private PagedRequest(int page, int pageSize)
	{
		Page = page;
		PageSize = pageSize;
	}

	public static Result<PagedRequest> Create(int page, int pageSize)
	{
		if (page <= 0)
		{
			return new Error(ErrorType.InvalidValue, "Page must be greater than zero");
		}

		if (pageSize <= 0)
		{
			return new Error(ErrorType.InvalidValue, "Page size must be greater than zero");
		}

		return new PagedRequest(page, pageSize);
	}

	public int Page { get; set; }

	public int PageSize { get; set; }

	public int Skip => (Page - 1) * PageSize;

	public int Take => PageSize;
}
