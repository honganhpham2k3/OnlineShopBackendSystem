namespace Application.Persistences.Repositories;

public interface IUnitOfWork
{
    public Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    public Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    public Task RollBackAsync(CancellationToken cancellationToken = default);
    public Task SaveChangeAsync(CancellationToken cancellationToken = default);
}
