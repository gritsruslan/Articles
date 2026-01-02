namespace Articles.API.Requests;

internal sealed record RegistrationRequest(string Name, string Email, string DomainId, string Password);
