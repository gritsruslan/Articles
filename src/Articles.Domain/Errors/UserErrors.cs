using Articles.Domain.Constants;
using Articles.Shared.Result;

namespace Articles.Domain.Errors;

public static class UserErrors
{
	public static Error EmptyEmail() =>
		ErrorsFactory.RequiredParameter("email");

	public static Error InvalidEmailLength(string invalidEmail) =>
		ErrorsFactory.InvalidParameterLength(
			"email", invalidEmail, UserConstants.EmailMinLength, UserConstants.EmailMaxLength);

	public static Error InvalidEmail(string invalidEmail) =>
		ErrorsFactory.InvalidParameter("email", invalidEmail);

	public static Error UserWithThisEmailAlreadyExists(string repeatedEmail) =>
		new(ErrorType.Validation, "User with this email already exists", "email.exists", "email",
			repeatedEmail);

	public static Error EmptyDomainId() =>
		ErrorsFactory.RequiredParameter("domain-id");

	public static Error InvalidDomainIdLength(string invalidDomainId) =>
		ErrorsFactory.InvalidParameterLength(
			"domain-id", invalidDomainId, UserConstants.DomainIdMinLength, UserConstants.DomainIdMaxLength);

	public static Error InvalidDomainId(string invalidDomainId) =>
		ErrorsFactory.InvalidParameter("domain-id", invalidDomainId);

	public static Error UserWithThisDomainIdAlreadyExists(string repeatedDomainId) =>
		new(ErrorType.Conflict, "User with this domain id already exists", "domain-id.exists", "domain-id",
			repeatedDomainId);

	public static Error EmptyName() =>
		ErrorsFactory.RequiredParameter("name");

	public static Error InvalidNameLength(string invalidName) =>
		ErrorsFactory.InvalidParameterLength(
			"name", invalidName, UserConstants.NameMinLength, UserConstants.NameMaxLength);
}
