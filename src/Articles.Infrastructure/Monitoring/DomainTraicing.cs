using System.Diagnostics;
using Articles.Domain.Constants;

namespace Articles.Infrastructure.Monitoring;

public sealed class DomainTraicing
{
	public static readonly ActivitySource ActivitySource = new(OverallConstants.DomainName);
}
