using System.Text.RegularExpressions;
using Articles.Domain.Constants;
using Articles.Domain.Errors;
using Articles.Shared.Result;

namespace Articles.Domain.ValueObjects;

public partial record struct DomainId
{
	private DomainId(string domainId) => Value = domainId;

	public string Value { get; }

	public static Result<DomainId> Create(string domainId)
	{
		if (string.IsNullOrWhiteSpace(domainId))
		{
			return UserErrors.EmptyDomainId();
		}

		if (domainId.Length is < UserConstants.DomainIdMinLength or > UserConstants.DomainIdMaxLength)
		{
			return UserErrors.InvalidDomainIdLength(domainId);
		}

		if (!DomainIdRegex().IsMatch(domainId))
		{
			return UserErrors.InvalidDomainId(domainId);
		}

		return new DomainId(domainId);
	}

	// Use only in cases where you are sure that the domain id is valid
	public static DomainId CreateVerified(string domainIdStr) => new(domainIdStr);

	[GeneratedRegex(UserConstants.DomainIdRegex)]
	private static partial Regex DomainIdRegex();
}
