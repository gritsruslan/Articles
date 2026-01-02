using Articles.Application.Authentication;
using Articles.Application.Interfaces.Authentication;

namespace Articles.Infrastructure.Authentication;

public class ApplicationUserProvider : IApplicationUserProvider
{
	public RecognizedUser CurrentUser { get; set; } = null!;
}
