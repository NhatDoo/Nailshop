using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace backend.context.nailservice.infrastructure.persistence.configurations;

public class NailServiceConfiguration : IEntityTypeConfiguration<domain.entity.NailService>
{
    public void Configure(EntityTypeBuilder<domain.entity.NailService> builder)
    {
        builder.ToTable("NailServices");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Category).HasConversion<string>();

        // 1 - N với Promotions
        builder.OwnsMany(s => s.Promotions, p =>
        {
            p.ToTable("Promotions");
            p.WithOwner().HasForeignKey("NailServiceId");
            p.HasKey("Id");
            p.Property(x => x.Title).IsRequired().HasMaxLength(200);
        });
    }
}
