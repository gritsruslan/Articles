using Articles.Application.Behaviours;
using Microsoft.Extensions.DependencyInjection;

namespace Articles.Application;

public static class DependencyInjection
{
	public static IServiceCollection AddApplication(
		this IServiceCollection services)
	{
		services.AddMediatR(options =>
		{
			options.AddOpenBehavior(typeof(MetricsPipelineBehaviour<,>));
			options.AddOpenBehavior(typeof(UseCaseTracingPipelineBehaviour<,>));
			options.AddOpenBehavior(typeof(AuthorizationPipelineBehaviour<,>));
			options.AddOpenBehavior(typeof(UsageLimitingPipelineBehaviour<,>));
			options.RegisterServicesFromAssembly(AssemblyMarker.Assembly);
		});

		return services;
	}
}
