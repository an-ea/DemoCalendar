using Calendar.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Calendar.Api.DependencyInjection;

public static class ServiceProviderExtensions
{
    public static IServiceProvider InitDatabase(this IServiceProvider services, bool isInMemoryDb)
    {
        using var scope = services.CreateScope();
        var serviceProvider = scope.ServiceProvider;
            
        using var context = serviceProvider.GetRequiredService<CalendarDbContext>();

        var alreadyExists = !isInMemoryDb && context.Database.GetService<IRelationalDatabaseCreator>().Exists();

        if (!isInMemoryDb)
            context.Database.Migrate();

        if (!alreadyExists)
            CalendarDbContextSeed.Seed(context, serviceProvider.GetRequiredService<ILogger<CalendarDbContextSeed>>());

        return services;
    }
}