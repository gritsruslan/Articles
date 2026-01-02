using Newtonsoft.Json;

namespace Articles.Application.Authentication;

public sealed class EmailConfirmationToken
{
	[JsonProperty(Required = Required.Always)]
	public UserId UserId { get; set; }

	[JsonProperty(Required = Required.Always)]
	public DateTime IssuedAt { get; set; }

	[JsonProperty(Required = Required.Always)]
	public DateTime ExpiresAt { get; set; }
}
