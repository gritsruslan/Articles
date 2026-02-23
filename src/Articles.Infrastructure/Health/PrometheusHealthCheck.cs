using Articles.Shared.Options;

namespace Articles.Infrastructure.Health;

internal sealed class PrometheusHealthCheck(IHttpClientFactory factory, IOptions<HealthCheckOptions> options) :
	HttpHealthCheck(factory, options.Value.PrometheusEndpoint);
