using System.Text.RegularExpressions;
using Articles.Domain.Constants;
using Articles.Domain.Errors;
using Articles.Shared.Result;

namespace Articles.Domain.ValueObjects;

public partial record struct DomainId
{
	private DomainId(string domainIdStr) => Value = domainIdStr;

	public string Value { get; }

	public static Result<DomainId> Create(string domainIdStr)
	{
		if (string.IsNullOrWhiteSpace(domainIdStr))
		{
			return UserErrors.EmptyDomainId(domainIdStr);
		}

		if (domainIdStr.Length is < UserConstants.DomainIdMinLength or > UserConstants.DomainIdMaxLength)
		{
			return UserErrors.InvalidDomainIdLength(domainIdStr);
		}

		if (!DomainIdRegex().IsMatch(domainIdStr))
		{
			return UserErrors.InvalidDomainId(domainIdStr);
		}

		return new DomainId(domainIdStr);
	}

	// Use only in cases where you are sure that the domain id is valid
	public static DomainId CreateVerified(string domainIdStr) => new(domainIdStr);

	[GeneratedRegex(UserConstants.DomainIdRegex)]
	private static partial Regex DomainIdRegex();
}
