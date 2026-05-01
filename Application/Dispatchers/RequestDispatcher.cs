using Application.Decorators;
using Application.Requests;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Reflection;

namespace Application.Dispatchers;

internal class RequestDispatcher : IRequestDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private static readonly ConcurrentDictionary<Type, Func<IServiceProvider, object, CancellationToken, Task<object>>>
        _pipelineCache = new();

    public RequestDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> DispatchAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var pipeline = _pipelineCache.GetOrAdd(requestType, _ => BuildPipeline(requestType, typeof(TResponse)));

        var result = await pipeline(_serviceProvider, request!, cancellationToken);
        return (TResponse)result;
    }

    private static Func<IServiceProvider, object, CancellationToken, Task<object>> BuildPipeline(Type requestType, Type responseType)
    {
        var method = typeof(RequestDispatcher)
            .GetMethod(nameof(BuildPipelineCore), BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(requestType, responseType);

        return (Func<IServiceProvider, object, CancellationToken, Task<object>>)method.Invoke(null, null)!;
    }

    private static Func<IServiceProvider, object, CancellationToken, Task<object>> BuildPipelineCore<TRequest, TResponse>()
        where TRequest : IRequest<TResponse>
    {
        return async (serviceProvider, request, cancellationToken) =>
        {
            var typedRequest = (TRequest)request;

            var decorators = serviceProvider
                .GetServices<IRequestDecorator<TRequest, TResponse>>()
                .Reverse()
                .ToList();

            var handler = serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

            Func<Task<TResponse>> executeDelegate = () => handler.HandleAsync(typedRequest, cancellationToken);

            foreach (var decorator in decorators)
            {
                var next = executeDelegate;
                var d = decorator;
                executeDelegate = () => d.HandleAsync(typedRequest, next, cancellationToken);
            }

            var result = await executeDelegate();
            return result!;
        };
    }
}
