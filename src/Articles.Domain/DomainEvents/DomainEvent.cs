using MediatR;

namespace Articles.Domain.DomainEvents;

public abstract record DomainEvent : INotification;
