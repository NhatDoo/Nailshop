using backend.context.identity.domain.vo;
using backend.context.notification.domain.entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.context.notification.infrastructure.persistence.configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable("Notifications");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.UserId)
            .HasConversion(
                v => v.Value,
                v => new UserIdVO(v))
            .IsRequired();

        builder.Property(n => n.Type).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(n => n.Channel).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(n => n.EntityType).HasConversion<string>().HasMaxLength(50);
        
        builder.Property(n => n.Title).HasMaxLength(255).IsRequired();
        builder.Property(n => n.Message).HasMaxLength(2000).IsRequired();
        builder.Property(n => n.EntityId).HasMaxLength(150);
    }
}
