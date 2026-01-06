namespace Articles.Domain.DomainEvents;

public sealed record UserRegisteredDomainEvent(string Email, string Name) : DomainEvent;
