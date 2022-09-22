using Calendar.Domain;
using Calendar.Infrastructure.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Infrastructure;

/// <summary>
/// Represents a calendar database context.
/// </summary>
public class CalendarDbContext : DbContext
{
    public CalendarDbContext(DbContextOptions<CalendarDbContext> options) : base(options)
    {
    }

    public DbSet<EventEntity> Events { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new EventEntityTypeConfiguration());
    }
}