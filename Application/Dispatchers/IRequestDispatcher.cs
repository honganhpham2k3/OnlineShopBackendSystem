using Application.Requests;

namespace Application.Dispatchers;

internal interface IRequestDispatcher
{
    Task<TResponse> DispatchAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}
