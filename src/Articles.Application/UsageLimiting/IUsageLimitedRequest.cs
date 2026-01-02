namespace Articles.Application.UsageLimiting;

internal interface IUsageLimitedRequest
{
	public string UsageLimitingPolicyName { get; }
}
