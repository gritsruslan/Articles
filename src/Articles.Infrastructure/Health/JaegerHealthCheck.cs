using Articles.Shared.Options;

namespace Articles.Infrastructure.Health;

internal sealed class JaegerHealthCheck(IHttpClientFactory factory, IOptions<HealthCheckOptions> options)
	: HttpHealthCheck(factory, options.Value.JaegerEndpoint);
