using Articles.Domain.Constants;
using Articles.Shared.Result;

namespace Articles.Domain.Errors;

public static class UserErrors
{
	public static Error EmptyEmail(string invalidEmail) =>
		AbstractErrors.EmptyParameter("email", invalidEmail);

	public static Error InvalidEmailLength(string invalidEmail) =>
		AbstractErrors.InvalidParameterLength(
			"email", invalidEmail, UserConstants.EmailMinLength, UserConstants.EmailMaxLength);

	public static Error InvalidEmail(string invalidEmail) =>
		AbstractErrors.InvalidParameter("email", invalidEmail);

	public static Error UserWithThisEmailAlreadyExists(string repeatedEmail) =>
		new(ErrorType.Conflict, "User with this email already exists", "email.exists", "email",
			repeatedEmail);

	public static Error EmptyDomainId(string invalidDomainId) =>
		AbstractErrors.EmptyParameter("domain-id", invalidDomainId);

	public static Error InvalidDomainIdLength(string invalidDomainId) =>
		AbstractErrors.InvalidParameterLength(
			"domain-id", invalidDomainId, UserConstants.DomainIdMinLength, UserConstants.DomainIdMaxLength);

	public static Error InvalidDomainId(string invalidDomainId) =>
		AbstractErrors.InvalidParameter("domain-id", invalidDomainId);

	public static Error UserWithThisDomainIdAlreadyExists(string repeatedDomainId) =>
		new(ErrorType.Conflict, "User with this domain id already exists", "domain-id.exists", "domain-id",
			repeatedDomainId);

	public static Error EmptyName(string invalidName) =>
		AbstractErrors.EmptyParameter("name", invalidName);

	public static Error InvalidNameLength(string invalidName) =>
		AbstractErrors.InvalidParameterLength(
			"name", invalidName, UserConstants.NameMinLength, UserConstants.NameMaxLength);
}
