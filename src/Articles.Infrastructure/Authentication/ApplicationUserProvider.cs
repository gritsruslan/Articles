using Articles.Application.Authentication;

namespace Articles.Infrastructure.Authentication;

public class ApplicationUserProvider : IApplicationUserProvider
{
	public RecognizedUser CurrentUser { get; set; } = null!;
}
