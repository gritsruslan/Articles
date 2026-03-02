using System.Threading.RateLimiting;
using Articles.Application.Interfaces.Authentication;
using Articles.Domain.Constants;
using Articles.Shared.Options;
using Microsoft.AspNetCore.Http.Features;

namespace Articles.API.Extensions;

public static class SecurityExtensions
{

	public static WebApplicationBuilder ConfigureMaxRequestBodySize(
		this WebApplicationBuilder builder)
	{
		const long maxRequestBodySize = FileFormats.MaxFileSize;

		builder.WebHost.ConfigureKestrel(options =>
			options.Limits.MaxRequestBodySize = maxRequestBodySize);
		builder.Services.Configure<FormOptions>(options =>
			options.MultipartBodyLengthLimit = maxRequestBodySize);

		return builder;
	}
}
