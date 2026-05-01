using Microsoft.Extensions.Hosting;

namespace Application.Persistences.Migrations;

internal interface IDatabaseMigrationService : IHostedService
{
    public Task UpdateDatabaseAsync(CancellationToken cancellationToken = default);
}
