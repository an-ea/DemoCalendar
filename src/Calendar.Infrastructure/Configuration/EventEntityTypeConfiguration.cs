using Calendar.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Calendar.Infrastructure.Configuration;

internal class EventEntityTypeConfiguration : IEntityTypeConfiguration<EventEntity>
{
    public void Configure(EntityTypeBuilder<EventEntity> builder)
    {
        builder.ToTable("Events");

        builder.HasKey(e => e.Id);
        builder.HasIndex(e => new { e.UserId, e.Begin, e.End });

        builder.Property(e => e.UserId).IsRequired();

        builder.Property(e => e.Subject)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.Begin).IsRequired();
        builder.Property(e => e.End).IsRequired();
    }
}