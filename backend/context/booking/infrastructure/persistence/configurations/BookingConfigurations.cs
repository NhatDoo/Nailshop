using backend.context.booking.domain.entity;
using backend.context.identity.domain.vo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.context.booking.infrastructure.persistence.configurations;

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");
        builder.HasKey(b => b.Id);

        // Chuyển đổi UserIdVO sang Guid khi lưu vào DB
        builder.Property(b => b.CustomerId)
            .HasConversion(
                v => v.Value,
                v => new UserIdVO(v))
            .IsRequired();

        builder.Property(b => b.ServiceName).HasMaxLength(200).IsRequired();
        builder.Property(b => b.Price).HasColumnType("decimal(18,2)");
        builder.Property(b => b.Status).HasConversion<string>(); // Lưu Enum dưới dạng String cho dễ đọc
        builder.Property(b => b.BookingTime).IsRequired();
    }
}
