using Articles.Shared.Options;

namespace Articles.Infrastructure.Health;

internal sealed class LokiHealthCheck(IHttpClientFactory factory, IOptions<HealthCheckOptions> options):
	HttpHealthCheck(factory, options.Value.LokiEndpoint);
