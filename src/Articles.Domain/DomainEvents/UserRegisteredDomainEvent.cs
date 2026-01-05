namespace Articles.Domain.DomainEvents;

public sealed class UserRegisteredDomainEvent(string Email, string Name) : DomainEvent;
