namespace Articles.API.Requests;

internal sealed record LoginRequest(string Email, string Password, bool RememberMe);
