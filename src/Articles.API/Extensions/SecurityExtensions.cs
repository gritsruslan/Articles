using System.Threading.RateLimiting;
using Articles.Application.Interfaces.Authentication;
using Articles.Shared.Options;
using Microsoft.AspNetCore.Http.Features;

namespace Articles.API.Extensions;

public static class SecurityExtensions
{
	public const long MaxRequestBodySize = 128_000_000;

	public static WebApplicationBuilder ConfigureMaxRequestBodySize(
		this WebApplicationBuilder builder)
	{
		builder.WebHost.ConfigureKestrel(options =>
			options.Limits.MaxRequestBodySize = MaxRequestBodySize);
		builder.Services.Configure<FormOptions>(options =>
			options.MultipartBodyLengthLimit = MaxRequestBodySize);

		return builder;
	}
}
