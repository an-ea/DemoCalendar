using AutoMapper;
using Calendar.Application.Commands.CreateEvent;
using Calendar.Application.Logging;
using Calendar.Domain.Abstract;
using Calendar.Infrastructure;
using Calendar.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Calendar.Api.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        services.AddTransient<ICalendar, Domain.Calendar>();
        services.AddTransient<IEventRepository, EventRepository>();

        services
            .AddMediatR(typeof(CreateEventCommand))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

        services.AddAutoMapper(cfg => cfg.AddProfiles(new Profile[]
            {
                new Application.Mapping.MappingProfile(),
                new Calendar.Infrastructure.Mapping.MappingProfile()
            }));

        return services;
    }


    public static IServiceCollection AddDatabaseContext(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<CalendarDbContext>(options => options
            .UseSqlServer(
                connectionString,
                sqlOptions => sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null))
            .LogTo(
                (eventId, _) => eventId.Id == CoreEventId.ExecutionStrategyRetrying,
                eventData =>
                {
                    if (eventData is not ExecutionStrategyEventData retryEventData)
                        return;

                    var exceptions = retryEventData.ExceptionsEncountered;
                    Console.WriteLine($"Retry #{exceptions.Count} with delay {retryEventData.Delay} due to error: {exceptions[^1].Message}");
                }));

        return services;
    }

    public static IServiceCollection AddInMemoryDatabaseContext(this IServiceCollection services)
    {
        services.AddDbContext<CalendarDbContext>(opt => opt.UseInMemoryDatabase("CalendarDb"));
        return services;
    }

}