using Newtonsoft.Json;

namespace Articles.Application.Authentication;

public sealed class RefreshToken
{
	[JsonProperty(Required = Required.Always)]
	public SessionId SessionId { get; set; }

	[JsonProperty(Required = Required.Always)]
	public UserId UserId { get; set; }

	[JsonProperty(Required = Required.Always)]
	public string Issuer { get; set; } = null!;

	[JsonProperty(Required = Required.Always)]
	public string Audience { get; set; } = null!;
}
