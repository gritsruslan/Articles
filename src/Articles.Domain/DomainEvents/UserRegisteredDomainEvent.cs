namespace Articles.Domain.DomainEvents;

public sealed record UserRegisteredDomainEvent(Guid UserId, string Email, string Name) : DomainEvent;
