using Calendar.Domain;
using Microsoft.Extensions.Logging;

namespace Calendar.Infrastructure;

public class CalendarDbContextSeed
{
    public static void Seed(CalendarDbContext context, ILogger<CalendarDbContextSeed> logger)
    {
        try
        {
            if (context.Events.Any())
                return;

            var begin = DateTime.Today.AddHours(15);

            var @event = new EventEntity
            {
                UserId = 1,
                Subject = "DemoCalendar",
                Description = "Check out the DemoCalendar",
                Begin = begin,
                End = begin.AddMinutes(15)
            };

            context.Events.Add(@event);
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception error: {ex}", ex);
        }
    }
}