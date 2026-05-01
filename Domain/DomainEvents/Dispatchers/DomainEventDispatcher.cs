using Domain.DomainEvents.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Domain.DomainEvents.Dispatchers;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DomainEventDispatcher> _logger;
    public DomainEventDispatcher(IServiceProvider serviceProvider, ILogger<DomainEventDispatcher> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (domainEvent == null) throw new ArgumentNullException(nameof(domainEvent));

        var eventType = domainEvent.GetType();
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(eventType);
        var handlers = _serviceProvider.GetServices(handlerType).ToList();

        _logger.LogInformation("Dispatching domain event of type {EventType} to {HandlerCount} handlers.",
            eventType.Name, handlers.Count);

        var tasks = handlers.Select(handler =>
        {
            var method = handlerType.GetMethod("HandleAsync")!;
            return (Task)method.Invoke(handler, [domainEvent, cancellationToken])!;
        });

        await Task.WhenAll(tasks);
    }
}