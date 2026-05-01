namespace Domain.DomainEvents.Abstractions;

internal interface IDomainEventHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{
    public Task HandleAsync(TDomainEvent domainEvent);
}
