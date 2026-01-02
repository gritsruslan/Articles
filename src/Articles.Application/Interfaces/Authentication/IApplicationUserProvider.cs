using Articles.Application.Authentication;

namespace Articles.Application.Interfaces.Authentication;

public interface IApplicationUserProvider
{
	RecognizedUser CurrentUser { get; set; }
}
