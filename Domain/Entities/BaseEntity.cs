using Domain.DomainEvents.Abstractions;

namespace Domain.Entities;

internal class BaseEntity<TId>
{
    protected TId Id = default!;
    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
    public void AddDomainEvent(IDomainEvent domainEvent)
        => _domainEvents.Add(domainEvent);
    public void ClearDomainEvents()
        => _domainEvents.Clear();
}
