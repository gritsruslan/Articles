namespace Articles.Shared.Options;

public sealed class HealthCheckOptions
{
	public string PrometheusEndpoint { get; set; } = null!;

	public string LokiEndpoint { get; set; } = null!;

	public string JaegerEndpoint { get; set; } = null!;

	public string MinioEndpoint { get; set; } = null!;
}
