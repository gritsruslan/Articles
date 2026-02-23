using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Articles.Infrastructure.Health;

internal abstract class HttpHealthCheck(IHttpClientFactory factory, string endpoint) : IHealthCheck
{
	private string Endpoint { get; } = endpoint;

	public async Task<HealthCheckResult> CheckHealthAsync(
		HealthCheckContext context,
		CancellationToken cancellationToken = default)
	{
		try
		{
			var httpClient = factory.CreateClient();
			using var response = await httpClient.GetAsync(Endpoint, cancellationToken);

			if (!response.IsSuccessStatusCode)
			{
				return HealthCheckResult.Unhealthy();
			}

			return HealthCheckResult.Healthy();
		}
		catch(Exception ex)
		{
			return HealthCheckResult.Unhealthy(ex.Message);
		}
	}
}
