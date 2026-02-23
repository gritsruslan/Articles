using Articles.Shared.Options;

namespace Articles.Infrastructure.Health;

internal sealed class MinioHealthCheck(IHttpClientFactory factory, IOptions<HealthCheckOptions> options)
	: HttpHealthCheck(factory, options.Value.MinioEndpoint);
