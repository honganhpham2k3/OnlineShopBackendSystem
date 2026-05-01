namespace Domain.DomainEvents.Abstractions;

public interface IDomainEventDispatcher
{
    public Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
