using backend.context.identity.domain.vo;
using backend.context.todo.domain.entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.context.todo.infrastructure.persistence.configurations;

public class TodoConfiguration : IEntityTypeConfiguration<Todo>
{
    public void Configure(EntityTypeBuilder<Todo> builder)
    {
        builder.ToTable("Todos");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Title)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        builder.Property(t => t.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.Priority)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(t => t.UserId)
            .HasConversion(
                v => v.Value,
                v => new UserIdVO(v))
            .IsRequired();

        // Primitive collection EF Core 8
        builder.Property(t => t.BookingIds);
    }
}
