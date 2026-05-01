using Application.Persistences.Migrations;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddHostedService<IDatabaseMigrationService>();

        return services;
    }
}
