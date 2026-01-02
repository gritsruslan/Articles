using System.ComponentModel.DataAnnotations;

namespace Articles.Shared.Options;

public sealed class SessionOptions
{
	[Range(1, 31 * 24 - 1)] // less than one month
	public int ShortTermLifeSpanHours { get; init; }

	[Range(1, 10 * 12)] // less than 10 years
	public int LongTermLifeSpanMonths { get; init; }
}
