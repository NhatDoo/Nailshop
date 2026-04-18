using backend.context.identity.domain.vo;
using backend.context.naildesign.domain.entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.context.naildesign.infrastructure.persistence.configurations;

public class NailDesignConfiguration : IEntityTypeConfiguration<NailDesign>
{
    public void Configure(EntityTypeBuilder<NailDesign> builder)
    {
        builder.ToTable("NailDesigns");
        builder.HasKey(n => n.Id);

        builder.Property(n => n.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(n => n.ImageUrl)
            .HasMaxLength(500)
            .IsRequired();

        // Chuyển đổi UserIdVO <-> Guid
        builder.Property(n => n.OwnerId)
            .HasConversion(
                v => v.Value,
                v => new UserIdVO(v))
            .IsRequired();

        // Lưu Enum dưới dạng String để dễ đọc trong DB
        builder.Property(n => n.Type)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(n => n.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(n => n.Description)
            .HasMaxLength(1000);
    }
}
