using Articles.Application.Authentication;

namespace Articles.Infrastructure.Authentication;

internal sealed class ApplicationUserProvider : IApplicationUserProvider
{
	public RecognizedUser CurrentUser { get; set; } = null!;
}
